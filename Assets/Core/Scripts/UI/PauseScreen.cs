using UnityEngine;
using CGL.Events;
using CGL.Data;

namespace CGL.UI
{
	public class PauseScreen : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("UI panel to show when the game is paused.")]
		private GameObject pausePanel;

		[SerializeField]
		[Tooltip("Stores the current pause state. True = paused, False = resumed.")]
		private BoolDataSO pauseData;

		[SerializeField]
		[Tooltip("Raise this event when quitting the game from the pause.")]
		private EventSO onQuitGameEvent;

		[SerializeField]
		[Tooltip("Subscribe to this event to show/hide the pause panel.")]
		private EventSO onPauseEvent;

		[SerializeField]
		[Tooltip("Subscribe to this event to react to pause state changes.")]
		private EventSO onPauseChangedEvent;

		private void OnEnable()
		{
			onPauseChangedEvent?.Subscribe(OnPauseChanged);
		}

		private void OnDisable()
		{
			onPauseChangedEvent?.Unsubscribe(OnPauseChanged);
		}

		private void OnPauseChanged()
		{
			if (pauseData == null) return;
			pausePanel?.SetActive(pauseData.value);
		}

		public void OnResume()
		{
			onPauseEvent?.RaiseEvent();
		}

		public void OnQuit()
		{
			onPauseEvent?.RaiseEvent();
			onQuitGameEvent?.RaiseEvent();
		}
	}
}