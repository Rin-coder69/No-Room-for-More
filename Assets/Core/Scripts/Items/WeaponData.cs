using UnityEngine;
using CGL.Audio;

namespace CGL.Inventory
{
	// scriptable object defining weapon-specific properties.
	// extends ItemData with additional fields for weapon functionality.
	[CreateAssetMenu(fileName = "Weapon", menuName = "CGL/Inventory/Weapon")]
	public class WeaponData : ItemData
	{
		[Header("Firing Properties")]
		[SerializeField]
		[Tooltip("Time in seconds between shots.")]
		public float fireRate = 0.5f;

		[SerializeField]
		[Tooltip("Random spread applied to projectile direction on each shot.")]
		public Vector3 spread = Vector3.one * 0.1f;

		[Header("Ammunition")]
		[SerializeField]
		[Tooltip("Number of shots before reload required. 0 = infinite.")]
		public int rounds = 0;

		[SerializeField]
		[Tooltip("Prefab instantiated when weapon fires.")]
		public GameObject ammoPrefab;

		private void OnValidate()
		{
			if (fireRate <= 0)
				Debug.LogWarning($"WeaponData {name} has a fire rate of 0 or less.", this);

			if (ammoPrefab == null)
				Debug.LogWarning($"WeaponData {name} has no ammo prefab assigned.", this);
		}
	}
}