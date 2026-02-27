using UnityEngine;

namespace CGL.Trigger
{
	public abstract class CollisionBase : MonoBehaviour
	{
		[SerializeField] string filterTag;

		protected abstract void OnEnter(Collision other);
		protected virtual void OnStay(Collision other) { }
		protected abstract void OnExit(Collision other);

		void OnCollisionEnter(Collision other)
		{
			if (string.IsNullOrEmpty(filterTag) || other.gameObject.CompareTag(filterTag))
				OnEnter(other);
		}

		void OnCollisionStay(Collision other)
		{
			if (string.IsNullOrEmpty(filterTag) || other.gameObject.CompareTag(filterTag))
				OnStay(other);
		}

		void OnCollisionExit(Collision other)
		{
			if (string.IsNullOrEmpty(filterTag) || other.gameObject.CompareTag(filterTag))
				OnExit(other);
		}
	}
}