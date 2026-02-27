using UnityEngine;

namespace CGL.Navigation
{
	[RequireComponent(typeof(NavMeshMover))]
	public class NavMeshWaypointFollower : WaypointFollower
	{
		private NavMeshMover navMeshMover;

		void Awake()
		{
			navMeshMover = GetComponent<NavMeshMover>();
		}

		protected override void Update()
		{
			if (currentWaypoint == null) return;

			// AtDestination fallback in case trigger doesn't fire
			if (navMeshMover.AtDestination)
			{
				SetNextWaypoint(currentWaypoint.GetNextWaypoint());
			}
		}

		public override void SetNextWaypoint(Waypoint waypoint)
		{
			base.SetNextWaypoint(waypoint);
			if (currentWaypoint == null) return;
			navMeshMover.Destination = currentWaypoint.transform.position;
			navMeshMover.Speed = speed;
		}
	}
}