using UnityEngine;

namespace CGL.Actor
{
	public class Destructable : Actor
	{
		[SerializeField]
		[Tooltip("Spawned at this transform when destroyed.")]
		private GameObject debrisPrefab;

		[SerializeField]
		[Tooltip("If true, spawns debris when destroyed.")]
		private bool spawnDebrisOnDeath = true;

		protected override void OnDeath()
		{
			base.OnDeath();

			if (spawnDebrisOnDeath && debrisPrefab != null)
				Instantiate(debrisPrefab, transform.position, transform.rotation);
		}
	}
}