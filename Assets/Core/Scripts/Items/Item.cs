using UnityEngine;

namespace CGL.Inventory
{
	// base class for all equippable and usable items in the game.
	// provides common equip/unequip behavior and defines the interface all items must implement.
	// each derived class owns its own data field and returns it via GetData().
	public abstract class Item : MonoBehaviour
	{
		public bool IsEquipped { get; protected set; } = false;

		// equips this item, making it visible and usable
		public virtual void Equip()
		{
			if (IsEquipped) return;
			IsEquipped = true;
			gameObject.SetActive(true);
		}

		// unequips this item, hiding it and preventing its use
		public virtual void Unequip()
		{
			if (!IsEquipped) return;
			IsEquipped = false;
			gameObject.SetActive(false);
		}

		// returns the item data for this item — each derived class owns its own data field
		public abstract ItemData GetData();

		// checks if the item is ready to be used
		public abstract bool IsReady();

		// begins using the item
		public abstract void Use();

		// stops using the item
		public abstract void StopUse();

		// called by animation events during item use sequences
		public abstract void OnAnimEventItemUse();
	}
}