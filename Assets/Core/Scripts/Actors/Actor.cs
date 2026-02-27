using CGL.Events;
using UnityEngine;
using UnityEngine.Events;

namespace CGL.Actor
{
	// base class for any entity with health.
	// provides common wiring between Health component and derived actor behavior.
	[RequireComponent(typeof(Health))]
	public class Actor : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Optional event raised when this actor dies (cross-scene).")]
		private GameObjectEventSO onDeathEvent;

		[Header("Events")]
		[Tooltip("Raised when this actor takes damage, passes health percent.")]
		public UnityEvent<float> onDamageTaken;

		[Tooltip("Raised when this actor dies.")]
		public UnityEvent onDeath;

		public Health Health { get; private set; }
		public bool IsAlive => Health.IsAlive;

		protected virtual void Awake()
		{
			Health = GetComponent<Health>();
		}

		protected virtual void OnEnable()
		{
			Health.OnDamageTaken.AddListener(OnDamageTaken);
			Health.OnDeath.AddListener(OnDeath);
		}

		protected virtual void OnDisable()
		{
			Health.OnDamageTaken.RemoveListener(OnDamageTaken);
			Health.OnDeath.RemoveListener(OnDeath);
		}

		protected virtual void OnDamageTaken(float healthPercent)
		{
			onDamageTaken?.Invoke(healthPercent);
		}

		protected virtual void OnDeath()
		{
			onDeathEvent?.RaiseEvent(gameObject);
			onDeath?.Invoke();
		}
	}
}