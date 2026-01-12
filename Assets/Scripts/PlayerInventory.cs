using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Player Inventory Settings")]
    public int slotCount = 20;   // adjustable in Inspector
    public int columns = 5;      // adjustable in Inspector

    [Header("UI References")]
    public Transform inventoryPanel;   // drag PlayerInventoryPanel here
    public GameObject slotPrefab;      // drag PlayerSlotPrefab here

    public List<ItemStack> oreStacks = new List<ItemStack>(); // each type of ore
    public List<ItemStack> gemStacks = new List<ItemStack>(); // each type of gem

    [System.Serializable]
    public class ItemStack
    {
        public string itemName;
        public int count;
    }

    private List<PlayerSlot> slots = new List<PlayerSlot>();
    private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();
    public List<InventorySlotUI> UISlots => uiSlots;

    [Header("Stack Settings")]
    public int maxStackSize = 20;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize empty slots
        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new PlayerSlot());

            GameObject slotObj = Instantiate(slotPrefab, inventoryPanel);
            InventorySlotUI ui = slotObj.GetComponent<InventorySlotUI>();
            uiSlots.Add(ui);
            ui.ClearSlot(); // start empty
        }
    }

    void Start()
    {
        int rows = Mathf.CeilToInt((float)slotCount / columns);
        WireSlotNavigation(rows, columns);
    }

    public void AddItem(Sprite icon, string itemName, int count = 1)
    {
        int remaining = count;

        // Try to stack into existing slots
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].occupied && slots[i].itemName == itemName && slots[i].count < maxStackSize)
            {
                int space = maxStackSize - slots[i].count;
                int toAdd = Mathf.Min(space, remaining);

                slots[i].count += toAdd;
                uiSlots[i].SetSlot(icon, slots[i].count, itemName);
                remaining -= toAdd;

                UpdateSummaryStacks(itemName, toAdd);

                if (remaining <= 0) return;
            }
        }

        // Place into empty slots
        for (int i = 0; i < slots.Count && remaining > 0; i++)
        {
            if (!slots[i].occupied)
            {
                int toAdd = Mathf.Min(maxStackSize, remaining);

                slots[i].itemName = itemName;
                slots[i].count = toAdd;
                slots[i].occupied = true;

                uiSlots[i].SetSlot(icon, toAdd, itemName);
                remaining -= toAdd;

                UpdateSummaryStacks(itemName, toAdd);
            }
        }
    }

    public void RemoveItem(string itemName, int count)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].occupied && slots[i].itemName == itemName)
            {
                slots[i].occupied = false;
                slots[i].itemName = "";
                slots[i].count = 0;
                uiSlots[i].ClearSlot();

                UpdateSummaryStacks(itemName, -count);
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

        if (!toSlot.occupied)
        {
            toSlot.itemName = fromSlot.itemName;
            toSlot.count = fromSlot.count;
            toSlot.occupied = true;

            uiSlots[toIndex].SetSlot(from.icon.sprite, fromSlot.count, fromSlot.itemName);

            fromSlot.itemName = "";
            fromSlot.count = 0;
            fromSlot.occupied = false;
            from.ClearSlot();
        }
        else
        {
            string tempName = toSlot.itemName;
            int tempCount = toSlot.count;
            Sprite tempIcon = to.icon.sprite;

            toSlot.itemName = fromSlot.itemName;
            toSlot.count = fromSlot.count;
            uiSlots[toIndex].SetSlot(from.icon.sprite, fromSlot.count, fromSlot.itemName);

            fromSlot.itemName = tempName;
            fromSlot.count = tempCount;
            uiSlots[fromIndex].SetSlot(tempIcon, tempCount, tempName);
        }

        // Totals don’t change on move, so no UpdateSummaryStacks needed here
    }

    private void UpdateSummaryStacks(string itemName, int amount)
    {
        bool isOre = itemName.ToLower().Contains("ore");
        List<ItemStack> targetList = isOre ? oreStacks : gemStacks;

        var stack = targetList.Find(s => s.itemName == itemName);
        if (stack != null)
        {
            stack.count += amount;
            if (stack.count <= 0)
            {
                targetList.Remove(stack);
            }
        }
        else if (amount > 0)
        {
            targetList.Add(new ItemStack { itemName = itemName, count = amount });
        }

        // Debug log to confirm updates
        Debug.Log($"Updated {itemName}: {amount}. OreStacks={string.Join(", ", oreStacks.ConvertAll(s => $"{s.itemName}:{s.count}"))}, GemStacks={string.Join(", ", gemStacks.ConvertAll(s => $"{s.itemName}:{s.count}"))}");
    }

    public void WireSlotNavigation(int rows, int cols)
    {
        if (uiSlots == null || uiSlots.Count == 0) return;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                if (index >= uiSlots.Count) continue;

                var slotUI = uiSlots[index];
                if (slotUI == null) continue;

                var button = uiSlots[index].GetComponent<Button>();
                if (button == null) continue;

                var nav = new Navigation { mode = Navigation.Mode.Explicit };

                if (row > 0 && (row - 1) * cols + col < uiSlots.Count)
                {
                    var neighbor = uiSlots[(row - 1) * cols + col];
                    if (neighbor != null)
                        nav.selectOnUp = neighbor.GetComponent<Button>();
                }
                if (row < rows - 1 && (row + 1) * cols + col < uiSlots.Count)
                {
                    var neighbor = uiSlots[(row + 1) * cols + col];
                    if (neighbor != null)
                        nav.selectOnDown = neighbor.GetComponent<Button>();
                }

                if (col > 0 && row * cols + (col - 1) < uiSlots.Count)
                {
                    var neighbor = uiSlots[row * cols + (col - 1)];
                    if (neighbor != null)
                        nav.selectOnLeft = neighbor.GetComponent<Button>();
                }

                if (col < cols - 1 && row * cols + (col + 1) < uiSlots.Count)
                {
                    var neighbor = uiSlots[row * cols + (col + 1)];
                    if (neighbor != null)
                        nav.selectOnRight = neighbor.GetComponent<Button>();
                }

                button.navigation = nav;
            }
        }
    }
}

[System.Serializable]
public class PlayerSlot
{
    public string itemName;
    public int count;
    public bool occupied;
}
