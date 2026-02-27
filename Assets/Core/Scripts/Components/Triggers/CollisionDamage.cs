using CGL.Actor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CGL.Trigger
{
	// damages any IDamageable on physics collision.
	// supports flat damage or velocity-scaled impact damage, and damage over time for crushing surfaces.
	public class CollisionDamage : CollisionBase
	{
		[SerializeField]
		[Tooltip("If true, damage is scaled by the relative velocity of the collision.")]
		private bool useVelocityScaling = true;

		[SerializeField]
		[Tooltip("Multiplier applied to relative velocity to calculate impact damage.")]
		private float damageMultiplier = 1f;

		[SerializeField]
		[Tooltip("Flat damage applied on collision when velocity scaling is disabled.")]
		private float flatDamage = 10f;

		[SerializeField]
		[Tooltip("Minimum velocity magnitude required to register a damage hit.")]
		private float minimumVelocity = 1f;

		[SerializeField]
		[Tooltip("If true, damage repeats on an interval while in contact (crushing, grinding).")]
		private bool damageOverTime = false;

		[SerializeField]
		[Tooltip("Seconds between damage ticks when damage over time is enabled.")]
		private float damageInterval = 0.5f;

		[SerializeField]
		[Tooltip("Raised when this collision successfully damages a target, passes damage amount.")]
		private UnityEvent<float> onDamageDealt;

		// keyed on IDamageable so the coroutine can clean itself up when the target dies
		private Dictionary<IDamageable, Coroutine> activeTargets = new();

		protected override void OnEnter(Collision other)
		{
			if (!other.gameObject.TryGetComponent(out IDamageable damageable)) return;

			if (damageOverTime)
			{
				if (!activeTargets.ContainsKey(damageable))
					activeTargets[damageable] = StartCoroutine(DamageOverTime(damageable));
			}
			else
			{
				float damage = CalculateDamage(other);
				ApplyDamage(damageable, damage);
			}
		}

		protected override void OnExit(Collision other)
		{
			if (!other.gameObject.TryGetComponent(out IDamageable damageable)) return;
			if (!activeTargets.TryGetValue(damageable, out Coroutine coroutine)) return;

			StopCoroutine(coroutine);
			activeTargets.Remove(damageable);
		}

		private float CalculateDamage(Collision collision)
		{
			if (!useVelocityScaling) return flatDamage;

			float velocity = collision.relativeVelocity.magnitude;
			if (velocity < minimumVelocity) return 0;

			return velocity * damageMultiplier;
		}

		private void ApplyDamage(IDamageable damageable, float damage)
		{
			if (!damageable.IsAlive || damage <= 0) return;

			damageable.TakeDamage(damage);
			onDamageDealt?.Invoke(damage);
		}

		private IEnumerator DamageOverTime(IDamageable damageable)
		{
			while (damageable.IsAlive)
			{
				ApplyDamage(damageable, flatDamage);
				yield return new WaitForSeconds(damageInterval);
			}

			// target died — clean up without needing OnExit
			activeTargets.Remove(damageable);
		}
	}
}