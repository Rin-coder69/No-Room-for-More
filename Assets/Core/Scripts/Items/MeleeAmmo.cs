using UnityEngine;

namespace CGL.Inventory
{
	// melee ammo using overlap sphere for hit detection.
	// spawn at the weapon or hand position via animation event.
	public class MeleeAmmo : Ammo
	{
		// set by the weapon that spawns this ammo — used to exclude the owner from hit detection
		public Transform Owner { get; set; }

		private void Start()
		{
			if (ammoData == null)
			{
				Destroy(gameObject);
				return;
			}

			FireMelee();
			Destroy(gameObject);
		}

		private void FireMelee()
		{
			Vector3 center = transform.position +
				transform.TransformDirection(ammoData.meleeOffset);

			// detect all colliders in radius
			Collider[] hits = Physics.OverlapSphere(center,
				ammoData.meleeRadius, ammoData.hitLayerMask);

			foreach (Collider hit in hits)
			{
				// skip owner and all their children
				if (Owner != null && hit.transform.IsChildOf(Owner)) continue;

				ApplyDamage(hit.gameObject, center, Vector3.up);
				SpawnImpactEffect(hit.transform.position, Vector3.up);
			}
		}

		// melee uses overlap sphere not physics collision
		protected override void OnCollisionEnter(Collision collision) { }
		protected override void OnCollisionStay(Collision collision) { }

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (ammoData == null) return;
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(
				transform.position + transform.TransformDirection(ammoData.meleeOffset),
				ammoData.meleeRadius);
		}
#endif
	}
}