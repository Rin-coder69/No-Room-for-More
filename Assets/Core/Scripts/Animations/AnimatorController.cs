using UnityEngine;

namespace CGL.Animation
{
	// generic animator controller that exposes animator parameters as public methods.
	// wire any UnityEvent to these methods to drive animations without a custom component.
	// for example: TriggerEvent.onEnter -> AnimatorController.SetTrigger("Open")
	[RequireComponent(typeof(Animator))]
	public class AnimatorController : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Parameter name used by SetFloat and SetInt when called from a UnityEvent with only a value argument.")]
		private string parameterName;

		private Animator animator;

		public Animator Animator => animator;

		protected virtual void Awake()
		{
			animator = GetComponent<Animator>();
		}

		// --- Trigger ---

		// set a trigger parameter by name — wire directly from any UnityEvent<string>
		public void SetTrigger(string name)
		{
			animator?.SetTrigger(Animator.StringToHash(name));
		}

		// --- Bool ---

		// set a bool parameter to true by name — wire from onEnter, onActivate, etc.
		public void SetBoolTrue(string name)
		{
			animator?.SetBool(Animator.StringToHash(name), true);
		}

		// set a bool parameter to false by name — wire from onExit, onDeactivate, etc.
		public void SetBoolFalse(string name)
		{
			animator?.SetBool(Animator.StringToHash(name), false);
		}

		// toggle a bool parameter by name
		public void SetBoolToggle(string name)
		{
			if (animator == null) return;
			int hash = Animator.StringToHash(name);
			animator.SetBool(hash, !animator.GetBool(hash));
		}

		// set a bool by name and value — useful when both are available from code
		public void SetBool(string name, bool value)
		{
			animator?.SetBool(Animator.StringToHash(name), value);
		}

		// --- Float ---

		// set a float parameter by value using parameterName field — wire from UnityEvent<float>
		public void SetFloat(float value)
		{
			if (string.IsNullOrEmpty(parameterName)) return;
			animator?.SetFloat(Animator.StringToHash(parameterName), value);
		}

		// set a float parameter by name and value — useful when both are available from code
		public void SetFloat(string name, float value)
		{
			animator?.SetFloat(Animator.StringToHash(name), value);
		}

		// --- Int ---

		// set an int parameter by value using parameterName field — wire from UnityEvent<int>
		public void SetInt(int value)
		{
			if (string.IsNullOrEmpty(parameterName)) return;
			animator?.SetInteger(Animator.StringToHash(parameterName), value);
		}

		// set an int parameter by name and value — useful when both are available from code
		public void SetInt(string name, int value)
		{
			animator?.SetInteger(Animator.StringToHash(name), value);
		}
	}
}