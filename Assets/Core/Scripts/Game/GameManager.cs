using System.Collections;
using UnityEngine;

using CGL.Data;
using CGL.DesignPatterns;
using CGL.Events;
using CGL.Scene;
using CGL.Actor;

namespace CGL.Core
{
	public class GameManager : Singleton<GameManager>
	{
		[Header("Data")]
		[SerializeField]
		[Tooltip("Contains scene-specific data like spawn points and cameras.")]
		private SceneDataSO sceneData;

		[SerializeField]
		[Tooltip("Ordered scene progression for level sequencing.")]
		private SceneProgressionSO sceneProgression;

		[SerializeField]
		[Tooltip("Scene name of the main menu to load when quitting.")]
		private StringDataSO mainMenuSceneNameData;

		[SerializeField]
		[Tooltip("Tracks the current game score.")]
		private IntDataSO scoreData;

		[SerializeField]
		[Tooltip("Tracks the highest score achieved.")]
		private IntDataSO highScoreData;

		[SerializeField]
		[Tooltip("Tracks whether the game is paused.")]
		private BoolDataSO pauseData;

		[SerializeField]
		[Tooltip("The instantiated player GameObject.")]
		private GameObjectDataSO playerGameObjectData;

		[Header("Settings")]
		[SerializeField]
		[Tooltip("Seconds to wait after player death before respawning.")]
		private float respawnDelay = 2f;

		[Header("Events")]
		[SerializeField]
		[Tooltip("Raised when score changes, passes points to add.")]
		private IntEventSO onScoreEvent;

		[SerializeField]
		[Tooltip("Raised to toggle pause state.")]
		private EventSO onPauseEvent;

		[SerializeField]
		[Tooltip("Raised after pause state has been updated.")]
		private EventSO onPauseChangedEvent;

		[SerializeField]
		[Tooltip("Raised when player dies, passes player GameObject.")]
		private GameObjectEventSO onPlayerDeathEvent;

		[SerializeField]
		[Tooltip("Raised when a new game starts.")]
		private EventSO onStartGameEvent;

		[SerializeField]
		[Tooltip("Raised when scene is fully loaded and ready.")]
		private EventSO onSceneReadyEvent;

		[SerializeField]
		[Tooltip("Raised when player has been spawned, passes player GameObject.")]
		private GameObjectEventSO onPlayerSpawnedEvent;

		[SerializeField]
		[Tooltip("Raised when requesting to go to the next level.")]
		private EventSO onNextLevelEvent;

		[SerializeField]
		[Tooltip("Raised when the player has quit the game.")]
		private EventSO onQuitGameEvent;

		[SerializeField]
		[Tooltip("Raised to load a scene by name.")]
		private StringEventSO onSceneLoadEvent;

		public override void Awake()
		{
			base.Awake();
			// load high score from player preferences
			if (highScoreData != null)
				highScoreData.value = PlayerPrefs.GetInt("highscore", 0);

			// sync progression index to the currently loaded scene
			sceneProgression?.ResolveCurrentScene();
		}

		private void OnEnable()
		{
			onScoreEvent?.Subscribe(OnAddScore);
			onSceneReadyEvent?.Subscribe(OnSceneReady);
			onPauseEvent?.Subscribe(OnPause);
			onStartGameEvent?.Subscribe(OnGameStart);
			onPlayerDeathEvent?.Subscribe(OnPlayerDeath);
			onNextLevelEvent?.Subscribe(OnNextLevel);
			onQuitGameEvent?.Subscribe(OnQuitGame);
		}

		private void OnDisable()
		{
			onScoreEvent?.Unsubscribe(OnAddScore);
			onSceneReadyEvent?.Unsubscribe(OnSceneReady);
			onPauseEvent?.Unsubscribe(OnPause);
			onStartGameEvent?.Unsubscribe(OnGameStart);
			onPlayerDeathEvent?.Unsubscribe(OnPlayerDeath);
			onNextLevelEvent?.Unsubscribe(OnNextLevel);
			onQuitGameEvent?.Unsubscribe(OnQuitGame);
		}

