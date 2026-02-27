using UnityEngine;
using CGL.Data;
using CGL.Events;

namespace CGL.UI
{
	public class MainMenu : MonoBehaviour
	{
		[Header("Panels")]
		[SerializeField]
		[Tooltip("Main title panel.")]
		private GameObject titlePanel;

		[SerializeField]
		[Tooltip("Options panel.")]
		private GameObject optionsPanel;

		[SerializeField]
		[Tooltip("Credits panel.")]
		private GameObject creditsPanel;


		[SerializeField]
		[Tooltip("Event to call to start the game.")]
		private EventSO onStartGameEvent;

		private void Start()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			ShowTitle();
		}

		private void ShowPanel(GameObject panel)
		{
			titlePanel?.SetActive(false);
			optionsPanel?.SetActive(false);
			creditsPanel?.SetActive(false);

			panel?.SetActive(true);
		}

		public void OnStartButton()
		{
			onStartGameEvent?.RaiseEvent();
		}

		public void OnOptionsButton()
		{
			ShowPanel(optionsPanel);
		}

		public void OnCreditsButton()
		{
			ShowPanel(creditsPanel);
		}

		public void OnReturnToTitleButton()
		{
			ShowPanel(titlePanel);
		}

		public void OnQuitButton()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		private void ShowTitle()
		{
			ShowPanel(titlePanel);
		}
	}
}