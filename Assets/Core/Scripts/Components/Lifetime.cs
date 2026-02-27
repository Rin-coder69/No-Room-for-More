using CGL.Actor;
using System.Collections;
using UnityEngine;

namespace CGL.Components
{
	// destroys or deactivates the GameObject after a randomized lifetime.
	// derives from Activatable — lifetime starts on OnActivate and cancels on OnDeactivate.
	public class Lifetime : Activatable
	{
		enum Action
		{
			Destroy,    // destroy the GameObject when lifetime expires
			Deactivate  // deactivate the GameObject when lifetime expires
		}

		[SerializeField]
		[Tooltip("Minimum lifetime in seconds.")]
		private float lifetimeMin = 1f;

		[SerializeField]
		[Tooltip("Maximum lifetime in seconds. Randomized between min and max on each activation.")]
		private float lifetimeMax = 1f;

		[SerializeField]
		[Tooltip("Whether to destroy or deactivate the object when lifetime expires.")]
		private Action action = Action.Destroy;

		private Coroutine lifetimeCoroutine;

		private void OnValidate()
		{
			if (lifetimeMax < lifetimeMin)
				lifetimeMax = lifetimeMin;
		}

		public override void OnActivate()
		{
			base.OnActivate();

			float lifetime = Random.Range(lifetimeMin, lifetimeMax);

			// always use coroutine so lifetime can be cancelled by OnDeactivate
			if (lifetimeCoroutine != null)
				StopCoroutine(lifetimeCoroutine);

			lifetimeCoroutine = StartCoroutine(LifetimeCR(lifetime));
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();

			if (lifetimeCoroutine != null)
			{
				StopCoroutine(lifetimeCoroutine);
				lifetimeCoroutine = null;
			}
		}

		private IEnumerator LifetimeCR(float lifetime)
		{
			yield return new WaitForSeconds(lifetime);

			if (action == Action.Destroy)
				Destroy(gameObject);
			else
				gameObject.SetActive(false);

			lifetimeCoroutine = null;
		}
	}
}