using CGL.Inventory;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LimitedInventory inventory;
    [SerializeField] private FurniturePlacer furniturePlacer;

    [Header("UI Settings")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;

    void Start()
    {
        StartCoroutine(DelayedRefresh());
        inventory.onInventoryChanged.AddListener(RefreshUI);
    }

    void Update()
    {
        if (furniturePlacer.isPlacing) return;
        // Hotkeys 1-5 to select inventory slots
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SelectSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SelectSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SelectSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            SelectSlot(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
            SelectSlot(4);
    }
    void SelectSlot(int index)
    {
        if (index >= inventory.ItemCount) return; // No item in this slot

        Item item = inventory.GetItem(index);
        if (item == null) return;

        ItemData data = item.GetData();
        if (data == null) return;

        // Start placing (same as clicking the button)
        furniturePlacer.StartPlacing(data.itemPrefab, data.furnitureSize);
    }
    IEnumerator DelayedRefresh()
    {
        yield return null;
        if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned in InventoryUI!");
            yield break;
        }
        Debug.Log("Delayed refresh, item count: " + inventory.ItemCount);
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < inventory.ItemCount; i++)
        {
            int index = i;
            Item item = inventory.GetItem(i);
            ItemData data = item.GetData();

            if (data == null) continue;

            GameObject slot = Instantiate(slotPrefab, slotContainer);

            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            Debug.Log("Icon found: " + (icon != null) + " | Sprite assigned: " + (data.icon != null));
            if (icon != null && data.icon != null)
                icon.sprite = data.icon;

            Text label = slot.GetComponentInChildren<Text>();
            if (label != null)
                label.text = data.displayName;

            Button button = slot.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    furniturePlacer.StartPlacing(data.itemPrefab, data.furnitureSize);
                });
            }
        }
    }
}