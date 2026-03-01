using UnityEngine;
using UnityEngine.UI;
using CGL.Inventory;
using System.Collections;

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
    }

    IEnumerator DelayedRefresh()
    {
        // wait one frame for inventory to initialize
        yield return null;
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

            Image icon = slot.GetComponentInChildren<Image>();
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