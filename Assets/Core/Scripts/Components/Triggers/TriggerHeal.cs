using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using CGL.Actor;

namespace CGL.Trigger
{
	// heals any Health component that enters the trigger.
	// supports single heal on enter or repeating heal over time with interval.
	public class TriggerHeal : TriggerBase
	{
		[SerializeField]
		[Tooltip("Amount healed per hit or per interval tick.")]
		private float healAmount = 25f;

		[SerializeField]
		[Tooltip("If true, healing repeats on an interval while inside the trigger.")]
		private bool healOverTime = false;

		[SerializeField]
		[Tooltip("Seconds between heal ticks when heal over time is enabled.")]
		private float healInterval = 0.5f;

		[SerializeField]
		[Tooltip("Raised when this trigger successfully heals a target, passes heal amount.")]
		private UnityEvent<float> onHealApplied;

		// keyed on Health so cleanup in the coroutine is straightforward
		private Dictionary<CGL.Actor.Health, Coroutine> activeTargets = new();

		protected override void OnEnter(Collider other)
		{
			if (!other.TryGetComponent(out CGL.Actor.Health health)) return;

			if (healOverTime)
			{
				if (!activeTargets.ContainsKey(health))
					activeTargets[health] = StartCoroutine(HealOverTime(health));
			}
			else
			{
				ApplyHeal(health);
			}
		}

		protected override void OnExit(Collider other)
		{
			if (!other.TryGetComponent(out CGL.Actor.Health health)) return;
			if (!activeTargets.TryGetValue(health, out Coroutine coroutine)) return;

			StopCoroutine(coroutine);
			activeTargets.Remove(health);
		}

		private void ApplyHeal(CGL.Actor.Health health)
		{
			if (!health.IsAlive || health.IsAtMaxHealth) return;

			health.Heal(healAmount);
			onHealApplied?.Invoke(healAmount);
		}

		private IEnumerator HealOverTime(CGL.Actor.Health health)
		{
			while (health.IsAlive && !health.IsAtMaxHealth)
			{
				ApplyHeal(health);
				yield return new WaitForSeconds(healInterval);
			}

			// target is either dead or full — clean up without needing OnExit
			activeTargets.Remove(health);
		}
	}
}