using System.Collections;
using CGL.Events;
using UnityEngine;

namespace CGL.Inventory
{
	// handles weapon behavior including firing, ammunition, and animation.
	// supports different firing modes based on weapon data usage type.
	public class Weapon : Item
	{
		[SerializeField]
		[Tooltip("Weapon data containing weapon-specific settings.")]
		private WeaponData weaponData;

		[SerializeField]
		[Tooltip("Animator component for weapon animations.")]
		private Animator animator;

		[SerializeField]
		[Tooltip("Transform where ammo will be spawned.")]
		private Transform ammoTransform;

		[SerializeField]
		[Tooltip("Raised each time this weapon fires — use for muzzle flash, sound, recoil animation.")]
		private EventSO onFireEvent;

		private int ammoCount = 0;
		private bool weaponReady = false;
		private IEnumerator autoFireCoroutine;

		public int AmmoCount => ammoCount;

		// true if weapon uses ammo and is not at full capacity
		public bool NeedsAmmo => weaponData != null && weaponData.rounds > 0 && ammoCount < weaponData.rounds;

		// returns weaponData as ItemData — weaponData is the single source of truth for this item
		public override ItemData GetData() => weaponData;

		private void Start()
		{
			autoFireCoroutine = AutoFire();
			if (ammoTransform == null) ammoTransform = transform;

			// initialize ammo count from weapon data
			if (weaponData != null) ammoCount = weaponData.rounds;
		}

		public override void Equip()
		{
			if (weaponData == null) return;
			base.Equip();

			weaponReady = true;

			if (animator != null && !string.IsNullOrEmpty(weaponData.animEquipName))
				animator.SetBool(weaponData.animEquipName, true);
		}

		public override void Unequip()
		{
			if (weaponData == null) return;
			base.Unequip();

			if (animator != null && !string.IsNullOrEmpty(weaponData.animEquipName))
				animator.SetBool(weaponData.animEquipName, false);
		}

		public override void Use()
		{
			if (!IsReady()) return;

			// use animation if trigger name is set
			if (animator != null && !string.IsNullOrEmpty(weaponData.animTriggerName))
			{
				FireWithAnimation();
			}
			else
			{
				FireDirect();
			}
		}

		public override void StopUse()
		{
			if (weaponData == null) return;

			if (weaponData.usageType == UsageType.Single ||
				weaponData.usageType == UsageType.Burst)
			{
				weaponReady = true;
			}

			if (autoFireCoroutine != null)
				StopCoroutine(autoFireCoroutine);
		}

		public override bool IsReady()
		{
			if (weaponData == null) return false;
			return weaponReady && (weaponData.rounds == 0 || ammoCount > 0);
		}

		// adds ammo up to the weapon's maximum rounds capacity
		public void AddAmmo(int amount)
		{
			if (weaponData == null) return;
			ammoCount = Mathf.Min(ammoCount + amount, weaponData.rounds);
		}

		// called by animation events to fire at the correct frame
		public override void OnAnimEventItemUse()
		{
			Fire();
		}

		private void FireWithAnimation()
		{
			animator.SetTrigger(weaponData.animTriggerName);
			weaponReady = false;
		}

		private void FireDirect()
		{
			switch (weaponData.usageType)
			{
				case UsageType.Single:
				case UsageType.Burst:
					Fire();
					if (weaponData.fireRate > 0)
					{
						weaponReady = false;
						StartCoroutine(ResetFireTimer());
					}
					break;

				case UsageType.Auto:
				case UsageType.Stream:
					autoFireCoroutine = AutoFire();
					StartCoroutine(autoFireCoroutine);
					break;
			}
		}

		// instantiates the ammo prefab and raises the fire event.
		// works for all ammo types — projectile, hitscan, and melee.
		private void Fire()
		{
			if (weaponData?.ammoPrefab == null) return;

			// apply spread to rotation
			Vector3 spreadOffset = new Vector3(
				Random.Range(-weaponData.spread.x, weaponData.spread.x),
				Random.Range(-weaponData.spread.y, weaponData.spread.y),
				Random.Range(-weaponData.spread.z, weaponData.spread.z));

			Quaternion spreadRotation = ammoTransform.rotation *
				Quaternion.Euler(spreadOffset);

			GameObject go = Instantiate(weaponData.ammoPrefab, ammoTransform.position, spreadRotation);

			// pass owner to melee ammo so it can exclude the player from hit detection
			MeleeAmmo meleeAmmo = go.GetComponent<MeleeAmmo>();
			if (meleeAmmo != null) meleeAmmo.Owner = transform.root;

			// decrement ammo if not infinite
			if (weaponData.rounds > 0) ammoCount--;

			// raised for all ammo types — muzzle flash, sound, recoil animation
			onFireEvent?.RaiseEvent();
		}

		// waits for fire rate duration then resets weapon ready state
		private IEnumerator ResetFireTimer()
		{
			yield return new WaitForSeconds(weaponData.fireRate);
			weaponReady = true;
		}

		// continuously fires at intervals based on fire rate
		private IEnumerator AutoFire()
		{
			while (true)
			{
				Fire();
				yield return new WaitForSeconds(weaponData.fireRate);
			}
		}
	}
}