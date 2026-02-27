using UnityEngine;
using CGL.Events;

namespace CGL.Inventory
{
	// world object that adds an item to the collector's inventory on contact.
	[RequireComponent(typeof(Collider))]
	public class ItemPickup : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Item data this pickup represents.")]
		private ItemData itemData;

		[SerializeField]
		[Tooltip("Tag of the object that can pick this up.")]
		private string pickupTag = "Player";

		[SerializeField]
		[Tooltip("Raised when this pickup is collected.")]
		private EventSO onPickupCollectedEvent;

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag(pickupTag)) return;

			Inventory inventory = other.GetComponent<Inventory>();
			if (inventory == null) return;

			// let Inventory handle instantiation and attachment
			if (inventory.AddItem(itemData))
			{
				onPickupCollectedEvent?.RaiseEvent();
				gameObject.SetActive(false);
			}
		}
	}
}