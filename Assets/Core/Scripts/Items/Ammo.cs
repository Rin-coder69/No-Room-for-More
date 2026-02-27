using CGL.Actor;
using UnityEngine;

namespace CGL.Inventory
{
	// base class for all ammunition types.
	// handles common damage application and impact effects on collision.
	[RequireComponent(typeof(Collider))]
	public abstract class Ammo : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Ammo data defining behavior, damage, and impact properties.")]
		protected AmmoData ammoData;

		protected virtual void OnCollisionEnter(Collision collision)
		{
			HandleImpact(collision.gameObject, collision.contacts[0].point, collision.contacts[0].normal);
		}

		protected virtual void OnCollisionStay(Collision collision)
		{
			if (!ammoData.damageOverTime) return;
			ApplyDamage(collision.gameObject, collision.contacts[0].point, transform.forward);
		}

		protected void HandleImpact(GameObject target, Vector3 point, Vector3 normal)
		{
			if (ammoData == null) return;

			// check layer mask
			if ((ammoData.hitLayerMask & (1 << target.layer)) == 0) return;

			// apply damage if not damage over time
			if (!ammoData.damageOverTime)
				ApplyDamage(target, point, normal);

			// spawn impact prefab
			SpawnImpactEffect(point, normal);

			// destroy on impact
			if (ammoData.destroyOnImpact)
				Destroy(gameObject);
		}

		protected void ApplyDamage(GameObject target, Vector3 point, Vector3 normal)
		{
			if (!target.TryGetComponent(out IDamageable damageable)) return;

			damageable.TakeDamage(ammoData.damage);
		}

		protected void SpawnImpactEffect(Vector3 point, Vector3 normal)
		{
			if (ammoData.impactPrefab == null) return;

			Quaternion rotation = Quaternion.LookRotation(normal);
			Instantiate(ammoData.impactPrefab, point, rotation);
		}
	}
}