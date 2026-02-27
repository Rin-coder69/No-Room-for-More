using CGL.Actor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CGL.Trigger
{
	// damages any IDamageable that enters the trigger.
	// supports single hit on enter or repeating damage over time with interval.
	public class TriggerDamage : TriggerBase
	{
		[SerializeField]
		[Tooltip("Damage applied per hit or per interval tick.")]
		private float damage = 10f;

		[SerializeField]
		[Tooltip("If true, damage repeats on an interval while inside the trigger.")]
		private bool damageOverTime = false;

		[SerializeField]
		[Tooltip("Seconds between damage ticks when damage over time is enabled.")]
		private float damageInterval = 0.5f;

		[SerializeField]
		[Tooltip("Raised when this trigger successfully damages a target, passes damage amount.")]
		private UnityEvent<float> onDamageDealt;

		// keyed on IDamageable so the coroutine can clean itself up when the target dies
		private Dictionary<IDamageable, Coroutine> activeTargets = new();

		protected override void OnEnter(Collider other)
		{
			if (!other.TryGetComponent(out IDamageable damageable)) return;

			if (damageOverTime)
			{
				if (!activeTargets.ContainsKey(damageable))
					activeTargets[damageable] = StartCoroutine(DamageOverTime(damageable));
			}
			else
			{
				ApplyDamage(damageable);
			}
		}

		protected override void OnExit(Collider other)
		{
			if (!other.TryGetComponent(out IDamageable damageable)) return;
			if (!activeTargets.TryGetValue(damageable, out Coroutine coroutine)) return;

			StopCoroutine(coroutine);
			activeTargets.Remove(damageable);
		}

		private void ApplyDamage(IDamageable damageable)
		{
			if (!damageable.IsAlive) return;
						
			damageable.TakeDamage(damage);
			onDamageDealt?.Invoke(damage);
		}

		private IEnumerator DamageOverTime(IDamageable damageable)
		{
			while (damageable.IsAlive)
			{
				ApplyDamage(damageable);
				yield return new WaitForSeconds(damageInterval);
			}

			// target died — clean up without needing OnExit
			activeTargets.Remove(damageable);
		}
	}
}