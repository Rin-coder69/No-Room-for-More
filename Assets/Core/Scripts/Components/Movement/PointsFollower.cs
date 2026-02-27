using UnityEngine;

using CGL.Actor;

namespace CGL.Components
{
	public class PointsFollower : Activatable
	{
		public enum MoveMode
		{
			Forward,    // move forward through points then stop
			Reverse,    // move backward through points then stop
			Loop,       // loop back to start when end is reached
			PingPong    // reverse direction when end or start is reached
		}

		[Space]

		[SerializeField]
		[Tooltip("Array of points to follow.")]
		private Transform[] points;

		[SerializeField]
		[Range(0.1f, 20.0f)]
		[Tooltip("Speed the object moves between points.")]
		private float speed = 5.0f;

		[SerializeField]
		[Range(0.01f, 1.0f)]
		[Tooltip("Distance from point to consider it reached.")]
		private float arrivalDistance = 0.1f;

		[SerializeField]
		[Tooltip("How the object moves through the points.")]
		private MoveMode moveMode = MoveMode.Loop;

		[SerializeField]
		[Tooltip("Rotate the object to face the next point.")]
		private bool faceDirection = true;

		private int currentIndex = 0;
		private int direction = 1;

		private bool isComplete = false;
		public bool IsComplete => isComplete;

		protected override void Start()
		{
			base.Start();

			// set starting index based on move mode
			currentIndex = (moveMode == MoveMode.Reverse) ? points.Length - 1 : 0;
			direction = (moveMode == MoveMode.Reverse) ? -1 : 1;

			// snap to first point
			transform.position = points[currentIndex].position;
		}

		private void Update()
		{
			if (!IsActive || isComplete || points == null) return;

			Transform targetPoint = points[currentIndex];

			// move towards current target point
			transform.position = Vector3.MoveTowards(transform.position,
				targetPoint.position, speed * Time.deltaTime);

			// rotate to face direction of travel
			if (faceDirection)
			{
				Vector3 moveDirection = targetPoint.position - transform.position;
				if (moveDirection != Vector3.zero)
				{
					transform.rotation = Quaternion.LookRotation(moveDirection);
				}
			}

			// check if arrived at current point
			if (Vector3.Distance(transform.position, targetPoint.position) <= arrivalDistance)
			{
				AdvanceToNextPoint();
			}
		}

		private void AdvanceToNextPoint()
		{
			int nextIndex = currentIndex + direction;

			switch (moveMode)
			{
				case MoveMode.Forward:
				case MoveMode.Reverse:
					// stop at end
					if (nextIndex < 0 || nextIndex >= points.Length)
					{
						isComplete = true;
						return;
					}
					break;

				case MoveMode.Loop:
					// wrap around to start
					nextIndex = nextIndex % points.Length;
					if (nextIndex < 0) nextIndex += points.Length;
					break;

				case MoveMode.PingPong:
					// reverse direction at ends
					if (nextIndex < 0 || nextIndex >= points.Length)
					{
						direction *= -1;
						nextIndex = currentIndex + direction;
					}
					break;
			}

			currentIndex = nextIndex;
		}
	}
}