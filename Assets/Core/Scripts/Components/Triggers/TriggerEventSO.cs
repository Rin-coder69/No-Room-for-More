using UnityEngine;
using CGL.Events;

namespace CGL.Trigger
{
	public class TriggerEventSO : TriggerBase
	{
		[SerializeField] EventSO onEnter;
		[SerializeField] EventSO onStay;
		[SerializeField] EventSO onExit;

		protected override void OnEnter(Collider other) => onEnter?.RaiseEvent();
		protected override void OnStay(Collider other) => onStay?.RaiseEvent();
		protected override void OnExit(Collider other) => onExit?.RaiseEvent();
	}
}
