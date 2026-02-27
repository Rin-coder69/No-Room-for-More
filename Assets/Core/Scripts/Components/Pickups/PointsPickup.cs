using CGL.Events;
using UnityEngine;

namespace CGL.Pickup
{
	// awards points to the collector on contact.
	public class PointsPickup : Pickup
	{
		[SerializeField]
		[Tooltip("Points awarded when this pickup is collected.")]
		private int points = 10;

		[SerializeField]
		[Tooltip("Raised when points are awarded, passes point value.")]
		private IntEventSO onScoreEvent;

		protected override bool OnCollect(Collider other)
		{
			onScoreEvent?.RaiseEvent(points);
			return true;
		}
	}
}
