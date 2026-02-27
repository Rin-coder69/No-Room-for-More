using UnityEngine;
using UnityEngine.InputSystem;

using CGL.DesignPatterns;
using CGL.Data;
using CGL.Events;

namespace CGL.Input
{
	public class InputReader : Singleton<InputReader>
	{
		[Header("Movement Data")]
		[SerializeField]
		[Tooltip("Stores the current movement input value.")]
		Vector2DataSO moveData;

		[SerializeField]
		[Tooltip("Stores the current look input value.")]
		Vector2DataSO lookData;

		[Header("Action Data")]
		[SerializeField]
		[Tooltip("Stores whether sprint is active.")]
		BoolDataSO sprintData;

		[SerializeField]
		[Tooltip("Stores whether attack is active.")]
		BoolDataSO attackData;

		[Header("Events")]
		[SerializeField]
		[Tooltip("Raised when jump is pressed.")]
		EventSO onJumpEvent;

		[SerializeField]
		[Tooltip("Raised when attack begins.")]
		EventSO onAttackBeginEvent;

		[SerializeField]
		[Tooltip("Raised when attack ends.")]
		EventSO onAttackEndEvent;

		[SerializeField]
		[Tooltip("Raised when next is pressed.")]
		EventSO onNextEvent;

		[SerializeField]
		[Tooltip("Raised when previous is pressed.")]
		EventSO onPreviousEvent;

		[SerializeField]
		[Tooltip("Raised when pause is pressed.")]
		EventSO onPauseEvent;

		// input actions
		private InputAction moveAction;
		private InputAction lookAction;
		private InputAction sprintAction;
		private InputAction jumpAction;
		private InputAction attackAction;
		private InputAction nextAction;
		private InputAction previousAction;
		private InputAction pauseAction;

		public override void Awake()
		{
			base.Awake();

			// find actions from project-wide input asset
			moveAction = InputSystem.actions.FindAction("Move");
			lookAction = InputSystem.actions.FindAction("Look");
			sprintAction = InputSystem.actions.FindAction("Sprint");
			jumpAction = InputSystem.actions.FindAction("Jump");
			attackAction = InputSystem.actions.FindAction("Attack");
			nextAction = InputSystem.actions.FindAction("Next");
			previousAction = InputSystem.actions.FindAction("Previous");
			pauseAction = InputSystem.actions.FindAction("Pause");
		}

		private void OnEnable()
		{
			moveAction.performed += OnMove;
			moveAction.canceled += OnMove;

			lookAction.performed += OnLook;
			lookAction.canceled += OnLook;

			sprintAction.performed += OnSprintPerformed;
			sprintAction.canceled += OnSprintCanceled;

			jumpAction.performed += OnJump;

			attackAction.performed += OnAttackBegin;
			attackAction.canceled += OnAttackEnd;

			nextAction.performed += OnNext;
			previousAction.performed += OnPrevious;

			pauseAction.performed += OnPause;
		}

		private void OnDisable()
		{
			moveAction.performed -= OnMove;
			moveAction.canceled -= OnMove;

			lookAction.performed -= OnLook;
			lookAction.canceled -= OnLook;

			sprintAction.performed -= OnSprintPerformed;
			sprintAction.canceled -= OnSprintCanceled;

			jumpAction.performed -= OnJump;

			attackAction.performed -= OnAttackBegin;
			attackAction.canceled -= OnAttackEnd;

			nextAction.performed -= OnNext;
			previousAction.performed -= OnPrevious;

			pauseAction.performed -= OnPause;
		}

		void OnMove(InputAction.CallbackContext ctx)
		{
			if (moveData != null) moveData.value = ctx.ReadValue<Vector2>();
		}

		void OnLook(InputAction.CallbackContext ctx)
		{
			if (lookData != null) lookData.value = ctx.ReadValue<Vector2>();
		}

		void OnSprintPerformed(InputAction.CallbackContext ctx)
		{
			if (sprintData != null) sprintData.value = true;
		}

		void OnSprintCanceled(InputAction.CallbackContext ctx)
		{
			if (sprintData != null) sprintData.value = false;
		}

		void OnJump(InputAction.CallbackContext ctx)
		{
			onJumpEvent?.RaiseEvent();
		}

		void OnAttackBegin(InputAction.CallbackContext ctx)
		{
			if (attackData != null) attackData.value = true;
			onAttackBeginEvent?.RaiseEvent();
		}

		void OnAttackEnd(InputAction.CallbackContext ctx)
		{
			if (attackData != null) attackData.value = false;
			onAttackEndEvent?.RaiseEvent();
		}

		void OnNext(InputAction.CallbackContext ctx)
		{
			onNextEvent?.RaiseEvent();
		}

		void OnPrevious(InputAction.CallbackContext ctx)
		{
			onPreviousEvent?.RaiseEvent();
		}

		void OnPause(InputAction.CallbackContext ctx)
		{
			onPauseEvent?.RaiseEvent();
		}
	}
}