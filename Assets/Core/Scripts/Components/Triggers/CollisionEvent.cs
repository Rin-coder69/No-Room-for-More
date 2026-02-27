using UnityEngine;
using UnityEngine.Events;

namespace CGL.Trigger
{
	public class CollisionEvent : CollisionBase
	{
		[SerializeField] UnityEvent<Collision> onEnter;
		[SerializeField] UnityEvent<Collision> onStay;
		[SerializeField] UnityEvent<Collision> onExit;

		protected override void OnEnter(Collision other) => onEnter?.Invoke(other);
		protected override void OnStay(Collision other) => onStay?.Invoke(other);
		protected override void OnExit(Collision other) => onExit?.Invoke(other);
	}
}
