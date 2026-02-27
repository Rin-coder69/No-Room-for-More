using CGL.Actor;
using UnityEngine;

namespace CGL.Pickup
{
	// restores health to the collector on contact.
	// stays in the world if the collector is already at max health.
	public class HealthPickup : Pickup
	{
		[SerializeField]
		[Tooltip("Amount of health to restore on pickup.")]
		private float healAmount = 25f;

		protected override bool OnCollect(Collider other)
		{
			CGL.Actor.Health health = other.GetComponent<CGL.Actor.Health>();
			if (health == null || health.IsAtMaxHealth) return false;

			health.Heal(healAmount);
			return true;
		}
	}
}
