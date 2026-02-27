using UnityEngine;
using CGL.Data;
using CGL.Events;


namespace CGL.Controller
{
	[RequireComponent(typeof(Rigidbody))]
	public class PhysicsCharacterController : CharacterControllerBase
	{
		[SerializeField]
		[Tooltip("Force mode used to move the rigidbody.")]
		private ForceMode forceMode = ForceMode.Force;

		private Rigidbody rb;

		protected override void Awake()
		{
			base.Awake();
			rb = GetComponent<Rigidbody>();
		}

		private void OnEnable()
		{
			onJumpEvent?.Subscribe(OnJump);
		}

		private void OnDisable()
		{
			onJumpEvent?.Unsubscribe(OnJump);
		}

		private void FixedUpdate()
		{
			CheckGrounded();
			Move();
		}

		protected override void Move()
		{
			if (moveData == null) return;

			// convert input to view space
			Vector3 input = new Vector3(moveData.value.x, 0, moveData.value.y);
			Vector3 direction = view != null ? view.TransformDirection(input) : input;
			direction.y = 0;

			// apply sprint multiplier if sprinting
			float currentSpeed = (sprintData != null && sprintData.value) ?
				speed * sprintMultiplier : speed;

			rb.AddForce(direction.normalized * currentSpeed, forceMode);
		}

		private void CheckGrounded()
		{
			isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayerMask);
		}

		private void OnJump()
		{
			if (!isGrounded) return;
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}
}
