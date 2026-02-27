using TMPro;
using UnityEngine;

using CGL.Data;
using CGL.Events;

namespace CGL.UI
{
	public class TextUI : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Text component to display the string value.")]
		private TMP_Text text;

		[SerializeField]
		[Tooltip("String data to display in the text component.")]
		private StringDataSO textData;

		[SerializeField]
		[Tooltip("Optional format string. Use {0} as placeholder e.g. 'Score: {0}'.")]
		private string format = "{0}";

		[SerializeField]
		[Tooltip("Raised when the text should update.")]
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
			if (text == null || textData == null) return;
			text.text = string.IsNullOrEmpty(format) ? textData.value : string.Format(format, textData.value);
		}
	}
}