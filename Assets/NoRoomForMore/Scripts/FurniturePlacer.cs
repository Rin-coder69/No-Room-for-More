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

        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, floorLayer))
        {
            GridManager targetGrid = GetGridAtPosition(hit.point);
            if (targetGrid == null) return;
            previewPosition = hit.point;
            previewPosition.y = 0;

            Vector3 snappedPos = targetGrid.SnapToGrid(previewPosition);
            previewObject.transform.position = snappedPos;
            previewObject.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            Vector2Int gridPos = targetGrid.WorldToGrid(snappedPos);
            bool canPlace = targetGrid.CanPlaceFurniture(gridPos, furnitureSize);
            SetPreviewMaterial(canPlace ? ValidPlacementMatieral : InvalidPlacementMaterial);
        }
    }

    private GridManager GetGridAtPosition(Vector3 worldPos)
    {
        foreach (GridManager grid in gridManagers)
        {
            Vector2Int gridCoord = grid.WorldToGrid(worldPos);

            // Check if this coordinate is within this grid's bounds
            if (gridCoord.x >= 0 && gridCoord.x < grid.gridWidth &&
                gridCoord.y >= 0 && gridCoord.y < grid.gridHeight)
            {
                return grid;
            }
        }

        return gridManagers.Count > 0 ? gridManagers[0] : null; // Fallback
    }

    void TryPlaceFurniture()
    {
        Vector3 placementPos = previewObject.transform.position;

        GridManager targetGrid = GetGridAtPosition(placementPos);
        if (targetGrid == null) return;

        Vector2Int gridPos = targetGrid.WorldToGrid(placementPos);
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

            targetGrid.OccupyTiles(gridPos, furnitureSize);

            if (ScoreManager.Instance)
            {
                ScoreManager.Instance.AddScore(10);
            }
            else
            {
                Debug.Log("No scoremanager was found.");
            }


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