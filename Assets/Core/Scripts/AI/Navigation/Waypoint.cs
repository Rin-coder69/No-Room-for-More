using UnityEngine;

namespace CGL.Navigation
{
	public class Waypoint : MonoBehaviour
	{
		[SerializeField] Waypoint[] nextWaypoints;

		public Waypoint GetNextWaypoint()
		{
			if (nextWaypoints == null || nextWaypoints.Length == 0) return null;
			return nextWaypoints[Random.Range(0, nextWaypoints.Length)];
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out WaypointFollower follower))
			{
				if (follower.TargetWaypoint == this)
				{
					follower.SetNextWaypoint(GetNextWaypoint());
				}
			}
		}

		public static Waypoint FindNearest(Vector3 position)
		{
			Waypoint[] allWaypoints = FindObjectsByType<Waypoint>(FindObjectsSortMode.None);
			Waypoint nearest = null;
			float nearestDistance = float.MaxValue;

			foreach (Waypoint waypoint in allWaypoints)
			{
				float distance = Vector3.Distance(position, waypoint.transform.position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearest = waypoint;
				}
			}
			return nearest;
		}
	}
}