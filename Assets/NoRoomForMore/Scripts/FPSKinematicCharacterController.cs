using UnityEngine;

namespace CGL.Controller
{
	[RequireComponent(typeof(CharacterController))]
	public class FPSKinematicCharacterController : CharacterControllerBase
	{
		//[SerializeField]
		//[Tooltip("Speed the character rotates towards movement direction.")]
		//[Range(1.0f, 30.0f)]
		//private float rotateTowardsSpeed = 10.0f;

		[SerializeField]
		[Tooltip("Force applied to physics objects when pushed.")]
		[Range(0.1f, 20.0f)]
		private float pushForce = 5.0f;

		[SerializeField]
		[Tooltip("Enable pushing of physics objects.")]
		private bool canPush = true;

		protected CharacterController characterController;
		private Vector3 moveDirection = Vector3.zero;

		public override Vector3 Velocity => (characterController != null) ? characterController.velocity : Vector3.zero;

		protected override void Awake()
		{
			base.Awake();
			characterController = GetComponent<CharacterController>();
		}

		private void OnEnable()
		{
			onJumpEvent?.Subscribe(OnJump);
		}

		private void OnDisable()
		{
			onJumpEvent?.Unsubscribe(OnJump);
		}

		protected virtual void Update()
		{
            CheckGrounded();
			ApplyGravity();
			Move();
		}

		protected void CheckGrounded()
		{
			isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayerMask);

			// clear vertical velocity when grounded
			if (isGrounded && velocity.y < 0) velocity.y = -2.0f;
		}

		protected override void Move()
		{
			if (moveData == null) return;

			// convert input to view space
			Vector3 input = new Vector3(moveData.value.x, 0, moveData.value.y);
			Vector3 direction = view != null ? view.TransformDirection(input) : input;
			direction.y = 0.0f;

			if (direction.magnitude > 0.1f)
				moveDirection = direction.normalized;

			float currentSpeed = (sprintData != null && sprintData.value) ?
				speed * sprintMultiplier : speed;

			// RotateTowardsMovement();

			// combine horizontal and vertical into one Move call
			// so characterController.velocity reflects full XZ + Y displacement
			Vector3 move = direction.normalized * currentSpeed + velocity;
			characterController.Move(move * Time.deltaTime);
		}

		//private void RotateTowardsMovement()
		//{
		//	if (moveDirection == Vector3.zero) return;

		//	// smoothly rotate towards movement direction
		//	Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
		//	transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateTowardsSpeed * Time.deltaTime);
		//}

		protected void ApplyGravity()
		{
			float currentGravity = velocity.y < 0 ? fallGravity : gravity;
			velocity.y -= currentGravity * Time.deltaTime;
		}

		private void OnJump()
		{
			if (!isGrounded) return;
			velocity.y = Mathf.Sqrt(jumpForce * 2.0f * gravity);
		}

		// called by Unity when the character controller hits a collider
		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (!canPush) return;

			Rigidbody rb = hit.collider.attachedRigidbody;

			// only push non-kinematic rigidbodies
			if (rb == null || rb.isKinematic) return;

			// only push objects to the side, not below or above
			if (hit.moveDirection.y < -0.3f) return;

			// apply force in the direction of movement
			Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
			rb.AddForce(pushDirection * pushForce, ForceMode.Force);
		}
	}
}