using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CGL.Actor;

namespace CGL.Spawner
{
	// Spawner
	// Asbstract class for spawner classes that spawn game objects.
	public abstract class Spawner : Activatable
	{
		[Header("Spawn Objects")]
		[SerializeField]
		[Tooltip("Array of objects to spawn")]
		GameObject[] spawnPrefabs;

		[SerializeField]
		[Tooltip("Parent object of all spawned objects (optional)")]
		Transform parent = null;

		[Header("Spawn Rate")]
		[SerializeField]
		[Range(0.0f, 60.0f)]
		float minSpawnTime = 1.0f;

		[SerializeField]
		[Range(0.0f, 60.0f)]
		float maxSpawnTime = 1.0f;

		[SerializeField]
		[Range(-1, 100)]
		[Tooltip("Maximum number of spawned objects active, -1 for no limit")]
		int maxSpawned = 1;

		[Header("Spawn Overlap")]
		[SerializeField]
		[Tooltip("Avoid spawning on top of an object.")]
		protected bool avoidOverlap = true;

		[SerializeField]
		[Tooltip("Radius to check of overlap objects.")]
		float overlapRadius = 0.5f;

		[SerializeField]
		[Tooltip("Layer Mask to check of overlap objects.")]
		LayerMask overlapLayerMask = Physics.AllLayers;

		// Runtime state tracking
		private List<GameObject> activeSpawns = new List<GameObject>(); // Tracks currently spawned objects
		private Coroutine spawnTimerCoroutine;                          // Reference to spawn timer coroutine

		protected const int MAX_SPAWN_ATTEMPTS = 5; // Maximum number of attempts to find a clear spawn position

		// abstract method that must be implemented by derived classes to define specific spawn behavior.
		public abstract void Spawn();



		protected override void Start()
		{
			base.Start();
		}

		protected void Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			// instanitate prefab and add to active spawns list
			GameObject go = Instantiate(prefab, position, rotation, parent);
			activeSpawns.Add(go);
		}

		bool IsReadyToSpawn()
		{
			// remove all null references from active spawns, destroyed objects will be null
			activeSpawns.RemoveAll(go => go == null);

			// check spawner is active and active spawns are less than max or max spawned is -1
			return (IsActive && (maxSpawned == -1 || activeSpawns.Count < maxSpawned));
		}

		protected GameObject GetSpawnPrefab()
		{
			// check spawn prefabs []
			if (spawnPrefabs == null || spawnPrefabs.Length == 0)
			{
				Debug.LogError("Spawner: No prefabs assigned!");
				return null;
			}

			// return random spawn prefab to instantiate
			return spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
		}

		protected bool IsOverlapClear(Vector3 position)
		{
			// check of location has overlapping collision (don't spawn on top of other object)
			return (Physics.OverlapSphere(position, overlapRadius, overlapLayerMask).Length == 0);
		}

		public override void OnActivate()
		{
			base.OnActivate();

			// stop existing coroutine to avoid duplicates
			if (spawnTimerCoroutine != null)
			{
				StopCoroutine(spawnTimerCoroutine);
				spawnTimerCoroutine = null;
			}

			// start spawn coroutine
			spawnTimerCoroutine = StartCoroutine(SpawnTimerCR());
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();

			// stop coroutine if exists
			if (spawnTimerCoroutine != null)
			{
				StopCoroutine(spawnTimerCoroutine);
				spawnTimerCoroutine = null;
			}
		}

		// coroutine that handles the timing of spawning
		IEnumerator SpawnTimerCR()
		{
			while (true)
			{
				// wait for a random time between min and max spawn time
				yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
				if (IsReadyToSpawn())
				{
					Spawn();
				}
			}
		}
	}
}

