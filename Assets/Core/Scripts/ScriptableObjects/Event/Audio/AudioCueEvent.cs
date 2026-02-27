using UnityEngine;
using UnityEngine.Events;

namespace CGL.Audio
{
	[CreateAssetMenu(fileName = "AudioCueEvent", menuName = "CGL/Events/Audio/Audio Cue Event")]
	public class AudioCueEvent : BaseSO
	{
		public AudioCuePlayAction OnAudioCuePlay;

		public void OnPlayEvent(AudioCueData audioCue, AudioConfigurationData audioConfiguration, Vector3 positionInSpace = default)
		{
			if (OnAudioCuePlay != null)
			{
				OnAudioCuePlay.Invoke(audioCue, audioConfiguration, positionInSpace);
			}
		}

		public delegate void AudioCuePlayAction(AudioCueData audioCueData, AudioConfigurationData audioConfigurationData, Vector3 positionInSpace);
	}
}