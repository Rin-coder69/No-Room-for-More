using UnityEngine;

using CGL.Actor;

namespace CGL.Components
{
	public class Rotator : Activatable
	{
		[Space]

		[SerializeField]
		[Tooltip("Rotation speed in degrees per second (X, Y, Z)")]
		private Vector3 rotationSpeed;

		[SerializeField]
		[Tooltip("Rotation space (Self = local, World = global)")]
		Space rotationSpace = Space.Self;

		void Update()
		{
			if (!IsActive) return;

			transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpace);
		}
	}
}
