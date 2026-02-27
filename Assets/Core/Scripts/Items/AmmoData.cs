using UnityEngine;

namespace CGL.Inventory
{
	// defines the type of ammunition behavior
	public enum AmmoType
	{
		Projectile,	// physics-based, follows trajectory, affected by gravity
		HitScan,    // instant hit detection using raycasts
		Melee       // overlap sphere detection
	}

	// defines ammunition properties and behavior.
	// controls how projectiles function, damage calculation, and impact effects.
	[CreateAssetMenu(fileName = "Ammo", menuName = "CGL/Inventory/Ammo")]
	public class AmmoData : ScriptableObject
	{
		[Header("General")]
		[SerializeField]
		[Tooltip("The fundamental behavior type of this ammunition.")]
		public AmmoType ammoType;

		[SerializeField]
		[Tooltip("Duration in seconds before projectile is destroyed. 0 = infinite.")]
		public float lifetime = 0;

		[SerializeField]
		[Tooltip("Which layers will be detected by this ammunition.")]
		public LayerMask hitLayerMask = Physics.AllLayers;

		[Header("Damage")]
		[SerializeField]
		[Tooltip("Amount of damage dealt to targets hit by this ammunition.")]
		public float damage = 10f;

		[SerializeField]
		[Tooltip("If true, damage is applied continuously while in contact.")]
		public bool damageOverTime = false;

		[SerializeField]
		[Tooltip("Damage applied per second when damage over time is enabled.")]
		public float damageRate = 5f;

		[Header("Impact")]
		[SerializeField]
		[Tooltip("If true, projectile is destroyed upon hitting a valid target.")]
		public bool destroyOnImpact = true;

		[SerializeField]
		[Tooltip("If true, impact effects trigger when lifetime expires.")]
		public bool impactOnExpired = false;

		[SerializeField]
		[Tooltip("Visual effect spawned at the point of impact.")]
		public GameObject impactPrefab;

		[Header("Projectile")]
		[SerializeField]
		[Tooltip("Force applied to projectile rigidbody when fired.")]
		public float force = 10f;

		[SerializeField]
		[Tooltip("How the force is applied to the projectile rigidbody.")]
		public ForceMode forceMode = ForceMode.Impulse;

		[SerializeField]
		[Tooltip("If true, projectile is affected by gravity and follows an arc.")]
		public bool hasGravity = false;

		[SerializeField]
		[Tooltip("If true, projectile bounces off surfaces instead of being destroyed.")]
		public bool bounce = false;

		[SerializeField]
		[Tooltip("If true, projectile orients itself along its velocity vector.")]
		public bool rotateToVelocity = true;

		[Header("HitScan")]
		[SerializeField]
		[Tooltip("Maximum distance in world units the weapon can hit.")]
		public float distance = 100f;

		[Header("Melee")]
		[SerializeField]
		[Tooltip("Radius of the melee hit detection sphere.")]
		public float meleeRadius = 0.5f;

		[SerializeField]
		[Tooltip("Offset from transform position for the hit sphere center.")]
		public Vector3 meleeOffset = Vector3.forward;

		private void OnValidate()
		{
			if (damage < 0)
				Debug.LogWarning($"AmmoData {name} has negative damage value.", this);

			if (damageOverTime && damageRate <= 0)
				Debug.LogWarning($"AmmoData {name} has damage over time enabled but damage rate is 0.", this);

			if (ammoType == AmmoType.Projectile && force <= 0)
				Debug.LogWarning($"AmmoData {name} is a projectile but has no force applied.", this);

			if (ammoType == AmmoType.HitScan && distance <= 0)
				Debug.LogWarning($"AmmoData {name} is a hitscan but has no distance set.", this);
		}
	}
}