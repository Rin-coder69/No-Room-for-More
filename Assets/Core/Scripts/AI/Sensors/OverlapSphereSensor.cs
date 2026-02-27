using UnityEngine;

namespace CGL.AI
{
	public class OverlapSphereSensor : Sensor
	{
		[SerializeField]
		[Range(0.1f, 50.0f)]
		[Tooltip("Overlap Sphere radius.")]
		private float radius;

		private Collider[] colliderBuffer;

		override protected void Awake()
		{
			base.Awake();
			colliderBuffer = new Collider[maxSensed];
		}

		override protected void Sense()
		{
			SensedCount = 0;

			int count = Physics.OverlapSphereNonAlloc(transform.position, radius, colliderBuffer, targetLayerMask);
			for (int i = 0; i < count; i++) 
			{
				// check target tag if exists
				if (string.IsNullOrEmpty(targetTag) || colliderBuffer[i].CompareTag(targetTag))
				{
					if (SensedCount < Sensed.Length)
					{
						// add collider game object to sensed array
						Sensed[SensedCount++] = colliderBuffer[i].gameObject;
					}
				}
			}
		}
	}
}
