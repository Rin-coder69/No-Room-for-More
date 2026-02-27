using UnityEngine;
using CGL.Data;

namespace CGL.Scene
{
	// stores the ordered sequence of levels in the game.
	// tracks the current level index and provides access to level data for loading.
	// assign to SceneLoader and GameManager via the inspector — no scene references needed.
	[CreateAssetMenu(fileName = "SceneProgression", menuName = "CGL/Scene/SceneProgression")]
	public class SceneProgressionSO : BaseSO
	{
		[SerializeField]
		[Tooltip("Ordered list of levels in the game progression sequence.")]
		private SceneDataSO[] levels;

		[SerializeField]
		[Tooltip("Index of the current level. Modified at runtime as the player progresses.")]
		private int currentIndex = 0;

		// current level data
		public SceneDataSO CurrentLevel => IsValid ? levels[currentIndex] : null;

		// true if there is a next level in the sequence
		public bool HasNextLevel => IsValid && currentIndex < levels.Length - 1;

		// true if the player is on the first level
		public bool IsFirstLevel => currentIndex == 0;

		// total number of levels in the progression
		public int LevelCount => levels != null ? levels.Length : 0;

		// one-based level number for display in UI ("Level 2 of 5")
		public int CurrentLevelNumber => currentIndex + 1;

		// true if the levels array is assigned and the current index is in range
		private bool IsValid => levels != null && levels.Length > 0 &&
			currentIndex >= 0 && currentIndex < levels.Length;

		// advances to the next level and returns its data.
		// returns null if already on the last level.
		public SceneDataSO MoveToNextLevel()
		{
			if (!HasNextLevel)
			{
				Debug.LogWarning("SceneProgressionSO: no next level, already at the end of progression.", this);
				return null;
			}

			currentIndex++;
			return CurrentLevel;
		}

		// returns the data for the next level without advancing the index.
		// useful for previewing the next level name or icon in UI.
		public SceneDataSO PeekNextLevel()
		{
			if (!HasNextLevel) return null;
			return levels[currentIndex + 1];
		}

		// returns level data at any index — useful for level select screens.
		public SceneDataSO GetLevel(int index)
		{
			if (levels == null || index < 0 || index >= levels.Length)
			{
				Debug.LogWarning($"SceneProgressionSO: index {index} is out of range.", this);
				return null;
			}

			return levels[index];
		}

		// jumps directly to a level by index — useful for level select or debug skipping.
		public bool SetLevel(int index)
		{
			if (levels == null || index < 0 || index >= levels.Length)
			{
				Debug.LogWarning($"SceneProgressionSO: cannot set level, index {index} is out of range.", this);
				return false;
			}

			currentIndex = index;
			return true;
		}

		// resets progression back to the first level.
		public void Reset()
		{
			currentIndex = 0;
		}

		// resolves the current index by matching the active scene name against the levels array.
		// call from GameManager.Awake so the index is always correct regardless of how the scene was loaded.
		// falls back to index 0 if the active scene is not found in the levels array.
		public void ResolveCurrentScene()
		{
			if (levels == null || levels.Length == 0) return;

			string activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

			for (int i = 0; i < levels.Length; i++)
			{
				if (levels[i] == null) continue;
				if (levels[i].SceneName == activeScene)
				{
					currentIndex = i;
					return;
				}
			}

			// active scene not in progression — default to first level
			Debug.LogWarning($"SceneProgressionSO: active scene '{activeScene}' not found in levels, defaulting to index 0.", this);
			currentIndex = 0;
		}

		private void OnValidate()
		{
			if (levels == null || levels.Length == 0)
			{
				Debug.LogWarning("SceneProgressionSO: levels array is empty.", this);
				return;
			}

			if (currentIndex < 0 || currentIndex >= levels.Length)
				Debug.LogWarning($"SceneProgressionSO: currentIndex {currentIndex} is out of range.", this);

			for (int i = 0; i < levels.Length; i++)
			{
				if (levels[i] == null)
					Debug.LogWarning($"SceneProgressionSO: level at index {i} is null.", this);
			}
		}
	}
}