using CGL.Data;
using CGL.Events;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CGL.UI
{
	public class CounterUI : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Text component to display the counter value.")]
		private TMP_Text text;

		[SerializeField]
		[Tooltip("Integer data to display in the text component.")]
		private IntDataSO intData;

		[SerializeField]
		[Tooltip("Float data to display in the text component. Ignored if Int Data is assigned.")]
		private FloatDataSO floatData;

		[SerializeField]
		[Tooltip("Format string for display. Use {0} as placeholder e.g. 'Score: {0:0000}'.")]
		private string format = "{0}";

		[SerializeField]
		[Tooltip("Raised when the counter should update.")]
		private EventSO onUpdateEvent;

		private void OnEnable()
		{
			onUpdateEvent?.Subscribe(OnUpdate);
			OnUpdate();
		}

		private void OnDisable()
		{
			onUpdateEvent?.Unsubscribe(OnUpdate);
		}

		public void OnUpdate()
		{
			if (text == null) return;

			// int data takes priority over float data
			if (intData != null)
			{
				text.text = string.IsNullOrEmpty(format) ?
					intData.value.ToString() : string.Format(format, intData.value);
			}
			else if (floatData != null)
			{
				text.text = string.IsNullOrEmpty(format) ?
					floatData.value.ToString() : string.Format(format, floatData.value);
			}
		}
	}
}

//**Format string examples specific to numbers:**
//{ 0}         -> "42"
//{ 0:0000}    -> "0042"        // score display
//{ 0:F1}      -> "3.5"         // one decimal
//{ 0:F0}      -> "4"           // rounded float
//{ 0:P0}      -> "85%"         // percentage from 0-1 value
//Score: { 0}  -> "Score: 42"
//{ 0} / 10    -> "7 / 10"      // ammo counter
//x{0}         -> "x3"          // lives counter