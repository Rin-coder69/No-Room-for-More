using UnityEngine;
using UnityEngine.Events;

namespace CGL.Actor
{
	// attach to any entity that can receive damage or healing.
	// wire responses to damage, healing, and death via Unity Events in the inspector.
	public class Health : MonoBehaviour, IDamageable
	{
		[SerializeField]
		[Tooltip("Maximum health for this entity.")]
		[Range(1f, 1000f)]
		private float maxHealth = 100f;

		[SerializeField]
		[Tooltip("If true, entity will be destroyed when health reaches zero.")]
		private bool destroyOnDeath = false;

		[Header("Events")]
		[SerializeField]
		[Tooltip("Raised when damage is taken, passes current health percent (0-1).")]
		private UnityEvent<float> onDamageTaken;

		[SerializeField]
		[Tooltip("Raised when health is restored, passes current health percent (0-1).")]
		private UnityEvent<float> onHeal;

		[SerializeField]
		[Tooltip("Raised when health changes for any reason, passes current health percent (0-1).")]
		private UnityEvent<float> onHealthChanged;

		[SerializeField]
		[Tooltip("Raised when health reaches zero.")]
		private UnityEvent onDeath;

		// expose events as properties so derived classes and external components can subscribe in code
		public UnityEvent<float> OnDamageTaken => onDamageTaken;
		public UnityEvent<float> OnHeal => onHeal;
		public UnityEvent<float> OnHealthChanged => onHealthChanged;
		public UnityEvent OnDeath => onDeath;

		private float currentHealth;

		public float CurrentHealth => currentHealth;
		public float MaxHealth => maxHealth;
		public float HealthPercent => maxHealth > 0 ? currentHealth / maxHealth : 0;
		public bool IsAlive => currentHealth > 0;
		public bool IsAtMaxHealth => currentHealth >= maxHealth;

		private void Awake()
		{
			// initialize in Awake so other components can safely read health state in their Start
			currentHealth = maxHealth;
		}

		private void Start()
		{
			// invoke after all components are initialized so UI and listeners are ready
			onHealthChanged?.Invoke(HealthPercent);
		}

		public void TakeDamage(float amount)
		{
			if (!IsAlive || amount <= 0) return;

			currentHealth = Mathf.Max(currentHealth - amount, 0);
			onDamageTaken?.Invoke(HealthPercent);
			onHealthChanged?.Invoke(HealthPercent);

			print($"health: {currentHealth}");

			if (!IsAlive) HandleDeath();
		}

		public void Heal(float amount)
		{
			if (!IsAlive || amount <= 0) return;

			currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
			onHeal?.Invoke(HealthPercent);
			onHealthChanged?.Invoke(HealthPercent);
		}

		public void SetHealth(float amount)
		{
			currentHealth = Mathf.Clamp(amount, 0, maxHealth);
			onHealthChanged?.Invoke(HealthPercent);

			// ensure death fires if health is set to zero directly
			if (!IsAlive) HandleDeath();
		}

		private void HandleDeath()
		{
			onDeath?.Invoke();
			if (destroyOnDeath) Destroy(gameObject);
		}
	}
}