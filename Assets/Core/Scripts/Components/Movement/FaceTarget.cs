using CGL.Actor;
using CGL.Data;
using UnityEngine;

namespace CGL.Components
{
	public class FaceTarget : Activatable
	{
		public enum FaceMode
		{
			Full,       // rotate on all axes to face target
			YAxisOnly   // rotate only on Y axis, good for upright objects
		}

		[Space]

		[SerializeField]
		[Tooltip("Transform to face towards. Ignored if Face Camera is enabled.")]
		private Transform target;

		[SerializeField]
		[Tooltip("Target game object data. Used if no direct target is assigned.")]
		private GameObjectDataSO targetData;

		[SerializeField]
		[Tooltip("If true, use the main camera as the target.")]
		private bool faceCamera = false;

		[SerializeField]
		[Tooltip("Full = face target on all axes. YAxisOnly = rotate only on Y axis.")]
		private FaceMode mode = FaceMode.Full;

		private Transform CurrentTarget => target != null ? target : targetData?.value.transform;

		private void Awake()
		{
			if (faceCamera)	target = Camera.main.transform;
		}

		private void LateUpdate()
		{
			if (!IsActive || CurrentTarget == null) return;

			Vector3 direction = CurrentTarget.position - transform.position;
			if (direction == Vector3.zero) return;

			switch (mode)
			{
				case FaceMode.Full:
					// face towards target position on all axes
					transform.rotation = Quaternion.LookRotation(direction);
					break;

				case FaceMode.YAxisOnly:
					// only rotate on Y axis, keep upright
					direction.y = 0;
					if (direction != Vector3.zero) transform.rotation = Quaternion.LookRotation(direction);
					break;
			}
		}
	}
}