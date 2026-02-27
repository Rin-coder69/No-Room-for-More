using CGL.Data;
using CGL.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Analytics.IAnalytic;

namespace CGL.UI
{
	public class ProgressBarUI : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Slider component to use as the progress bar.")]
		private Slider slider;

		[SerializeField]
		[Tooltip("Float data to display as progress. Value should be between 0 and 1.")]
		private FloatDataSO progressData;

		[SerializeField]
		[Range(0.0f, 10.0f)]
		[Tooltip("Speed the progress bar smoothly moves to the target value. 0 = instant.")]
		private float smoothSpeed = 0.0f;

		[SerializeField]
		[Tooltip("Raised when the progress bar should update.")]
		private EventSO onUpdateEvent;

		private float targetValue = 0.0f;

		private void OnEnable()
		{
			onUpdateEvent?.Subscribe(OnUpdate);
			OnUpdate();
		}

		private void OnDisable()
		{
			onUpdateEvent?.Unsubscribe(OnUpdate);
		}

		private void Update()
		{
			if (slider == null || smoothSpeed == 0) return;

			// smoothly move slider towards target value
			slider.value = Mathf.MoveTowards(slider.value, targetValue,	smoothSpeed * Time.deltaTime);
		}

		public void OnUpdate()
		{
			if (slider == null || progressData == null) return;

			targetValue = Mathf.Clamp01(progressData.value);
			// set immediately if no smoothing
			if (smoothSpeed == 0) slider.value = targetValue;
		}
	}
}
