using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CGL.Scene
{
	// Ensures that a specified scene is loaded into the game.
	// Useful for managing persistent scenes like UI, audio, or manager scenes.
	public class SceneBootstrapper : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Scene that should be loaded.")]
		string sceneName;

		// - Single: Unload all current scenes, load the new scene
		// - Additive: Keep current scenes, add the new scene
		[SerializeField]
		[Tooltip("How scene should be loaded. Single - unload all scenes. Additive - add new scene")]
		LoadSceneMode mode;

		[SerializeField] bool useAsyncLoading = false;

		// OnValidate executes when editor changes are made
		private void OnValidate()
		{
			// Verify scene exists in build settings
			if (!string.IsNullOrEmpty(sceneName))
			{
				bool sceneExists = false;
				for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
				{
					// get scene names from scenes in build
					string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
					string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
					// check for scene name match
					if (sceneNameFromPath == sceneName)
					{
						sceneExists = true;
						break;
					}
				}
				if (!sceneExists)
				{
					Debug.LogWarning($"Scene '{sceneName}' is not in build settings!");
				}
			}

			// don't use async loading when loaind single scene
			if (useAsyncLoading && mode == LoadSceneMode.Single)
			{
				Debug.LogWarning("SceneEnforcer: Async loading with Single mode may cause issues!", this);
			}
		}

		private void Awake()
		{
			// checks if the required scene is loaded, and loads it if not present.
			if (!IsSceneLoaded(sceneName))
			{
				if (useAsyncLoading)
				{
					StartCoroutine(LoadSceneAsyncCR());

				}
				else
				{
					SceneManager.LoadScene(sceneName, mode);
				}
			}
		}

		
		// checks if a scene with the specified name is currently loaded.
		public static bool IsSceneLoaded(string sceneName)
		{
			// iterate through all currently loaded scenes
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				// get the scene at current index
				var scene = SceneManager.GetSceneAt(i);

				// use case-insensitive comparison
				if (string.Equals(scene.name, sceneName, System.StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			// scene not found in loaded scenes
			return false;
		}

		private IEnumerator LoadSceneAsyncCR()
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
			while (!asyncLoad.isDone)
			{
				yield return null;
			}
		}
	}
}