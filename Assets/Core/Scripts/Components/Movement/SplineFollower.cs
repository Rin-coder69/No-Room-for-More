using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

using CGL.Actor;

namespace CGL.Components
{
	public class SplineFollower : Activatable
	{
		[Space]

		[SerializeField]
		[Tooltip("Spline container to follow.")]
		SplineContainer splineContainer;

		[SerializeField, Range(0, 40)]
		[Tooltip("Speed the object moves along the spline.")]
		float speed = 1;

		[SerializeField]
		[Tooltip("Reverse the direction of movement.")]
		bool reverse = false;

		// debug use - normalized distance along spline (0-1)
		[SerializeField, Range(0, 1)] float tdistance = 0;

		// the speed that the game object moves at
		public float Speed { get { return speed; } set { speed = value; } }

		// length in world coordinates
		public float Length => cachedLength;

		// distance in world coordinates
		public float Distance
		{
			get { return tdistance * Length; }
			set { tdistance = value / Length; }
		}

		float cachedLength;

		protected override void OnEnable()
		{
			base.OnEnable();
			cachedLength = splineContainer.CalculateLength();
		}

		void Update()
		{
			if (!IsActive) return;

			Distance += speed * Time.deltaTime * (reverse ? -1.0f : 1.0f);
			UpdatePositionAndRotation(math.frac(tdistance));
		}

		void UpdatePositionAndRotation(float t)
		{
			Vector3 position = splineContainer.EvaluatePosition(t);
			Vector3 up = splineContainer.EvaluateUpVector(t);
			Vector3 forward = Vector3.Normalize(splineContainer.EvaluateTangent(t));
			transform.position = position;
			transform.rotation = Quaternion.LookRotation(forward, up);
		}
	}
}