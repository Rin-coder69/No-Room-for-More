using CGL.Controller;
using Unity.Cinemachine;
using UnityEngine;

public class CharacterExample : MonoBehaviour
{
	[SerializeField] private KinematicCharacterController kinematicCharacter;
	[SerializeField] private RotationCharacterController rotationCharacter;
	[SerializeField] private PhysicsCharacterController physicsCharacter;
	[SerializeField] private CinemachineCamera cinemachineCamera;

	private void Start()
	{
		kinematicCharacter.enabled = true;
		rotationCharacter.enabled = false;
		physicsCharacter.enabled = false;
		cinemachineCamera.Target.TrackingTarget = kinematicCharacter.transform;
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 150, 40), "Kinematic"))
		{
			kinematicCharacter.enabled = true;
			rotationCharacter.enabled = false;
			physicsCharacter.enabled = false;
			cinemachineCamera.Target.TrackingTarget = kinematicCharacter.transform;
		}

		if (GUI.Button(new Rect(10, 60, 150, 40), "Rotation"))
		{
			kinematicCharacter.enabled = false;
			rotationCharacter.enabled = true;
			physicsCharacter.enabled = false;
			cinemachineCamera.Target.TrackingTarget = rotationCharacter.transform;
		}

		if (GUI.Button(new Rect(10, 110, 150, 40), "Physics"))
		{
			kinematicCharacter.enabled = false;
			rotationCharacter.enabled = false;
			physicsCharacter.enabled = true;
			cinemachineCamera.Target.TrackingTarget = physicsCharacter.transform;
		}
	}
}
