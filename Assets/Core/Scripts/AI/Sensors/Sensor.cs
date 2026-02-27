using System.Collections;
using UnityEngine;

namespace CGL.AI
{
	public abstract class Sensor : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Tag name to sense (optional).")]
		protected string targetTag;
		
		[SerializeField]
		[Tooltip("Layer mask to sense.")]
		protected LayerMask targetLayerMask = Physics.AllLayers;

		[SerializeField]
		[Range(0.0f, 5.0f)]
		[Tooltip("Rate in seconds of sensing.")]
		private float senseRate = 0.1f;

		[SerializeField]
		[Range(1, 20)]
		[Tooltip("Maximum number of objects that can be sensed.")]
		protected int maxSensed = 10;

		// array of objects sensed
		public GameObject[] Sensed { get; protected set; }
		public int SensedCount { get; protected set; }

		Coroutine senseCoroutine = null;

		// derived classes need to override Sense() method
		protected abstract void Sense();

		protected virtual void Awake()
		{
			// arrays are pre-allocated to avoid memory allocation/deallocation
			Sensed = new GameObject[maxSensed];
		}

		private void OnEnable()
		{
			if (senseCoroutine != null)
			{
				StopCoroutine(senseCoroutine);
				senseCoroutine = null;
			}
			senseCoroutine = StartCoroutine(SenseCR());
		}

		private void OnDisable()
		{
			if (senseCoroutine != null)
			{
				StopCoroutine(senseCoroutine);
				senseCoroutine = null;
			}
		}

		IEnumerator SenseCR()
		{
			while (true)
			{
				Sense();
				if (senseRate == 0) yield return null;
				else yield return new WaitForSeconds(senseRate);
			}
		}
	}
}
