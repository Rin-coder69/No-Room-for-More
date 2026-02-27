using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using CGL.DesignPatterns;
using CGL.Data;
using CGL.Events;

namespace CGL.Audio
{
	public class AudioManager : Singleton<AudioManager>
	{
		[SerializeField]
		[Tooltip("Audio mixer that controls volume groups.")]
		AudioMixer audioMixer;

		[SerializeField]
		[Tooltip("Master volume data shared with the options UI.")]
		FloatDataSO masterVolume;

		[SerializeField]
		[Tooltip("SFX volume data shared with the options UI.")]
		FloatDataSO sfxVolume;

		[SerializeField]
		[Tooltip("Music volume data shared with the options UI.")]
		FloatDataSO musicVolume;

		[SerializeField]
		[Tooltip("Raised when any audio volume has changed.")]
		EventSO onAudioChange;

		[SerializeField]
		[Tooltip("Event channel to receive audio cue play requests.")]
		AudioCueEvent onPlayAudioCueEvent;

		[SerializeField]
		[Tooltip("Maximum number of audio emitters in the pool.")]
		[Range(1, 32)]
		private int maxEmitters = 16;

		[SerializeField]
		[Tooltip("Default audio configuration used when none is specified.")]
		private AudioConfigurationData defaultAudioConfiguration;

		private List<AudioEmitter> audioEmitters = new List<AudioEmitter>();

		// constants for storing volume settings in PlayerPrefs
		const string MASTER_VOLUME = "MasterVolume";
		const string SFX_VOLUME = "SFXVolume";
		const string MUSIC_VOLUME = "MusicVolume";

		private void OnEnable()
		{
			onAudioChange?.Subscribe(OnAudioChange);
		}

		private void OnDisable()
		{
			onAudioChange?.Unsubscribe(OnAudioChange);
		}

		public override void Awake()
		{
			base.Awake();

			// subscribe to audio cue play event
			if (onPlayAudioCueEvent != null)
				onPlayAudioCueEvent.OnAudioCuePlay += OnPlayAudioCue;

			// load saved volume settings from PlayerPrefs
			if (masterVolume != null)
			{
				masterVolume.value = PlayerPrefs.GetFloat(MASTER_VOLUME, 1);
				SetGroupVolume(MASTER_VOLUME, masterVolume.value);
			}

			if (sfxVolume != null)
			{
				sfxVolume.value = PlayerPrefs.GetFloat(SFX_VOLUME, 1);
				SetGroupVolume(SFX_VOLUME, sfxVolume.value);
			}

			if (musicVolume != null)
			{
				musicVolume.value = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1);
				SetGroupVolume(MUSIC_VOLUME, musicVolume.value);
			}
		}

		private void OnDestroy()
		{
			// unsubscribe from audio cue event to prevent memory leaks
			if (onPlayAudioCueEvent != null)
				onPlayAudioCueEvent.OnAudioCuePlay -= OnPlayAudioCue;
		}

		// updates the audio mixer settings when the audio change event is triggered
		public void OnAudioChange()
		{
			if (masterVolume != null) SetGroupVolume(MASTER_VOLUME, masterVolume.value);
			if (sfxVolume != null) SetGroupVolume(SFX_VOLUME, sfxVolume.value);
			if (musicVolume != null) SetGroupVolume(MUSIC_VOLUME, musicVolume.value);
		}

		public void OnPlayAudioCue(AudioCueData audioCueData, AudioConfigurationData audioConfigurationData, Vector3 positionInSpace)
		{
			if (audioCueData == null) return;

			// get available audio emitter
			AudioEmitter audioEmitter = GetAudioEmitter();
			if (audioEmitter == null) return;

			audioEmitter.PlayAudioClip(audioCueData.GetClip(), audioConfigurationData, audioCueData.looping, positionInSpace);
		}

		public void OnPlayAudioClip(AudioClip audioClip)
		{
			if (audioClip == null) return;

			AudioEmitter audioEmitter = GetAudioEmitter();
			if (audioEmitter == null) return;

			Debug.Log("play audio clip");
			// use default configuration if available
			audioEmitter.PlayAudioClip(audioClip, defaultAudioConfiguration, false);
		}

		private AudioEmitter GetAudioEmitter(Transform parent = null)
		{
			// clean up destroyed emitters
			audioEmitters.RemoveAll(ae => ae == null);

			// find an available emitter
			AudioEmitter audioEmitter = audioEmitters.Find(ae => !ae.IsPlaying);

			// create new emitter if none available and under max pool size
			if (audioEmitter == null)
			{
				if (audioEmitters.Count >= maxEmitters)
				{
					Debug.LogWarning("AudioManager: Max emitter pool size reached!", this);
					return null;
				}

				GameObject emitterObject = new GameObject("AudioEmitter");
				audioEmitter = emitterObject.AddComponent<AudioEmitter>();
				emitterObject.transform.SetParent(parent != null ? parent : transform);
				audioEmitters.Add(audioEmitter);
			}

			return audioEmitter;
		}

		// retrieves the current volume of a specified audio group in linear form
		public float GetGroupVolume(string groupName)
		{
			if (audioMixer == null) return 0f;
			audioMixer.GetFloat(groupName, out float dB);
			return DBToLinear(dB);
		}

		// sets the volume of a specific audio group, converts to decibels, and saves to PlayerPrefs
		public void SetGroupVolume(string groupName, float value)
		{
			if (audioMixer == null) return;
			audioMixer.SetFloat(groupName, LinearToDB(value));
			PlayerPrefs.SetFloat(groupName, value);
		}

		// converts linear volume (0-1) to decibels for the audio mixer
		public static float LinearToDB(float linear)
		{
			return (linear != 0) ? 20.0f * Mathf.Log10(linear) : -144.0f;
		}

		// converts decibels back to linear volume (0-1)
		public static float DBToLinear(float dB)
		{
			return Mathf.Pow(10.0f, dB / 20.0f);
		}
	}
}