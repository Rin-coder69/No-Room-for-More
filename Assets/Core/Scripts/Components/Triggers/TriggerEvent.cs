using UnityEngine;
using UnityEngine.Events;

namespace CGL.Trigger
{
	public class TriggerEvent : TriggerBase
	{
		[SerializeField] UnityEvent onEnter;
		[SerializeField] UnityEvent onStay;
		[SerializeField] UnityEvent onExit;

		protected override void OnEnter(Collider other) => onEnter?.Invoke();
		protected override void OnStay(Collider other) => onStay?.Invoke();
		protected override void OnExit(Collider other) => onExit?.Invoke();
	}
}
