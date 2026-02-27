using UnityEngine;

namespace CGL.Spawner
{
	// Spawner Volume
	// Spawn objects from random point inside volume.
	// Uses Box or Sphere collider to define volume.
	public class SpawnerVolume : Spawner
	{
		[Header("Volume")]

		[SerializeField]
		[Tooltip("(Sphere, Box) collider that objects can spawn from.")]
		private Collider volume;

		private void Awake()
		{
			volume ??= GetComponent<Collider>();
		}

		public override void Spawn()
		{
			// validate that a collider has been assigned
			if (volume == null)
			{
				Debug.LogError("SpawnerVolume: No collider assigned for volume-based spawning!", this);
				return;
			}

			bool isClearPosition = false;
			Vector3 clearPosition = transform.position;

			// handle box volume
			if (volume is BoxCollider boxVolume)
			{
				// maximum number of attempts to find a clear spawn position
				int attempts = MAX_SPAWN_ATTEMPTS;
				while (attempts-- > 0)
				{
					// generate random position within box bounds
					clearPosition.x = Random.Range(boxVolume.bounds.min.x, boxVolume.bounds.max.x);
					clearPosition.y = Random.Range(boxVolume.bounds.min.y, boxVolume.bounds.max.y);
					clearPosition.z = Random.Range(boxVolume.bounds.min.z, boxVolume.bounds.max.z);

					// if not avoiding overlap or position is clear, exit the loop with valid spawn position
					if (!avoidOverlap || IsOverlapClear(clearPosition))
					{
						isClearPosition = true;
						break;
					}
				}
			}
			else if (volume is SphereCollider sphereVolume)
			{
				// maximum number of attempts to find a clear spawn position
				int attempts = MAX_SPAWN_ATTEMPTS;
				while (attempts-- > 0)
				{
					// generate random position within sphere
					clearPosition = sphereVolume.transform.position + (Random.insideUnitSphere * sphereVolume.radius);

					// if not avoiding overlap or position is clear, exit the loop with valid spawn position
					if (!avoidOverlap || IsOverlapClear(clearPosition))
					{
						isClearPosition = true;
						break;
					}
				}
			}
			else
			{
				Debug.LogError("SpawnerVolume: Unsupported collider type!", this);
				return;
			}

			// check if no clear position available
			if (!isClearPosition)
			{
				Debug.LogWarning("SpawnerVolume: Unable to find a valid spawn position after multiple attempts!", this);
				return;
			}

			// get a spawn prefab
			GameObject spawnGameObject = GetSpawnPrefab();
			if (spawnGameObject == null) return; // prevents null object instantiation

			// spawn object from prefab (uses spawner rotation)
			Spawn(spawnGameObject, clearPosition, transform.rotation);
		}
	}
}