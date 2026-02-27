using UnityEngine;
using CGL.Data;
using CGL.Events;

namespace CGL.Controller
{
	public abstract class CharacterControllerBase : Controller
	{
		[SerializeField]
		[Tooltip("View transform to transform input to view space.")]
		protected Transform view;

		[SerializeField]
		[Range(0.1f, 20.0f)]
		[Tooltip("Movement speed.")]
		protected float speed = 5.0f;

		[SerializeField]
		[Range(1.0f, 5.0f)]
		[Tooltip("Sprint speed multiplier.")]
		protected float sprintMultiplier = 1.5f;

		[SerializeField]
		[Range(0.1f, 20.0f)]
		[Tooltip("Jump force.")]
		protected float jumpForce = 5.0f;

		[Header("Input Data")]
		[SerializeField]
		[Tooltip("Movement input data.")]
		protected Vector2DataSO moveData;

		[SerializeField]
		[Tooltip("Sprint input data.")]
		protected BoolDataSO sprintData;

		[Header("Events")]
		[SerializeField]
		[Tooltip("Subscribe to this event to trigger jump.")]
		protected EventSO onJumpEvent;

		[SerializeField]
		[Tooltip("Gravity applied when rising.")]
		[Range(0.1f, 30.0f)]
		protected float gravity = 9.81f;

		[SerializeField]
		[Tooltip("Gravity applied when falling. Higher value = faster fall.")]
		[Range(0.1f, 60.0f)]
		protected float fallGravity = 20.0f;

		[SerializeField]
		[Tooltip("Layer mask to check if grounded.")]
		protected LayerMask groundLayerMask = Physics.AllLayers;

		[SerializeField]
		[Range(0.1f, 2.0f)]
		[Tooltip("Distance to check below for ground.")]
		protected float groundCheckDistance = 1.2f;

		protected Vector3 velocity;
		protected bool isGrounded = false;

		// properties used by ActorAnimatorController to drive animator parameters
		public bool IsGrounded => isGrounded;
		public bool IsFalling => !isGrounded && velocity.y < 0;
		public bool IsSprinting => sprintData != null && sprintData.value;
		public Vector2 MoveInput => moveData != null ? moveData.value : Vector2.zero;
		public virtual Vector3 Velocity => velocity;

		public float Speed
		{
			get => speed;
			set => speed = value;
		}

		protected virtual void Awake()
		{
			if (view == null) view = Camera.main.transform;
		}

		private void Start()
		{
			if (view == null) view = Camera.main.transform;
		}

		protected abstract void Move();
	}
}