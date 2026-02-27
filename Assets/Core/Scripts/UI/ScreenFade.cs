using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CGL.Events;

#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif
using CGL.Data;

namespace CGL.Scene
{
	/// Handles screen fade transitions using a UI Image component.
	/// Can fade in/out with customizable colors, duration and easing.
	public class ScreenFade : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Image to use for fade.")] 
		private Image image;

		[Header("Fade Settings")]
		
		[SerializeField]
		[Tooltip("Duration of the fade transition.")]
		private float fadeTime = 1.0f;
		
		[SerializeField]
		[Tooltip("Start fade color (usually fully opaque).")]
		private Color startColor = Color.black;
		
		[SerializeField]
		[Tooltip("End fade color (usually fully transparent).")]
		private Color endColor = new Color(0, 0, 0, 0); 

		[SerializeField]
		[Tooltip("Automatically fade in when scene starts.")]
		private bool autoFade = true;

		[SerializeField]
		[Tooltip("Set to true when screen fade is done.")]
		private BoolDataSO onScreenFadeDone;

		[SerializeField]
		[Tooltip("Raised when starting screen fade in.")]
		private EventSO onScreenFadeIn;

		[SerializeField]
		[Tooltip("Raised when starting screen fade out.")]
		private EventSO onScreenFadeOut;

		// current state of the fade transition
		public enum FadeState { Idle, In, Out }
		public FadeState CurrentState { get; private set; } = FadeState.Idle;

		// current fade transition has completed
		public bool IsDone { get; private set; } = false;

		// reference to the active fade coroutine
		private Coroutine fadeCoroutine;

		// validate required references on startup
		private void Awake()
		{
			if (image == null)
			{
				Debug.LogError("Image reference not set in ScreenFade!");
				enabled = false;
				return;
			}
			// fade in is set to auto fade
			if (autoFade)
			{
				FadeIn();
			}
		}

		private void OnEnable()
		{
			onScreenFadeIn?.Subscribe(FadeIn);
			onScreenFadeOut?.Subscribe(FadeOut);
		}

		private void OnDisable()
		{
			onScreenFadeIn?.Unsubscribe(FadeIn);
			onScreenFadeOut?.Unsubscribe(FadeOut);
		}

		private void Update()
		{
#if UNITY_EDITOR
			// debug fade in/out using arrow keys
			if (Keyboard.current.upArrowKey.wasPressedThisFrame) FadeIn();
			if (Keyboard.current.downArrowKey.wasPressedThisFrame) FadeOut();
#endif
		}

		// fade from start color to end color
		public void FadeIn()
		{
			// stop coroutine if active
			if (fadeCoroutine != null)
				StopCoroutine(fadeCoroutine);
			// start coroutine to fade
			fadeCoroutine = StartCoroutine(FadeRoutineCR(startColor, endColor, fadeTime, FadeState.In));
		}

		// fade from end color to start color
		public void FadeOut()
		{
			// stop coroutine if active
			if (fadeCoroutine != null)
				StopCoroutine(fadeCoroutine);
			// start coroutine to fade
			fadeCoroutine = StartCoroutine(FadeRoutineCR(endColor, startColor, fadeTime, FadeState.Out));
		}

		private IEnumerator FadeRoutineCR(Color colorFrom, Color colorTo, float duration, FadeState fadeState)
		{
			// fade start
			onScreenFadeDone.value = false;
			IsDone = false;
			CurrentState = fadeState;

			// interpolate fade colors over fade time
			float timer = 0.0f;
			while (timer < duration)
			{
				timer += Time.deltaTime;
				// progress is normalize 0 <-> 1
				float progress = timer / duration;
				// interpolate colors
				image.color = Color.Lerp(colorFrom, colorTo, progress);

				yield return null;
			}

			// fade complete
			image.color = colorTo;
			IsDone = true;
			onScreenFadeDone.value = true;
			CurrentState = FadeState.Idle;
			fadeCoroutine = null;
		}

		// set fade image to end or start color
		public void SetImmediate(bool fadeIn)
		{
			// stop coroutine if active
			if (fadeCoroutine != null)
				StopCoroutine(fadeCoroutine);
			fadeCoroutine = null;

			// set image color to end or start color based on fadeIn
			image.color = fadeIn ? endColor : startColor;

			IsDone = true;
			CurrentState = FadeState.Idle;
		}
	}
}