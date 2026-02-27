using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using CGL.DesignPatterns;
using CGL.Events;
using CGL.UI;
using CGL.Data;

namespace CGL.Scene
{
	public class SceneLoader : Singleton<SceneLoader>
	{
		[SerializeField] private GameObject loadingPanel;

		[SerializeField]
		[Tooltip("Float data with the load progress amount.")]
		private FloatDataSO loadProgressData;

		[SerializeField]
		[Tooltip("Minimum time to show loading screen.")]
		private float minimumLoadTime = 1.0f;

		[Header("Events")]
		[SerializeField]
		[Tooltip("Raise this event with a scene name to trigger loading.")]
		StringEventSO onSceneLoadEvent;

		[SerializeField]
		[Tooltip("Raised when loading starts.")]
		EventSO onSceneLoadStartEvent;
		
		[SerializeField]
		[Tooltip("Raised when loading is done.")]
		EventSO onSceneLoadDoneEvent;

		[Header("Screen Fade")]

		[SerializeField]
		[Tooltip("Raised to start screen fade in.")]
		private EventSO onScreenFadeIn;

		[SerializeField]
		[Tooltip("Raised to start screen fade out.")]
		private EventSO onScreenFadeOut;

		[SerializeField]
		[Tooltip("Set to true when screen fade is done.")]
		private BoolDataSO onScreenFadeDone;

		private bool isLoading = false;    // tracks if a scene is currently loading
		public bool IsLoading => isLoading;

		private void OnEnable()
		{
			onSceneLoadEvent?.Subscribe(Load);
		}

		private void OnDisable()
		{
			onSceneLoadEvent?.Unsubscribe(Load);
		}

		public void Load(string sceneName)
		{
			// prevent multiple simultaneous loads
			if (isLoading)
			{
				Debug.LogWarning("Scene load already in progress!");
				return;
			}

			// validate scene exists
			if (!Application.CanStreamedLevelBeLoaded(sceneName))
			{
				Debug.LogError($"Scene {sceneName} does not exist in build settings!");
				return;
			}

			StartCoroutine(LoadSceneAsyncCR(sceneName));
		}

		// coroutine that handles the scene loading process
		private IEnumerator LoadSceneAsyncCR(string sceneName)
		{
			// set load values
			isLoading = true;
			float loadStartTime = Time.time;

			// reset time scale in case it was modified
			Time.timeScale = 1.0f;

			// raise load start event
			onSceneLoadStartEvent?.RaiseEvent();

			// fade out current scene, wait till done
			onScreenFadeOut?.RaiseEvent();
			yield return new WaitUntil(() => onScreenFadeOut == null || onScreenFadeDone == null || onScreenFadeDone.value);

			// start async scene loading
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
			asyncOperation.allowSceneActivation = false;

			// show loading panel
			loadingPanel?.SetActive(true);
			loadProgressData.value = 0.0f;

			// update loading progress until complete
			while (!asyncOperation.isDone)
			{
				// convert progress to 0-1 range (async progress goes to 0.9)
				float progress = (asyncOperation.progress / 0.9f);

				// update loading progress bar
				loadProgressData.value = progress;

				// continue until loading complete and minimum time elapsed
				if (asyncOperation.progress >= 0.9f && Time.time - loadStartTime >= minimumLoadTime)
				{
					break;
				}

				yield return null;
			}

			// hide loading panel
			loadingPanel?.SetActive(false);

			// allow scene to activate and wait for completion
			asyncOperation.allowSceneActivation = true;
			yield return new WaitUntil(() => asyncOperation.isDone);

			// raise load complete event
			print("load done");
			onSceneLoadDoneEvent?.RaiseEvent();

			// complete loading process
			isLoading = false;

			// fade in new scene
			onScreenFadeIn?.RaiseEvent();
			yield return new WaitUntil(() => onScreenFadeOut == null || onScreenFadeDone == null || onScreenFadeDone.value);
		}
	}
}