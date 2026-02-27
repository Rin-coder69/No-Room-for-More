using UnityEngine;

namespace CGL.AI
{
	public class SphereCastSensor : Sensor
	{
		[SerializeField]
		[Range(0.1f, 50.0f)]
		[Tooltip("SphereCast distance.")]
		private float distance;

		[SerializeField]
		[Range(0.1f, 50.0f)]
		[Tooltip("SphereCast angle.")]
		private float angle;

		[SerializeField]
		[Range(1, 20)]
		[Tooltip("Number of rays to cast.")]
		private int rayCount;

		[SerializeField]
		[Range(0.1f, 50.0f)]
		[Tooltip("SphereCast radius.")]
		private float radius;


		override protected void Sense()
		{
			SensedCount = 0;

			float angleStep = (rayCount > 1) ? angle / (rayCount - 1) : 0;
			float startAngle = -angle / 2;

			for (int i = 0; i < rayCount; i++)
			{
				float currentAngle = startAngle + (angleStep * i);
				// direction is in object space
				Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

				Ray ray = new Ray(transform.position, direction);
				if (Physics.SphereCast(ray, radius, out RaycastHit hit, distance, targetLayerMask))
				{
					// check target tag if exists
					if (string.IsNullOrEmpty(targetTag) || hit.collider.CompareTag(targetTag))
					{
						if (SensedCount < Sensed.Length)
						{
							// add collider game object to sensed array
							Sensed[SensedCount++] = hit.collider.gameObject;
						}
					}
				}
			}
		}
	}
}
