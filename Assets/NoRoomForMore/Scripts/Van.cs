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
    [SerializeField] float upwardForce = 1f;

    [SerializeField] float interactRange = 3f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactRange && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    void Interact()
    {
        VanDoor.SetActive(true);
        //Audio.Play();

        int randomIndex = Random.Range(0, Furn.Length);
        GameObject furniture = Instantiate(Furn[randomIndex], spawnPoint.position, Random.rotation);

        Rigidbody rb = furniture.GetComponent<Rigidbody>();
        if (rb == null)
            rb = furniture.AddComponent<Rigidbody>();

        Vector3 shootDirection = spawnPoint.forward * launchForce + Vector3.up * upwardForce;
        rb.AddForce(shootDirection, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}