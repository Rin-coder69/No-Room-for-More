using UnityEngine;
using UnityEngine.Events;

namespace CGL.Events
{
	public class BaseEventSO<T> : BaseSO
	{
		private UnityAction<T> listeners;

		public void Subscribe(UnityAction<T> listener) => listeners += listener;
		public void Unsubscribe(UnityAction<T> listener) => listeners -= listener;
		public void RaiseEvent(T value) => listeners?.Invoke(value);
	}
}
