using UnityEngine;

using CGL.Actor;

namespace CGL.Components
{
	public class Wave : Activatable
	{
		[Space]

		[SerializeField]
		[Range(0, 5)]
		[Tooltip("Wave frequency (speed of oscillation)")]
		float frequency = 1;
		
		[SerializeField]
		[Range(0, 5)]
		[Tooltip("Wave amplitude (distance traveled)")]
		float amplitude = 1;
		
		[SerializeField]
		[Tooltip("Direction of wave movement")]
		Vector3 waveDirection = Vector3.up;

		[SerializeField]
		[Range(0, 100)]
		[Tooltip("Random phase offset range")]
		float randomPhase = 0;

		Vector3 position;
		float phase = 0;

		protected override void Start()
		{
			base.Start();

			// store starting postion, wave will use this as origin
			position = transform.position;
			// Random.value returns a random value between 0.0-1.0
			// the random value is multiplied by randomPhase to set starting phase value
			phase = Random.value * randomPhase;
		}

		void Update()
		{
			// update the phase using delta time * frequency (speed of phase)
			phase += (frequency * Time.deltaTime);
			// set current position to start postion + normalize wave direction scaled by sine wave
			transform.position = position + waveDirection.normalized * Mathf.Sin(phase) * amplitude;
		}
	}
}
