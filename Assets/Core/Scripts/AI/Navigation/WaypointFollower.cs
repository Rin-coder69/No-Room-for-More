using UnityEngine;

namespace CGL.Navigation
{
	public class WaypointFollower : MonoBehaviour
	{
		[SerializeField] protected float speed = 5f;

		protected Waypoint currentWaypoint;
		public Waypoint TargetWaypoint => currentWaypoint;

		void Start()
		{
			SetNextWaypoint(Waypoint.FindNearest(transform.position));
		}

		protected virtual void Update()
		{
			if (currentWaypoint == null) return;

			// move towards current waypoint position
			transform.position = Vector3.MoveTowards(transform.position,
				currentWaypoint.transform.position, speed * Time.deltaTime);
		}

		public virtual void SetNextWaypoint(Waypoint waypoint)
		{
			currentWaypoint = waypoint;
		}
	}
}