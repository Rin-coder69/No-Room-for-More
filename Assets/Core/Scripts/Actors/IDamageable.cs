using UnityEngine;

namespace CGL.Actor
{
	// interface for any entity that can receive and respond to damage.
	public interface IDamageable
	{
		// current health of the entity
		float CurrentHealth { get; }

		// maximum possible health for the entity
		float MaxHealth { get; }

		// normalized health value from 0 to 1 for UI progress bars
		float HealthPercent => CurrentHealth / MaxHealth;

		// true when current health is greater than zero
		bool IsAlive => CurrentHealth > 0;

		// applies damage to the entity
		void TakeDamage(float amount);

		// heals the entity by the specified amount
		void Heal(float amount);
	}
}