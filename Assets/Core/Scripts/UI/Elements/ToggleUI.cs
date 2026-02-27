using UnityEngine;
using UnityEngine.UI;
using CGL.Data;
using CGL.Events;

namespace CGL.UI
{
	public class ToggleUI : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Toggle component to initialize with data value.")]
		private Toggle toggle;

		[SerializeField]
		[Tooltip("Stores the current toggle state.")]
		private BoolDataSO toggleData;

		[SerializeField]
		[Tooltip("Raised when the toggle value changes.")]
		private BoolEventSO onValueChangedEvent;

		private void OnEnable()
		{
			if (toggle != null && toggleData != null) toggle.isOn = toggleData.value;
		}

		public void OnValueChanged(bool value)
		{
			if (toggleData != null) toggleData.value = value;
			onValueChangedEvent?.RaiseEvent(value);
		}
	}
}