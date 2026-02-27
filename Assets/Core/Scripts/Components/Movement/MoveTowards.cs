using UnityEngine;

using CGL.Data;
using CGL.Actor;

namespace CGL.Components
{
	public class MoveTowards : Activatable
	{
		[Space]
		[Header("Target")]
		[SerializeField]
		[Tooltip("Target transform to move towards. Overrides Target Data if assigned.")]
		private Transform target;

		[SerializeField]
		[Tooltip("Target game object data. Used if no direct target is assigned.")]
		private GameObjectDataSO targetData;

		[Header("Movement")]
		[SerializeField]
		[Range(0.1f, 20.0f)]
		[Tooltip("Speed the object moves towards the target.")]
		private float speed = 5.0f;

		[SerializeField]
		[Range(0.1f, 5.0f)]
		[Tooltip("Distance from target to stop moving.")]
		private float stoppingDistance = 0.1f;

		[SerializeField]
		[Range(1.0f, 30.0f)]
		[Tooltip("Speed the object rotates towards movement direction.")]
		private float rotationSpeed = 10.0f;

		[SerializeField]
		[Tooltip("Rotate the object towards the target.")]
		private bool rotateTowardsTarget = true;

		private Transform CurrentTarget => target != null ? target : targetData?.value.transform;

		public bool HasArrived { get; private set; } = false;

		public override void OnActivate()
		{
			base.OnActivate();
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
		}

		private void Update()
		{
			if (!IsActive || CurrentTarget == null) return;

			Vector3 direction = CurrentTarget.position - transform.position;
			float distance = direction.magnitude;

			// check if arrived at target
			HasArrived = distance <= stoppingDistance;
			if (HasArrived) return;

			// rotate towards target
			if (rotateTowardsTarget && direction != Vector3.zero)
			{
				Quaternion targetRotation = Quaternion.LookRotation(direction);
				transform.rotation = Quaternion.Slerp(transform.rotation,
					targetRotation, rotationSpeed * Time.deltaTime);
			}

			// move towards target
			transform.position = Vector3.MoveTowards(transform.position,
				CurrentTarget.position, speed * Time.deltaTime);
		}
	}
}