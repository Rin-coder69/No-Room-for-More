using System.Collections.Generic;
using UnityEngine;
using CGL.Events;

namespace CGL.Inventory
{
	// manages the player's inventory of items, handling equipping, switching, and usage.
	public class Inventory : MonoBehaviour
	{
		[Header("Starting Items")]
		[SerializeField]
		[Tooltip("Item data assets to instantiate at startup.")]
		private List<ItemData> startingItems = new List<ItemData>();

		[Header("Events")]
		[SerializeField]
		[Tooltip("Subscribe to this event to switch to the next item.")]
		private EventSO onNextEvent;

		[SerializeField]
		[Tooltip("Subscribe to this event to switch to the previous item.")]
		private EventSO onPreviousEvent;

		[SerializeField]
		[Tooltip("Subscribe to this event to use the current item.")]
		private EventSO onUseEvent;

		[SerializeField]
		[Tooltip("Subscribe to this event to stop using the current item.")]
		private EventSO onStopUseEvent;

		[SerializeField]
		[Tooltip("Raised when the current item changes, passes the new item.")]
		private ItemEventSO onItemChangedEvent;

		[SerializeField]
		[Tooltip("Raised when an item is added to the inventory.")]
		private EventSO onItemAddedEvent;

		[SerializeField]
		[Tooltip("Raised when an item is removed from the inventory.")]
		private EventSO onItemRemovedEvent;

		// runtime list of items in the inventory
		private List<Item> items = new List<Item>();

		public Item CurrentItem { get; private set; }
		public int ItemCount => items.Count;

		private int currentItemIndex = -1;

		private void OnEnable()
		{
			onNextEvent?.Subscribe(NextItem);
			onPreviousEvent?.Subscribe(PreviousItem);
			onUseEvent?.Subscribe(Use);
			onStopUseEvent?.Subscribe(StopUse);
		}

		private void OnDisable()
		{
			onNextEvent?.Unsubscribe(NextItem);
			onPreviousEvent?.Unsubscribe(PreviousItem);
			onUseEvent?.Unsubscribe(Use);
			onStopUseEvent?.Unsubscribe(StopUse);
		}

		private void Start()
		{
			InstantiateStartingItems();
		}

		// instantiates starting items from item data assets
		private void InstantiateStartingItems()
		{
			foreach (ItemData data in startingItems)
			{
				if (data == null || data.itemPrefab == null)
				{
					Debug.LogWarning($"Inventory: starting item data {data?.id} or prefab is null.", this);
					continue;
				}

				Transform attachPoint = FindAttachPoint(data.attachBone);

				// deactivate before GetComponent so item Start/OnEnable don't fire until equipped
				GameObject go = Instantiate(data.itemPrefab, attachPoint);
				go.SetActive(false);

				Item item = go.GetComponent<Item>();

				if (item == null)
				{
					Debug.LogWarning($"Inventory: prefab {data.itemPrefab.name} has no Item component.", this);
					Destroy(go);
					continue;
				}

				// add without equipping — equip first item after all are added
				items.Add(item);
			}

			// equip first item if any were added
			if (items.Count > 0) SwitchItem(0);
		}

		// activates the currently equipped item
		public void Use()
		{
			CurrentItem?.Use();
		}

		// stops usage of the currently equipped item
		public void StopUse()
		{
			CurrentItem?.StopUse();
		}

		// switches to the item at the specified index
		public bool SwitchItem(int index)
		{
			if (index < 0 || index >= items.Count) return false;
			if (index == currentItemIndex) return true;

			// unequip current item
			CurrentItem?.Unequip();

			// equip new item
			currentItemIndex = index;
			CurrentItem = items[currentItemIndex];
			CurrentItem?.Equip();

			// pass current item so listeners know what was equipped
			onItemChangedEvent?.RaiseEvent(CurrentItem);

			return true;
		}

		// switches to the next item in the inventory
		public void NextItem()
		{
			if (items.Count == 0) return;
			SwitchItem((currentItemIndex + 1) % items.Count);
		}

		// switches to the previous item in the inventory
		public void PreviousItem()
		{
			if (items.Count == 0) return;
			SwitchItem((currentItemIndex - 1 + items.Count) % items.Count);
		}

		// adds an item component to the inventory
		public bool AddItem(Item item)
		{
			if (item == null) return false;

			items.Add(item);
			onItemAddedEvent?.RaiseEvent();

			// auto equip if only item
			if (items.Count == 1) SwitchItem(0);

			return true;
		}

		// adds an item from item data, instantiating the prefab and attaching to the correct bone
		public bool AddItem(ItemData data)
		{
			if (data == null || data.itemPrefab == null) return false;

			// reject if not stackable and already in inventory
			if (!data.allowMultiple && HasItem(data.id)) return false;

			Transform attachPoint = FindAttachPoint(data.attachBone);

			// deactivate before GetComponent so item Start/OnEnable don't fire until equipped
			GameObject go = Instantiate(data.itemPrefab, attachPoint);
			go.SetActive(false);

			Item item = go.GetComponent<Item>();

			if (item == null)
			{
				Destroy(go);
				return false;
			}

			return AddItem(item);
		}

		// removes an item from the inventory
		public bool RemoveItem(Item item)
		{
			if (item == null) return false;

			int index = items.IndexOf(item);
			if (index == -1) return false;

			// unequip if removing current item
			if (item == CurrentItem)
			{
				CurrentItem.Unequip();
				CurrentItem = null;
				currentItemIndex = -1;
			}

			items.RemoveAt(index);
			onItemRemovedEvent?.RaiseEvent();

			// equip another item if available
			if (currentItemIndex == -1 && items.Count > 0)
			{
				SwitchItem(0);
			}
			else if (index < currentItemIndex)
			{
				// adjust index if removed item was before current
				currentItemIndex--;
			}

			return true;
		}

		// drops an item into the world and removes it from inventory
		public bool DropItem(Item item)
		{
			if (item == null) return false;
			if (!items.Contains(item)) return false;

			// spawn pickup prefab in world at current position
			ItemData data = item.GetData();
			if (data?.pickupPrefab != null)
			{
				Instantiate(data.pickupPrefab, transform.position, transform.rotation);
			}

			RemoveItem(item);
			Destroy(item.gameObject);

			return true;
		}

		// gets the item at the specified index
		public Item GetItem(int index)
		{
			if (index < 0 || index >= items.Count) return null;
			return items[index];
		}

		// checks if inventory contains an item with the given id
		public bool HasItem(string id)
		{
			return items.Exists(item => item.GetData()?.id == id);
		}

		// searches the full transform hierarchy for a bone by name
		private Transform FindAttachPoint(string boneName)
		{
			if (string.IsNullOrEmpty(boneName)) return transform;

			Transform bone = FindDeep(transform, boneName);
			if (bone == null)
			{
				Debug.LogWarning($"Inventory: attach bone '{boneName}' not found on {gameObject.name}.", this);
				return transform;
			}

			return bone;
		}

		// recursive depth-first search through the transform hierarchy
		private Transform FindDeep(Transform parent, string name)
		{
			if (parent.name == name) return parent;
			foreach (Transform child in parent)
			{
				Transform result = FindDeep(child, name);
				if (result != null) return result;
			}
			return null;
		}
	}
}