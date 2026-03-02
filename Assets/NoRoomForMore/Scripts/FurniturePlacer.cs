using CGL.Controller;
using CGL.Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FurniturePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GridManager> gridManagers;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LimitedInventory inventory;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private FPSKinematicCharacterController playerMovementScript;

    [Header("Furniture Settings")]
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private Material ValidPlacementMatieral;
    [SerializeField] private Material InvalidPlacementMaterial;
    [SerializeField] private float rotationStep = 90f;
    [SerializeField] private float moveSpeed = 5f;

    private GameObject selectedFurniture;
    private GameObject previewObject;
    private Vector2Int furnitureSize;
    public bool isPlacing = false;
    private float currentRotation = 0f;
    private Vector3 previewPosition;

    // helper to get the active grid manager
    private GridManager ActiveGrid => gridManagers != null && gridManagers.Count > 0 ? gridManagers[0] : null;

    void Update()
    {
        if (isPlacing && previewObject != null)
        {
            UpdatePreview();

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                currentRotation += rotationStep;
                previewObject.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                TryPlaceFurniture();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelPlacement();
            }
        }
    }

    public void StartPlacing(GameObject furniturePrefab, Vector2Int size)
    {
        selectedFurniture = furniturePrefab;
        furnitureSize = size;
        isPlacing = true;
        currentRotation = 0f;
        playerMovementScript.enabled = false;

        Vector3 flatForward = playerTransform.forward;
        flatForward.y = 0;
        flatForward.Normalize();
        previewPosition = playerTransform.position + flatForward * 3f;
        previewPosition.y = 0;

        previewObject = Instantiate(selectedFurniture);
        DisablePreviewPhysics(previewObject);
    }

    void UpdatePreview()
    {
        if (ActiveGrid == null) return;

        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, floorLayer))
        {
            previewPosition = hit.point;
            previewPosition.y = 0;

            Vector3 snappedPos = ActiveGrid.SnapToGrid(previewPosition);
            previewObject.transform.position = snappedPos;
            previewObject.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            Vector2Int gridPos = ActiveGrid.WorldToGrid(snappedPos);
            bool canPlace = ActiveGrid.CanPlaceFurniture(gridPos, furnitureSize);
            SetPreviewMaterial(canPlace ? ValidPlacementMatieral : InvalidPlacementMaterial);
        }
    }

    void TryPlaceFurniture()
    {
        if (ActiveGrid == null) return;

        Vector3 placementPos = previewObject.transform.position;
        Vector2Int gridPos = ActiveGrid.WorldToGrid(placementPos);
        playerMovementScript.enabled = true;

        if (ActiveGrid.CanPlaceFurniture(gridPos, furnitureSize))
        {
            GameObject placed = Instantiate(selectedFurniture, placementPos, Quaternion.Euler(0, currentRotation, 0));

            Rigidbody rb = placed.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            ActiveGrid.OccupyTiles(gridPos, furnitureSize);
            ScoreManager.Instance.AddScore(10);

            Item item = inventory.CurrentItem;
            inventory.RemoveItem(item);
            inventoryUI.RefreshUI();

            Destroy(previewObject);
            isPlacing = false;
            selectedFurniture = null;
            currentRotation = 0f;
        }
        else
        {
            Debug.Log("Cannot place furniture here!");
        }
    }

    void CancelPlacement()
    {
        Destroy(previewObject);
        isPlacing = false;
        selectedFurniture = null;
        currentRotation = 0f;
        playerMovementScript.enabled = true;
    }

    void DisablePreviewPhysics(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        foreach (Collider col in obj.GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    void SetPreviewMaterial(Material mat)
    {
        if (mat == null) return;
        foreach (Renderer rend in previewObject.GetComponentsInChildren<Renderer>())
            rend.material = mat;
    }
}