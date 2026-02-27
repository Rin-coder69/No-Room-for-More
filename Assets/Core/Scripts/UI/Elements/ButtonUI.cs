using CGL.Events;
using UnityEngine;

namespace CGL.UI
{
	public class ButtonUI : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Raised when the button is clicked.")]
		EventSO onClickEvent;

		[SerializeField]
		[Tooltip("Raised when the button is clicked, passing the click value as a string.")]
		StringEventSO onClickStringEvent;

		public void OnClick()
		{
			onClickEvent?.RaiseEvent();
		}

		public void OnClick(string str)
		{
			onClickStringEvent?.RaiseEvent(str);
		}
	}
}