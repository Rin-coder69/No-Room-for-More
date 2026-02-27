using UnityEngine;

namespace CGL.Controller
{
	public class RotationCharacterController : KinematicCharacterController
	{
		[SerializeField]
		[Tooltip("Rotation speed in degrees per second.")]
		[Range(0.1f, 360.0f)]
		private float rotationSpeed = 180.0f;

		protected override void Update()
		{
			CheckGrounded();
			ApplyGravity();
			Rotate();
			Move();
		}

		private void Rotate()
		{
			if (moveData == null) return;

			// x input rotates left and right
			float rotation = moveData.value.x * rotationSpeed * Time.deltaTime;
			transform.Rotate(0, rotation, 0);
		}

		protected override void Move()
		{
			if (moveData == null) return;

			float currentSpeed = (sprintData != null && sprintData.value) ?	speed * sprintMultiplier : speed;

			// combine forward movement with vertical velocity in one Move call
			Vector3 move = transform.forward * moveData.value.y * currentSpeed + velocity;
			characterController.Move(move * Time.deltaTime);
		}
	}
}