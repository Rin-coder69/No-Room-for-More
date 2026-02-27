using CGL.Audio;
using CGL.Events;
using Unity.Cinemachine;
using UnityEngine;

namespace CGL.Scene
{
	// placed in each scene to define its type and configuration.
	// populates the shared SceneDataSO with scene-specific settings on load.
	public class SceneDescriptor : MonoBehaviour
	{
		public enum SceneType
		{
			Menu,       // user interface scene like main menu or options
			Level,      // gameplay scene requiring player and camera setup
			Cinematic   // non-interactive cutscene
		}

		[SerializeField]
		[Tooltip("The type of scene this descriptor represents.")]
		private SceneType sceneType;

		[SerializeField]
		[Tooltip("The player prefab to instantiate when the scene starts.")]
		private GameObject playerPrefab;

		[SerializeField]
		[Tooltip("The initial spawn point for the player in this scene.")]
		private Transform startPlayerSpawn;

		[SerializeField]
		[Tooltip("Cinemachine camera hooked up to follow the player at runtime.")]
		private CinemachineCamera playerCamera;

		[SerializeField]
		[Tooltip("The music cue that plays while this scene is active.")]
		private AudioCueData backgroundMusic;

		[SerializeField]
		[Tooltip("Subscribe to this event to be notified when the scene has loaded.")]
		private EventSO onSceneLoadCompleteEvent;

		[SerializeField]
		[Tooltip("Raised when the scene is ready to start.")]
		private EventSO onSceneReadyEvent;

		[SerializeField]
		[Tooltip("Shared data container populated with this scene's settings.")]
		private SceneDataSO sceneData;

		private bool loaded = false;

		private void OnEnable()
		{
			onSceneLoadCompleteEvent?.Subscribe(OnSceneLoaded);
		}

		private void OnDisable()
		{
			onSceneLoadCompleteEvent?.Unsubscribe(OnSceneLoaded);
			loaded = false;
		}

#if UNITY_EDITOR
		private void Start()
		{
			// when entering play mode directly in this scene, trigger load manually
			if (!loaded) OnSceneLoaded();
		}
#endif

		private void OnSceneLoaded()
		{
			if (loaded) return;
			loaded = true;

			if (sceneData == null)
			{
				Debug.LogWarning("SceneDescriptor: sceneData is not assigned.", this);
				return;
			}

			// populate shared data via method so SceneDataSO controls its own fields
			sceneData.Populate(sceneType, playerPrefab, startPlayerSpawn, playerCamera, backgroundMusic);

			onSceneReadyEvent?.RaiseEvent();
		}
	}
}