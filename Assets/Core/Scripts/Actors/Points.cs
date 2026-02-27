// awards points to the player when this object is destroyed or triggered.
// attach to any GameObject that should award points — enemies, crates, collectibles.
using CGL.Events;
using UnityEngine;

namespace CGL.Actor
{
	public class Points : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Points awarded when this object is scored.")]
		private int points = 10;

		[SerializeField]
		[Tooltip("Raised when points are awarded, passes point value.")]
		private IntEventSO onScoreEvent;

		public void AwardPoints()
		{
			onScoreEvent?.RaiseEvent(points);
		}
	}
}