using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Van : MonoBehaviour
{
    //[SerializeField] AudioSource Audio;
    [SerializeField] GameObject VanDoor;
    [SerializeField] GameObject[] Furn;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float launchForce = 5f;
    [SerializeField] float upwardForce = 1f;
    [SerializeField] int minFurniture = 15;
    [SerializeField] int maxFurniture = 15;
    [SerializeField] GameObject[] allFurniturePrefabs;

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
        // empty van
        if (Furn.Length == 0)
        {
            RestockVan();
            return;
        }

        VanDoor.SetActive(true);
        //Audio.Play();

        int randomIndex = Random.Range(0, Furn.Length);
        GameObject furniture = Instantiate(Furn[randomIndex], spawnPoint.position, Random.rotation);

        List<GameObject> furnList = new List<GameObject>(Furn);
        furnList.RemoveAt(randomIndex);
        Furn = furnList.ToArray();

        Rigidbody rb = furniture.GetComponent<Rigidbody>();
        if (rb == null)
            rb = furniture.AddComponent<Rigidbody>();

        Vector3 shootDirection = spawnPoint.forward * launchForce + Vector3.up * upwardForce;
        rb.AddForce(shootDirection, ForceMode.Impulse);


    }

    public void RestockVan()
    {
        List<GameObject> newStock = new List<GameObject>();

        int amountToAdd = Random.Range(minFurniture, maxFurniture + 1);

        for (int i = 0; i < amountToAdd; i++)
        {
            int randomIndex = Random.Range(0, allFurniturePrefabs.Length);
            newStock.Add(allFurniturePrefabs[randomIndex]);
        }

        Furn = newStock.ToArray();

        Debug.Log($"Van restocked with {Furn.Length} furniture pieces!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}