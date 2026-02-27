using System.Linq;
using UnityEngine;

namespace CGL.Spawner
{
	// Spawner Points
	// Spawn objects from transform points in the scene.
	// Points can be manually assigned in the inspector or found by tag at runtime.
	public class SpawnerPoints : Spawner
	{
		[Header("Spawn Points")]

		[SerializeField]
		[Tooltip("Points to spawn objects at")]
		Transform[] spawnPoints;

		[SerializeField]
		[Tooltip("If provided, use objects with tag to spawn from")]
		string spawnTagName = ""; 

		void Awake()
		{
			// check if points are not assigned AND we have a valid tag to search for
			if ((spawnPoints == null || spawnPoints.Length == 0) && !string.IsNullOrEmpty(spawnTagName))
			{
				// find all objects with the specified tag and convert to transform array
				spawnPoints = GameObject.FindGameObjectsWithTag(spawnTagName)
								  .Select(go => go.transform)
								  .ToArray();

				// log warning if no objects with the tag were found
				if (spawnPoints.Length == 0)
				{
					Debug.LogWarning($"SpawnerPoints: No GameObjects found with tag '{spawnTagName}'");
				}
			}
		}

		public override void Spawn()
		{
			// validate that spawn points have been assigned
			if (spawnPoints == null || spawnPoints.Length == 0)
			{
				Debug.LogError("SpawnerPoints: No spawn points assigned!");
				return;
			}

			// find spawn point
			Transform spawnTransform = null;
			// maximum number of attempts to find a clear spawn position
			int attempts = MAX_SPAWN_ATTEMPTS;
			while (attempts-- > 0)
			{
				// select a random spawn point from the array
				spawnTransform = spawnPoints[Random.Range(0, spawnPoints.Length)];

				// if not avoiding overlap or position is clear, exit the loop with valid spawn transform
				if (!avoidOverlap || IsOverlapClear(spawnTransform.position))
					break;
			}

			// check if no spawn transform available
			if (spawnTransform == null)
			{
				Debug.LogWarning("SpawnerPoints: Unable to find a valid spawn position after multiple attempts!", this);
				return;
			}

			// get a spawn prefab
			GameObject spawnGameObject = GetSpawnPrefab();
			if (spawnGameObject == null) return; // prevents null object instantiation

			// spawn object from prefab
			Spawn(spawnGameObject, spawnTransform.position, spawnTransform.rotation);
		}
	}
}