using UnityEngine;

namespace CGL.Inventory
{
	// physics-based projectile that applies force on spawn and handles lifetime.
	[RequireComponent(typeof(Rigidbody))]
	public class ProjectileAmmo : Ammo
	{
		private Rigidbody rb;

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
			rb.useGravity = ammoData != null && ammoData.hasGravity;
		}

		private void Start()
		{
			if (ammoData == null) return;

			// apply initial forward force
			if (ammoData.force > 0)
				rb.AddRelativeForce(Vector3.forward * ammoData.force, ammoData.forceMode);

			// schedule destruction after lifetime expires
			if (ammoData.lifetime > 0)
			{
				Invoke(nameof(OnLifetimeExpired), ammoData.lifetime);
			}
		}

		private void Update()
		{
			if (ammoData == null) return;

			// rotate to align with velocity direction
			if (ammoData.rotateToVelocity && rb.linearVelocity != Vector3.zero)
				transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
		}

		protected override void OnCollisionEnter(Collision collision)
		{
			if (ammoData == null) return;

			// skip if bouncing
			if (ammoData.bounce) return;

			base.OnCollisionEnter(collision);
		}

		private void OnLifetimeExpired()
		{
			if (ammoData.impactOnExpired)
				SpawnImpactEffect(transform.position, transform.up);

			Destroy(gameObject);
		}
	}
}