using CGL.Inventory;
using UnityEngine;
using UnityEngine.InputSystem; 


public class FurniturePlacer : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private LimitedInventory inventory;
    [SerializeField] private InventoryUI inventoryUI;


    [Header("Furniture Settings")]
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private Material ValidPlacementMatieral;
    [SerializeField] private Material InvalidPlacementMaterial;

    private GameObject selectedFurniture;
    private GameObject previewObject;
    private Vector2Int furnitureSize; // Size in grid tiles 
    private bool isPlacing = false;

   void Update()
    {
        if(isPlacing && previewObject != null)
        {
            UpdatePreview();
            if(Mouse.current.leftButton.wasPressedThisFrame)
            {
                TryPlaceFurniture();
            }

            if(Mouse.current.rightButton.wasPressedThisFrame)
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
        // Create preview object

        previewObject = Instantiate(selectedFurniture);
        DisablePreviewPhysics(previewObject);
        isPlacing = true;
    }

    void UpdatePreview()
    {
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(Physics.Raycast(ray, out RaycastHit hit, 100f,floorLayer))
        {
            Vector3 snappedPos = gridManager.SnapToGrid(hit.point);
            previewObject.transform.position = snappedPos;

            Vector2Int gridPos = gridManager.WorldToGrid(snappedPos);
            bool canPlace = gridManager.CanPlaceFurniture(gridPos, furnitureSize);

            SetPreviewMaterial(canPlace ? ValidPlacementMatieral : InvalidPlacementMaterial);
        }
    }


    void TryPlaceFurniture()
    {
        Vector3 placementPos = previewObject.transform.position;
        Vector2Int gridPos = gridManager.WorldToGrid(placementPos);

        if (gridManager.CanPlaceFurniture(gridPos, furnitureSize))
        {
            // Place the real furniture
            GameObject placed = Instantiate(selectedFurniture, placementPos, Quaternion.identity);

            // Freeze it in place
            Rigidbody rb = placed.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // Mark tiles as occupied in the grid
            gridManager.OccupyTiles(gridPos, furnitureSize);

            // Remove item from inventory
            Item item = inventory.CurrentItem;
            inventory.RemoveItem(item);

            // Refresh the UI
            inventoryUI.RefreshUI();

            // Clean up
            Destroy(previewObject);
            isPlacing = false;
            selectedFurniture = null;
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
    }

    void DisablePreviewPhysics(GameObject obj)
    {
        // Turn off physics on the preview ghost
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Turn off colliders on the preview ghost
        foreach (Collider col in obj.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }

    void SetPreviewMaterial(Material mat)
    {
        if (mat == null) return;
        foreach (Renderer rend in previewObject.GetComponentsInChildren<Renderer>())
        {
            rend.material = mat;
        }
    }
}
