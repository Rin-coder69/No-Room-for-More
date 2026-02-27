using CGL.Events;
using UnityEngine;

namespace CGL.Pickup
{
	// base class for all pickup objects.
	// handles tag filtering and collection flow.
	// derive and implement OnCollect to define pickup behavior.
	[RequireComponent(typeof(Collider))]
	public abstract class Pickup : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Tag of the object that can pick this up.")]
		private string pickupTag = "Player";

		[SerializeField]
		[Tooltip("If true, the pickup is destroyed on collection. If false, it is deactivated.")]
		private bool destroyOnPickup = false;

		[SerializeField]
		[Tooltip("Optional effect or object to instantiate at the pickup position on collection.")]
		private GameObject collectEffect;

		[SerializeField]
		[Tooltip("Raised when this pickup is collected.")]
		private EventSO onPickupCollectedEvent;

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag(pickupTag)) return;
			if (!OnCollect(other)) return;

			onPickupCollectedEvent?.RaiseEvent();

			if (collectEffect != null)
				Instantiate(collectEffect, transform.position, transform.rotation);

			if (destroyOnPickup)
				Destroy(gameObject);
			else
				gameObject.SetActive(false);
		}

		// implement pickup behavior here — return true if collection was successful.
		// returning false leaves the pickup in the world.
		protected abstract bool OnCollect(Collider other);
	}
}