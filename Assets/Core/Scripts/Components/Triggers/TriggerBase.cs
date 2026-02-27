using UnityEngine;

namespace CGL.Trigger
{
	public abstract class TriggerBase : MonoBehaviour
	{
		[SerializeField] string filterTag = "Player";

		protected abstract void OnEnter(Collider other);
		protected virtual void OnStay(Collider other) { }
		protected abstract void OnExit(Collider other);

		void OnTriggerEnter(Collider other)
		{
			if (string.IsNullOrEmpty(filterTag) || other.CompareTag(filterTag))
				OnEnter(other);
		}

		void OnTriggerExit(Collider other)
		{
			if (string.IsNullOrEmpty(filterTag) || other.CompareTag(filterTag))
				OnExit(other);
		}

		void OnTriggerStay(Collider other)
		{
			if (string.IsNullOrEmpty(filterTag) || other.CompareTag(filterTag))
				OnStay(other);
		}
	}
}
