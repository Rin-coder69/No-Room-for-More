using UnityEngine;
using UnityEngine.Events;

namespace CGL.Events
{
	[CreateAssetMenu(fileName = "Event", menuName = "CGL/Events/Basic/Event")]
	public class EventSO : BaseSO
	{
		private UnityAction listeners;

		public void Subscribe(UnityAction listener) => listeners += listener;
		public void Unsubscribe(UnityAction listener) => listeners -= listener;
		public void RaiseEvent() => listeners?.Invoke();
	}
}
