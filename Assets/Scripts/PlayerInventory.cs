using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Backpack Settings")]
    public int slotCount = 24;        // adjustable in Inspector
    public int maxStackSize = 20;     // adjustable in Inspector

    [Header("UI References")]
    public Transform backpackPanel;   // drag BackpackPanel here
    public GameObject inventorySlotPrefab; // drag InventorySlotPrefab here

    private List<InventorySlot> slots = new List<InventorySlot>();
    private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();
    public IReadOnlyList<InventorySlotUI> UISlots => uiSlots;

    public int GetEmptySlotIndex()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (string.IsNullOrEmpty(slots[i].itemName))
                return i;
        }
        return -1; // no empty slot
    }
    
    void Awake()
    {
        Debug.Log($"[PlayerInventory] Instantiating {slotCount} slots into {backpackPanel.name}");

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize empty slots
        for (int i = 0; i < slotCount; i++)
            slots.Add(new InventorySlot());

        // Instantiate UI slots
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, backpackPanel);
            InventorySlotUI ui = slotObj.GetComponent<InventorySlotUI>();
            uiSlots.Add(ui);
            ui.ClearSlot(); // start empty
        }

        // ✅ Wire navigation after slots exist
        int rows = slotCount / 6; // assuming 6 columns
        WireSlotNavigation(rows, cols: 6); // 4 rows × 6 columns = 24 slots
    }

    public void AddItem(MineableDrop drop)
    {
        Debug.Log($"[Inventory] Adding {drop.dropName} to backpack");

        if (drop == null) return;

        
        // Try to find existing stack
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemName == drop.dropName && slots[i].count < maxStackSize)
            {
                slots[i].count++;
                UpdateUISlot(i, drop.icon, slots[i].count);
                return;
            }
        }

        // Try to find empty slot
        for (int i = 0; i < slots.Count; i++)
        {
            if (string.IsNullOrEmpty(slots[i].itemName))
            {
                slots[i].itemName = drop.dropName;
                slots[i].count = 1;
                UpdateUISlot(i, drop.icon, slots[i].count);
                return;
            }
        }

        Debug.Log("[Inventory] Backpack full!");
    }

    public void AddItem(MineableDrop drop, int amount)
    {
        Debug.Log($"[Inventory] Adding {amount}x {drop.dropName} to backpack");

        if (drop == null) return;

        for (int i = 0; i < amount; i++)
        {
    
            AddItem(drop); // call the single‑item version
        }
    }
    
    // ✅ Remove items
    public void RemoveItem(string itemName, int amount = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemName == itemName)
            {
                slots[i].count -= amount;
                if (slots[i].count <= 0)
                {
                    slots[i].itemName = "";
                    slots[i].count = 0;
                    uiSlots[i].ClearSlot();
                }
                else
                {
                    UpdateUISlot(i, uiSlots[i].icon.sprite, slots[i].count);
                }
                return;
            }
        }
    }

    public void MoveItem(InventorySlotUI from, InventorySlotUI to)
    {
        int fromIndex = uiSlots.IndexOf(from);
        int toIndex = uiSlots.IndexOf(to);

        if (fromIndex == -1 || toIndex == -1) return;
        var fromSlot = slots[fromIndex];
        var toSlot = slots[toIndex];

        if (string.IsNullOrEmpty(toSlot.itemName))
        {
            // Move into empty slot
            toSlot.itemName = fromSlot.itemName;
            toSlot.count = fromSlot.count;
            UpdateUISlot(toIndex, from.icon.sprite, toSlot.count);

            // Clear source
            fromSlot.itemName = "";
            fromSlot.count = 0;
            from.ClearSlot();
        }
        else
        {
            // Swap items
            string tempName = toSlot.itemName;
            int tempCount = toSlot.count;
            Sprite tempIcon = to.icon.sprite;

            toSlot.itemName = fromSlot.itemName;
            toSlot.count = fromSlot.count;
            UpdateUISlot(toIndex, from.icon.sprite, toSlot.count);

            fromSlot.itemName = tempName;
            fromSlot.count = tempCount;
            UpdateUISlot(fromIndex, tempIcon, fromSlot.count);
        }
    }

    // ✅ Query item count
    public int GetItemCount(string itemName)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemName == itemName)
                return slots[i].count;
        }
        return 0;
    }

    private void UpdateUISlot(int index, Sprite icon, int count)
    {
        uiSlots[index].SetSlot(icon, count);
    }
    public void WireSlotNavigation(int rows, int cols)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                var button = uiSlots[index].GetComponent<Button>();
                var nav = new Navigation { mode = Navigation.Mode.Explicit };
                // Up
                if (row > 0)
                    nav.selectOnUp = uiSlots[(row - 1) * cols + col].GetComponent<Button>();

                // Down
                if (row < rows - 1)
                    nav.selectOnDown = uiSlots[(row + 1) * cols + col].GetComponent<Button>();

                // Left
                if (col > 0)
                    nav.selectOnLeft = uiSlots[row * cols + (col - 1)].GetComponent<Button>();

                // Right
                if (col < cols - 1)
                    nav.selectOnRight = uiSlots[row * cols + (col + 1)].GetComponent<Button>();

                button.navigation = nav;
            }
        }
    }
}    
[System.Serializable]
public class InventorySlot
{
    public string itemName;
    public int count;
}
