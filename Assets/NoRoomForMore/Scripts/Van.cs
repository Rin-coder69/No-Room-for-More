using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Van : MonoBehaviour
{
    //[SerializeField] AudioSource Audio;
    [SerializeField] GameObject VanDoor;
    [SerializeField] GameObject[] Furn;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float launchForce = 5f;
    [SerializeField] float upwardForce = 3f;

    void Start()
    {

    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    void Interact()
    {
        // open the door
        VanDoor.SetActive(true);

        // play sound
        //Audio.Play();

        // spawn and launch furniture
        int randomIndex = Random.Range(0, Furn.Length);
        GameObject furniture = Instantiate(Furn[randomIndex], spawnPoint.position, Random.rotation);

        Rigidbody rb = furniture.GetComponent<Rigidbody>();
        if (rb == null)
            rb = furniture.AddComponent<Rigidbody>();

        Vector3 shootDirection = spawnPoint.forward + Vector3.up * upwardForce;
        rb.AddForce(shootDirection.normalized * launchForce, ForceMode.Impulse);
    }
}