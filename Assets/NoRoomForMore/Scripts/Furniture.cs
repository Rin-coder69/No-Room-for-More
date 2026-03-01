using UnityEngine;
using CGL.Inventory;

public class Furniture : Item
{
    [Header("Furniture Data")]
    [SerializeField] private ItemData itemData;

    [Header("Grid Properties")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int(2, 2);
    [SerializeField] private int rotationIndex = 0; // 0=0°, 1=90°, 2=180°, 3=270°

    public Vector2Int GridSize => rotationIndex % 2 == 0 ? gridSize : new Vector2Int(gridSize.y, gridSize.x);
    public int RotationIndex => rotationIndex;

    // Required by Item base class
    public override ItemData GetData() => itemData;

    public override bool IsReady() => true; // Furniture is always ready to place

    public override void Equip()
    {
        base.Equip();
        // When equipped, furniture is "held" but invisible
        // The placement system will show a ghost preview instead
        gameObject.SetActive(false); // Keep it hidden, show ghost preview elsewhere
    }

    public override void Unequip()
    {
        base.Unequip();
        // Hide ghost preview
    }

    public override void Use()
    {
        // This will be called when player clicks to place furniture
        // Placement logic will go here or in a separate FurniturePlacement script
        Debug.Log($"Attempting to place {itemData.displayName}");
    }

    public override void StopUse()
    {
        // Not needed for furniture placement
    }

    public override void OnAnimEventItemUse()
    {
        // Not needed for furniture
    }

    public void Rotate()
    {
        rotationIndex = (rotationIndex + 1) % 4;
    }
}