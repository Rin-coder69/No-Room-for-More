using UnityEngine;
using UnityEngine.AI;

namespace CGL.Navigation
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class NavMeshMover : MonoBehaviour
	{
		[SerializeField]
		[Range(0.1f, 20.0f)]
		[Tooltip("Movement speed.")]
		private float speed = 5.0f;

		[SerializeField]
		[Range(0.1f, 10.0f)]
		[Tooltip("How close to destination before considered arrived.")]
		private float stoppingDistance = 0.5f;

		[SerializeField]
		[Range(0.1f, 360.0f)]
		[Tooltip("How fast the agent turns.")]
		private float angularSpeed = 120.0f;

		[SerializeField]
		[Range(0.1f, 10.0f)]
		[Tooltip("How fast the agent accelerates.")]
		private float acceleration = 8.0f;

		private NavMeshAgent agent;

		// expose destination so other systems can set it
		public Vector3 Destination
		{
			get => agent.destination;
			set => agent.SetDestination(value);
		}

		// movement speed
		public float Speed
		{
			get => agent.speed;
			set => agent.speed = value;
		}

		// distance from destination to stop
		public float StoppingDistance
		{
			get => agent.stoppingDistance;
			set => agent.stoppingDistance = value;
		}

		// check if at destination
		public bool AtDestination => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

		// normalized speed for animation (0-1)
		public float NormalizedSpeed => agent.velocity.magnitude / agent.speed;

		// stop agent movement
		public bool IsStopped
		{
			get => agent.isStopped;
			set => agent.isStopped = value;
		}

		void Awake()
		{
			agent = GetComponent<NavMeshAgent>();
			if (agent == null)
			{
				Debug.LogError("NavMeshMovement: No NavMeshAgent component found!", this);
				return;
			}

			// set serialized values to agent
			agent.speed = speed;
			agent.stoppingDistance = stoppingDistance;
			agent.angularSpeed = angularSpeed;
			agent.acceleration = acceleration;
		}
	}
}
