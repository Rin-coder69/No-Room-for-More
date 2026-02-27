using System;
using UnityEngine;

namespace CGL.Audio
{
	// Scriptable Object that defines a set of audio clips that can be played together as a logical sound "cue".
	// Supports organization of multiple audio variations with different playback sequence modes.
	[CreateAssetMenu(fileName = "AudioCueData", menuName = "CGL/Data/Audio/Audio Cue Data")]
	public class AudioCueData : ScriptableObject
	{
		[Tooltip("If true, the audio will play continuously until stopped")]
		public bool looping = false;

		[Tooltip("Groups of audio clips with different playback patterns")]
		[SerializeField] private AudioClipGroup audioClipGroups;

		public AudioClip GetClip() => audioClipGroups.GetNextClip();

		// different modes for selecting the next clip from a group.
		public enum SequenceMode
		{
			Random, // select clips completely randomly, allowing for immediate repetition
			RandomNoImmediateRepeat, // select clips randomly, but avoid playing the same clip twice in a row
			Sequential, // play clips in the exact order they appear in the array, looping back to the beginning.
		}

		// represents a group of AudioClips that can be treated as one, and provides automatic randomisation or sequencing based on the <c>SequenceMode</c> value.
		[Serializable]
		public class AudioClipGroup
		{
			[Tooltip("How to select the next clip: randomly, randomly with no repeats, or in sequence")]
			public SequenceMode sequenceMode = SequenceMode.RandomNoImmediateRepeat;

			[Tooltip("The audio clips that make up this group - variations of the same sound")]
			public AudioClip[] audioClips;

			// tracks the index of the next clip to be played
			// initialized to -1 to indicate that no clip has been selected yet
			private int nextClipToPlay = -1;

			// tracks the index of the most recently played clip
			// used to prevent immediate repetition in RandomNoImmediateRepeat mode
			private int lastClipPlayed = -1;

			// chooses the next clip in the sequence, either following the order or randomly.
			public AudioClip GetNextClip()
			{
				// return first clip if there is only one clip to play
				if (audioClips.Length == 1)
				{
					return audioClips[0];
				}

				if (nextClipToPlay == -1)
				{
					// index needs to be initialised: 0 if Sequential, random if otherwise
					nextClipToPlay = (sequenceMode == SequenceMode.Sequential) ? 0 : UnityEngine.Random.Range(0, audioClips.Length);
				}
				else
				{
					// select next clip index based on the appropriate SequenceMode
					switch (sequenceMode)
					{
						case SequenceMode.Random:
							// completely random selection - can repeat the same clip
							nextClipToPlay = UnityEngine.Random.Range(0, audioClips.Length);
							break;

						case SequenceMode.RandomNoImmediateRepeat:
							// random but avoids playing the same clip twice in a row
							do
							{
								nextClipToPlay = UnityEngine.Random.Range(0, audioClips.Length);
							} while (nextClipToPlay == lastClipPlayed);
							break;

						case SequenceMode.Sequential:
							// play clips in order, looping back to the start when reaching the end
							nextClipToPlay = (nextClipToPlay + 1) % audioClips.Length;
							break;
					}
				}

				// store the selected clip index for comparison in the next call
				lastClipPlayed = nextClipToPlay;

				return audioClips[nextClipToPlay];
			}


		}
	}
}