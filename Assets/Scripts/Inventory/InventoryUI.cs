using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public BaseInventoryData inventoryData;   // UnifiedInventoryData
    public Transform slotParent;              // Grid parent
    public GameObject slotPrefab;             // Slot prefab with InventorySlotUI + SlotRules

    [Header("Unified Layout")]
    public int backpackSlotCount = 20;
    public int relicSlotCount = 10;
    // Trash slot is always 1 (last index)

    [Header("Grid Layout")]
    public int columns = 5;

    private readonly List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();
    public IReadOnlyList<InventorySlotUI> UISlots => uiSlots;

    void Start()
    {
        ValidateCapacity();
        BuildSlots();
        RefreshAllSlots();
        WireNavigation();
    }

    private void ValidateCapacity()
    {
        int expected = backpackSlotCount + relicSlotCount + 1;
        if (inventoryData.capacity != expected)
        {
            Debug.LogError(
                $"UnifiedInventoryData capacity mismatch. Expected {expected}, got {inventoryData.capacity}"
            );
        }
    }

    public void BuildSlots()
    {
        // Clear old slots
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        uiSlots.Clear();

        // Create UI slots equal to unified inventory capacity
        for (int i = 0; i < inventoryData.capacity; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);

            // InventorySlotUI
            InventorySlotUI ui = obj.GetComponent<InventorySlotUI>();
            ui.Setup(i, inventoryData);
            uiSlots.Add(ui);

            // SlotRules
            SlotRules rules = obj.GetComponent<SlotRules>();
            if (rules == null)
                rules = obj.AddComponent<SlotRules>();

            if (i < backpackSlotCount)
                rules.slotType = SlotType.Backpack;
            else if (i < backpackSlotCount + relicSlotCount)
                rules.slotType = SlotType.Relic;
            else
                rules.slotType = SlotType.Trash;
        }
    }

    public void RefreshAllSlots()
    {
        for (int i = 0; i < uiSlots.Count; i++)
        {
            var dataSlot = inventoryData.slots[i];
            var uiSlot = uiSlots[i];

            if (dataSlot.occupied)
            {
                uiSlot.SetSlot(
                    dataSlot.item.icon,
                    dataSlot.count,
                    dataSlot.item.itemName
                );
            }
            else
            {
                uiSlot.ClearSlot();
            }
        }
    }

    public void WireNavigation()
    {
        int rows = Mathf.CeilToInt((float)uiSlots.Count / columns);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                if (index >= uiSlots.Count)
                    continue;

                var button = uiSlots[index].GetComponent<Button>();
                if (button == null)
                    continue;

                var nav = new Navigation { mode = Navigation.Mode.Explicit };

                // Up
                if (row > 0)
                {
                    int upIndex = (row - 1) * columns + col;
                    if (upIndex < uiSlots.Count)
                        nav.selectOnUp = uiSlots[upIndex].GetComponent<Button>();
                }

                // Down
                if (row < rows - 1)
                {
                    int downIndex = (row + 1) * columns + col;
                    if (downIndex < uiSlots.Count)
                        nav.selectOnDown = uiSlots[downIndex].GetComponent<Button>();
                }

                // Left
                if (col > 0)
                {
                    int leftIndex = row * columns + (col - 1);
                    if (leftIndex < uiSlots.Count)
                        nav.selectOnLeft = uiSlots[leftIndex].GetComponent<Button>();
                }

                // Right
                if (col < columns - 1)
                {
                    int rightIndex = row * columns + (col + 1);
                    if (rightIndex < uiSlots.Count)
                        nav.selectOnRight = uiSlots[rightIndex].GetComponent<Button>();
                }

                button.navigation = nav;
            }
        }
    }
}
