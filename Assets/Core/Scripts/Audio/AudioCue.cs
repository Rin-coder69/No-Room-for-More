using UnityEngine;

namespace CGL.Audio
{
	// Connects the data-focused AudioCueData with the runtime event system.
	public class AudioCue : MonoBehaviour
	{
		[Header("Audio definition")]

		[Tooltip("The audio data asset defining what clips will be played.")]
		[SerializeField] private AudioCueData audioCueData;

		[Tooltip("If true, this audio will play automatically shortly after scene load.")]
		[SerializeField] private bool playOnStart = false;

		[Header("Configuration")]

		[Tooltip("Event channel to communicate with the AudioManager.")]
		[SerializeField] private AudioCueEvent audioCueEvent;

		[Tooltip("Settings for how this audio cue should be played.")]
		[SerializeField] private AudioConfigurationData audioConfigurationData;

		// called when the object is activated, initiates playback if playOnStart is true.
		private void Start()
		{
			if (playOnStart)
			{
				Play();
			}
		}

		// called when the object is deactivated.
		private void OnDisable()
		{
			playOnStart = false;
		}

		// plays the audio cue at the current transform position.
		public void Play()
		{
			if (audioCueEvent == null || audioCueData == null || audioConfigurationData == null) return;
			audioCueEvent.OnPlayEvent(audioCueData, audioConfigurationData, transform.position);
		}
	}
}