using UnityEngine;
using System.Collections.Generic;

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
    
}

[System.Serializable]
public class InventorySlot
{
    public string itemName;
    public int count;
}
