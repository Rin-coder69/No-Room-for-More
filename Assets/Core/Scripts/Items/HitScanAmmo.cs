using UnityEngine;

namespace CGL.Inventory
{
	// instant hit detection using a raycast.
	// fires on start and destroys itself after processing.
	public class HitScanAmmo : Ammo
	{
		private void Start()
		{
			if (ammoData == null)
			{
				Destroy(gameObject);
				return;
			}

			FireRaycast();
			Destroy(gameObject);
		}

		private void FireRaycast()
		{
			if (!Physics.Raycast(transform.position, transform.forward,
				out RaycastHit hit, ammoData.distance, ammoData.hitLayerMask)) return;

			// apply damage to damageable target
			ApplyDamage(hit.collider.gameObject, hit.point, hit.normal);

			// spawn impact effect aligned to surface normal
			SpawnImpactEffect(hit.point, hit.normal);
		}

		// hitscan uses raycast not physics collision so disable base collision handling
		protected override void OnCollisionEnter(Collision collision) { }
		protected override void OnCollisionStay(Collision collision) { }
	}
}