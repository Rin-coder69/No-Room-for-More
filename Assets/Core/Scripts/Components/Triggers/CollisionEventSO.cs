using UnityEngine;
using CGL.Events;

namespace CGL.Trigger
{
	public class CollisionEventSO : CollisionBase
	{
		[SerializeField] EventSO onEnter;
		[SerializeField] EventSO onStay;
		[SerializeField] EventSO onExit;

		protected override void OnEnter(Collision other) => onEnter?.RaiseEvent();
		protected override void OnStay(Collision other) => onStay?.RaiseEvent();
		protected override void OnExit(Collision other) => onExit?.RaiseEvent();
	}
}
