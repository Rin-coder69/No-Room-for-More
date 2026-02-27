using Unity.Cinemachine;
using UnityEngine;
using CGL.Audio;

namespace CGL.Scene
{
	// shared data container for a scene's configuration.
	// asset data is set in the inspector and never changes.
	// runtime data is populated by SceneDescriptor when the scene loads.
	[CreateAssetMenu(fileName = "SceneData", menuName = "CGL/Data/Scene/SceneData")]
	public class SceneDataSO : BaseSO
	{
		[Header("Asset Data")]
		[SerializeField]
		[Tooltip("Unity scene name as it appears in build settings.")]
		private string sceneName;

		[SerializeField]
		[Tooltip("Human-readable name shown in UI (e.g. Forest Level, Boss Arena).")]
		private string sceneDisplayName;

		[SerializeField]
		[Tooltip("The type of scene this data represents.")]
		private SceneDescriptor.SceneType sceneType;

		[Header("Runtime Data")]
		[SerializeField]
		[Tooltip("Populated at runtime by SceneDescriptor — player prefab to instantiate in this scene.")]
		private GameObject playerPrefab;

		[SerializeField]
		[Tooltip("Populated at runtime by SceneDescriptor — transform position where the player will spawn.")]
		private Transform startPlayerSpawn;

		[SerializeField]
		[Tooltip("Populated at runtime by SceneDescriptor — Cinemachine camera hooked up to follow the player.")]
		private CinemachineCamera playerCamera;

		[SerializeField]
		[Tooltip("Populated at runtime by SceneDescriptor — music cue that plays while this scene is active.")]
		private AudioCueData backgroundMusic;

		// asset data properties
		public string SceneName => sceneName;
		public string SceneDisplayName => sceneDisplayName;
		public SceneDescriptor.SceneType SceneType => sceneType;

		// runtime data properties
		public GameObject PlayerPrefab => playerPrefab;
		public Transform StartPlayerSpawn => startPlayerSpawn;
		public CinemachineCamera PlayerCamera => playerCamera;
		public AudioCueData BackgroundMusic => backgroundMusic;

		// called by SceneDescriptor on load to fill in scene-specific runtime data
		public void Populate(SceneDescriptor.SceneType type, GameObject prefab,
			Transform spawn, CinemachineCamera camera, AudioCueData music)
		{
			sceneType = type;
			playerPrefab = prefab;
			startPlayerSpawn = spawn;
			playerCamera = camera;
			backgroundMusic = music;
		}
	}
}