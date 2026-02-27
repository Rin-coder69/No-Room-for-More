using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CGL.Audio
{
	// Handles audio playback from a single source in the game.
	[RequireComponent(typeof(AudioSource))]
	public class AudioEmitter : MonoBehaviour
	{
		// actual Unity AudioSource component that this class wraps and manages.
		private AudioSource audioSource;
				
		// event that notifies listeners (typically AudioManager) when a non-looping audio clip has finished playing.
		public event UnityAction<AudioEmitter> onAudioFinishedPlaying;

		// checks if the audio source is currently playing.
		public bool IsPlaying => audioSource.isPlaying;
		// checks if the audi source is set to loop playback.
		public bool IsLooping =>audioSource.loop;

		// initialize the AudioEmitter by getting the AudioSource component and ensuring it doesn't play automatically.
		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			audioSource.playOnAwake = false; // We want to control playback manually
		}

		// plays an audio clip with the specified settings.
		public void PlayAudioClip(AudioClip clip, AudioConfigurationData settings, bool loop, Vector3 position = default)
		{
			if (clip == null) return;

			// configure the audio source
			audioSource.clip = clip;
			if (settings != null) settings.ApplyTo(audioSource); // apply all settings from the configuration
			audioSource.transform.position = position;
			audioSource.loop = loop;
			audioSource.time = 0f; // Start from the beginning of the clip
			audioSource.Play();

			// if not looping, set up a coroutine to detect when the clip finishes
			if (!loop)
			{
				StartCoroutine(FinishedPlayingCR(clip.length));
			}
		}

		// returns the currently assigned AudioClip, or null if nothing is assigned
		public AudioClip GetClip()
		{
			return audioSource.clip;
		}

		// used when the game is unpaused, to pick up SFX from where they left.
		public void Resume()
		{
			audioSource.UnPause();
		}

		// used when the game is paused.
		public void Pause()
		{
			audioSource.Pause();
		}

		// immediately stops the audio playback.
		public void Stop()
		{
			audioSource.Stop();
		}

		// allows a looping sound to finish its current playthrough and then stop.
		// will trigger the onAudioFinishedPlaying event when complete.
		public void Finish()
		{
			if (audioSource.loop)
			{
				audioSource.loop = false; // Disable looping
				float timeRemaining = audioSource.clip.length - audioSource.time; // Calculate remaining time
				StartCoroutine(FinishedPlayingCR(timeRemaining));
			}
		}



		// coroutine that waits for the specified clip length and then fires the completion event.
		IEnumerator FinishedPlayingCR(float clipLength)
		{
			yield return new WaitForSeconds(clipLength);

			NotifyBeingDone();
		}

		// invokes the onAudioFinishedPlaying event to notify listeners (typically the AudioManager)
		// that this emitter has finished playing its assigned clip.
		private void NotifyBeingDone()
		{
			onAudioFinishedPlaying?.Invoke(this); // AudioManager will pick this up
		}
	}
}