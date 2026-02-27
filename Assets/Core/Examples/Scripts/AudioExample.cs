using CGL.Audio;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioExample : MonoBehaviour
{
    [SerializeField] AudioCue footstep;

    void Start()
    {
        //   
    }
     
    void Update()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            footstep.Play();
        }
    }
}