		public void OnSceneReady()
		{
			if (sceneData == null) return;

			switch (sceneData.SceneType)
			{
				case SceneDescriptor.SceneType.Menu:
					// show cursor for menu navigation
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
					break;
				case SceneDescriptor.SceneType.Level:
					StartLevel();
					break;
				case SceneDescriptor.SceneType.Cinematic:
					// disable player input and let cinematic play
					break;
			}
		}

		public void OnGameStart()
		{
			if (scoreData != null) scoreData.value = 0;

			// unpause if returning from a paused state
			if (pauseData != null && pauseData.value) OnPause();

			// reset progression to first level and load it
			if (sceneProgression != null)
			{
				sceneProgression.Reset();
				onSceneLoadEvent?.RaiseEvent(sceneProgression.CurrentLevel.SceneName);
			}
		}

		private void StartLevel()
		{
			// hide and lock cursor during gameplay
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			OnPlayerSpawn();
		}

		private void OnPlayerSpawn()
		{
			if (sceneData == null) return;

			if (sceneData.PlayerPrefab == null)
			{
				Debug.LogWarning("GameManager: PlayerPrefab is not assigned in SceneData.", this);
				return;
			}

			if (sceneData.StartPlayerSpawn == null)
			{
				Debug.LogWarning("GameManager: StartPlayerSpawn is not assigned in SceneData.", this);
				return;
			}

			// instantiate player at spawn point
			GameObject player = Instantiate(
				sceneData.PlayerPrefab,
				sceneData.StartPlayerSpawn.position,
				sceneData.StartPlayerSpawn.rotation);

			if (playerGameObjectData != null) playerGameObjectData.value = player;

			// use CameraTarget marker if present, fall back to player root
			CameraTarget cameraTarget = player.GetComponentInChildren<CameraTarget>();
			Transform cameraFollowTarget = cameraTarget != null ? cameraTarget.transform : player.transform;

			if (cameraTarget == null)
				Debug.LogWarning("GameManager: no CameraTarget found on player prefab, falling back to root transform.", this);

			// configure cinemachine camera to follow and look at the camera target
			if (sceneData.PlayerCamera != null)
			{
				sceneData.PlayerCamera.Follow = cameraFollowTarget;
				sceneData.PlayerCamera.LookAt = cameraFollowTarget;
			}
			else
			{
				Debug.LogWarning("GameManager: PlayerCamera is not assigned in SceneData.", this);
			}

			// notify other systems that player has spawned
			onPlayerSpawnedEvent?.RaiseEvent(player);
		}

		public void OnPlayerDeath(GameObject player)
		{
			StartCoroutine(RespawnCR(player));
		}

		public void OnNextLevel()
		{
			if (sceneProgression == null)
			{
				Debug.LogWarning("GameManager: sceneProgression is not assigned.", this);
				return;
			}

			SceneDataSO next = sceneProgression.MoveToNextLevel();
			if (next != null)
			{
				onSceneLoadEvent?.RaiseEvent(next.SceneName);
			}
			else
			{
				// no more levels — load main menu
				onSceneLoadEvent?.RaiseEvent(mainMenuSceneNameData?.value);
			}
		}

		public void OnQuitGame()
		{
			onSceneLoadEvent?.RaiseEvent(mainMenuSceneNameData?.value);
		}

		public void OnAddScore(int points)
		{
			if (scoreData == null) return;

			// add points to current score
			scoreData.value += points;

			// update high score if beaten
			if (highScoreData != null && scoreData.value >= highScoreData.value)
			{
				highScoreData.value = scoreData.value;
				PlayerPrefs.SetInt("highscore", highScoreData.value);
			}
		}

		public void OnPause()
		{
			if (pauseData == null) return;

			// toggle pause state
			pauseData.value = !pauseData.value;

			// apply side effects
			Time.timeScale = pauseData.value ? 0f : 1f;
			Cursor.visible = pauseData.value;
			Cursor.lockState = pauseData.value ? CursorLockMode.None : CursorLockMode.Locked;

			// notify listeners that pause state has changed
			onPauseChangedEvent?.RaiseEvent();
		}

		private IEnumerator RespawnCR(GameObject player)
		{
			yield return new WaitForSeconds(respawnDelay);
			Destroy(player);
			StartLevel();
		}
	}
}