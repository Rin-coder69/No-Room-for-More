using CGL.Inventory;
using UnityEngine;

namespace CGL.Pickup
{
	// restores ammo to a matching weapon in the collector's inventory on contact.
	// stays in the world if the collector has no matching weapon or ammo is already full.
	public class AmmoPickup : Pickup
	{
		[SerializeField]
		[Tooltip("Weapon data id matching the weapon this ammo refills.")]
		private string weaponId;

		[SerializeField]
		[Tooltip("Amount of ammo to restore on pickup.")]
		private int ammoAmount = 10;

		protected override bool OnCollect(Collider other)
		{
			CGL.Inventory.Inventory inventory = other.GetComponent<CGL.Inventory.Inventory>();
			if (inventory == null) return false;

			Weapon weapon = FindWeapon(inventory);
			if (weapon == null || !weapon.NeedsAmmo) return false;

			weapon.AddAmmo(ammoAmount);
			return true;
		}

		private Weapon FindWeapon(CGL.Inventory.Inventory inventory)
		{
			for (int i = 0; i < inventory.ItemCount; i++)
			{
				Item item = inventory.GetItem(i);
				if (item is Weapon weapon && weapon.GetData()?.id == weaponId)
					return weapon;
			}
			return null;
		}
	}
}
