using UnityEngine;
using UnityEngine.Audio;

namespace CGL.Audio
{
	// Scriptable Object that handles audio configuration settings for different types of sounds in the game.
	// Allows for consistent audio settings that can be reused across multiple audio sources.
	[CreateAssetMenu(fileName = "AudioConfigurationData", menuName = "CGL/Data/Audio/Audio Configuration")]
	public class AudioConfigurationData : ScriptableObject
	{
		// defines the category of audio this configuration applies to.
		public enum Type
		{
			Sfx,    // sound effects
			Music   // background music
		}

		// defines the playback priority of the audio
		public enum Priority
		{
			Highest = 0,    // will almost never be culled
			High = 64,      // very unlikely to be culled
			Standard = 128, // default priority
			Low = 194,      // more likely to be culled when many sounds play
			VeryLow = 256,  // will be culled first when many sounds play
		}

		[Tooltip("Category of sound - affects how it's processed in the audio system")]
		public Type type = Type.Sfx;

		[Tooltip("Determines which sounds get culled first when too many are playing")]
		public Priority priority = Priority.Standard;

		[Tooltip("Audio Mixer Group to route this sound through - controls effects and volume grouping")]
		public AudioMixerGroup audioMixerGroup = null;

		[Tooltip("Base volume level from 0 (silent) to 1 (full volume)")]
		[Range(0, 1)] public float volume = 1;

		[Tooltip("Random volume variation (±) applied to each sound instance for natural variety")]
		[Range(0, 0.2f)] public float volumeRandom = 0;

		[Tooltip("Pitch adjustment: 0=normal, negative=lower, positive=higher")]
		[Range(0, 3)] public float pitch = 0;

		[Tooltip("Random pitch variation (±) in semitones for more natural sound repetition")]
		[Range(0, 12)] public float pitchRandom = 0;

		[Tooltip("How 3D the sound is: 0=2D (no positioning), 1=fully 3D positioned")]
		[Range(0, 1)] public float spatialBlend = 1;

		[Tooltip("How volume decreases with distance (Logarithmic is more realistic)")]
		public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

		[Tooltip("Distance (in units) at which the sound begins to fade")]
		[Range(0.01f, 5)] public float minDistance = 0.1f;

		[Tooltip("Maximum distance (in units) at which the sound can be heard")]
		[Range(5, 100)] public float maxDistance = 50;

		// applies all configuration settings to the given AudioSource component.
		public void ApplyTo(AudioSource audioSource)
		{
			// Apply mixer group
			audioSource.outputAudioMixerGroup = this.audioMixerGroup;

			// Apply volume and pitch settings with randomization
			audioSource.priority = (int)this.priority;
			audioSource.volume = this.volume + (Random.Range(-this.volumeRandom, this.volumeRandom));
			audioSource.pitch = this.pitch + (Random.Range(-this.pitchRandom, this.pitchRandom));

			//// Apply spatial settings
			audioSource.spatialBlend = this.spatialBlend;
			audioSource.rolloffMode = this.rolloffMode;
			audioSource.minDistance = this.minDistance;
			audioSource.maxDistance = this.maxDistance;
		}
	}
}
