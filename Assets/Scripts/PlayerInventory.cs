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

    // Internal data
    private List<InventorySlot> slots = new List<InventorySlot>();
    private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();

    void Awake()
    {
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
            ui.SetSlot(null, 0); // start empty
        }
    }

    public void AddItem(MineableDrop drop)
    {
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
