using UnityEngine;
using UnityEngine.Events;
using CGL.Events;
using CGL.Data;

namespace CGL.Actor
{
	// player actor that manages health data and score.
	// attach alongside a Health component and a Controller component for movement.
	[RequireComponent(typeof(Health))]
	public class Player : Actor
	{
		[Header("Data")]
		[SerializeField]
		[Tooltip("Shared health percent data updated when health changes, used by UI.")]
		private FloatDataSO healthData;

		[SerializeField]
		[Tooltip("Shared score data incremented when the player scores points.")]
		private IntDataSO scoreData;

		[Header("Events")]
		[SerializeField]
		[Tooltip("Raised when the player dies, passes the player GameObject.")]
		private GameObjectEventSO onPlayerDeathEvent;

		[SerializeField]
		[Tooltip("Raised when points are awarded to the player.")]
		private IntEventSO onScoreEvent;

		[Tooltip("Raised locally when health of score changes to update the UI.")]
		public EventSO onUIUpdateEvent;

		protected override void OnEnable()
		{
			base.OnEnable();
			Health.OnHealthChanged.AddListener(OnHealthChanged);
			Health.OnDeath.AddListener(OnDeath);
			onScoreEvent.Subscribe(AddScore);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Health.OnHealthChanged.RemoveListener(OnHealthChanged);
			Health.OnDeath.RemoveListener(OnDeath);
			onScoreEvent.Unsubscribe(AddScore);
		}

		// adds points to the score and notifies listeners
		public void AddScore(int points)
		{
			if (scoreData == null) return;

			scoreData.value += points;
			onUIUpdateEvent?.RaiseEvent();
		}

		private void OnHealthChanged(float healthPercent)
		{
			print($"health {healthPercent}");
			if (healthData != null)
			{
				healthData.value = healthPercent;
				onUIUpdateEvent?.RaiseEvent();
			}
		}

		protected override void OnDeath()
		{
			onDeath?.Invoke();
			onPlayerDeathEvent?.RaiseEvent(gameObject);
		}
	}
}