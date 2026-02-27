using UnityEngine;
using UnityEngine.UI;
using CGL.Data;
using CGL.Events;

namespace CGL.UI
{
	public class SliderUI : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Slider component to initialize with data value.")]
		Slider slider;

		[SerializeField]
		[Tooltip("Float data to update with slider.")]
		FloatDataSO sliderData;

		[SerializeField]
		[Tooltip("Raised when the slider value changes.")]
		EventSO onValueChangedEvent;

		private void OnEnable()
		{
			if (slider == null || sliderData == null) return;
			slider.value = sliderData.value;
		}

		public void OnValueChanged(float value)
		{
			if (sliderData == null) return;
			sliderData.value = value;
			onValueChangedEvent?.RaiseEvent();
		}
	}
}