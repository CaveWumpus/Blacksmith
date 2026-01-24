/*using UnityEngine;
using UnityEngine.EventSystems;

public class UnifiedInventoryController : MonoBehaviour
{
    public InventoryUI inventoryUI;
    public BaseInventoryData inventoryData;
    public static UnifiedInventoryController Instance;

    private InventorySlotUI moveSourceSlot;
    private bool isInMoveMode = false;

    public void OnSubmit()
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        var slot = selected.GetComponent<InventorySlotUI>();
        if (slot == null) return;

        // Start move mode
        if (!isInMoveMode)
        {
            if (slot.IsEmpty()) return;

            moveSourceSlot = slot;
            isInMoveMode = true;
            moveSourceSlot.SetSelectedVisual(true);
            return;
        }

        // Place / swap / delete
        TryPlaceItem(slot);
    }
    void Awake()
    {
        Instance = this;
    }


    public void OnCancel()
    {
        CancelMoveMode();
    }

    private void TryPlaceItem(InventorySlotUI targetSlot)
    {
        if (moveSourceSlot == null)
        {
            CancelMoveMode();
            return;
        }

        var sourceData = inventoryData.slots[moveSourceSlot.slotIndex];
        var targetData = inventoryData.slots[targetSlot.slotIndex];

        // Try to get SlotRules safely
        SlotRules rules = null;
        if (inventoryUI != null && inventoryUI.UISlots.Count > targetSlot.slotIndex)
            rules = inventoryUI.UISlots[targetSlot.slotIndex].GetComponent<SlotRules>();

        // If UI is missing or rules are missing, treat slot as General
        bool isTrash = rules != null && rules.slotType == SlotType.Trash;
        bool acceptsItem = rules == null || rules.Accepts(sourceData.item);

        // Trash behavior
        if (isTrash)
        {
            sourceData.Clear();

            if (inventoryUI != null)
                inventoryUI.RefreshAllSlots();

            CancelMoveMode();
            return;
        }

        // Validate slot acceptance
        if (!acceptsItem)
        {
            targetSlot.FlashInvalid();
            return;
        }

        // Swap or move
        bool targetOccupied = targetData.occupied;

        if (!targetOccupied)
        {
            // Move item
            targetData.item = sourceData.item;
            targetData.count = sourceData.count;
            sourceData.Clear();
        }
        else
        {
            // Swap items
            var tempItem = targetData.item;
            var tempCount = targetData.count;

            targetData.item = sourceData.item;
            targetData.count = sourceData.count;

            sourceData.item = tempItem;
            sourceData.count = tempCount;
        }

        if (inventoryUI != null)
            inventoryUI.RefreshAllSlots();

        CancelMoveMode();
    }

    public bool TryAddItem(ItemDefinition item, int amount)
    {
        int remaining = amount;

        // -----------------------------
        // 1. Fill existing stacks first
        // -----------------------------
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];

            // Get SlotRules safely
            SlotRules rules = null;
            if (inventoryUI != null && inventoryUI.UISlots.Count > i)
                rules = inventoryUI.UISlots[i].GetComponent<SlotRules>();

            // Trash slots never accept items
            if (rules != null && rules.slotType == SlotType.Trash)
                continue;

            // Relic slots only accept relics
            if (rules != null && rules.slotType == SlotType.Relic)
                continue;

            // Only stack into matching stacks
            if (!slot.occupied || slot.item != item)
                continue;

            // If stack is full, skip
            if (slot.count >= item.maxStack)
                continue;

            int space = item.maxStack - slot.count;
            int toAdd = Mathf.Min(space, remaining);

            slot.count += toAdd;
            remaining -= toAdd;

            if (remaining <= 0)
            {
                if (inventoryUI != null)
                    inventoryUI.RefreshAllSlots();
                return true;
            }
        }

        // -----------------------------
        // 2. Create new stacks
        // -----------------------------
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];

            SlotRules rules = null;
            if (inventoryUI != null && inventoryUI.UISlots.Count > i)
                rules = inventoryUI.UISlots[i].GetComponent<SlotRules>();

            if (rules != null && rules.slotType == SlotType.Trash)
                continue;

            if (rules != null && rules.slotType == SlotType.Relic)
                continue;

            if (slot.occupied)
                continue;

            int toAdd = Mathf.Min(item.maxStack, remaining);

            slot.item = item;
            slot.count = toAdd;

            remaining -= toAdd;

            if (remaining <= 0)
            {
                if (inventoryUI != null)
                    inventoryUI.RefreshAllSlots();
                return true;
            }
        }

        // -----------------------------
        // 3. Not enough space
        // -----------------------------
        if (inventoryUI != null)
            inventoryUI.RefreshAllSlots();

        return false; // leftover items remain
    }


    public bool TryAddRelic(ItemDefinition relicItem)
    {

        // -----------------------------
        // 1. Try relic-only slots first
        // -----------------------------
        for (int i = 0; i < inventoryData.slots.Length; i++)
        {
            var slot = inventoryData.slots[i];

            SlotRules rules = null;
            if (inventoryUI != null && inventoryUI.UISlots.Count > i)
                rules = inventoryUI.UISlots[i].GetComponent<SlotRules>();

            if (rules == null || rules.slotType != SlotType.Relic)
                continue;

            if (slot.occupied)
                continue;

            slot.item = relicItem;
            slot.count = 1;

            if (inventoryUI != null)
                inventoryUI.RefreshAllSlots();

            return true;
        }

        // -----------------------------
        // 2. Fall back to general slots
        // -----------------------------
        return TryAddItem(relicItem, 1);
    }


    public bool TryAddDrop(DropResult drop)
    {
        switch (drop.dropType)
        {
            case TileType.Ore:
            case TileType.Gem:
                return TryAddItem(drop.vein.item, 1);

            case TileType.Relic:
                return TryAddRelic(drop.relic.item);

            case TileType.Recipe:
                RecipeManager.Instance.UnlockRecipe(drop.recipe);
                return true;

            case TileType.Pattern:
                PatternManager.Instance.UnlockPattern(drop.pattern);
                return true;

            default:
                return false;
        }
    }

    private void CancelMoveMode()
    {
        if (moveSourceSlot != null)
            moveSourceSlot.SetSelectedVisual(false);

        moveSourceSlot = null;
        isInMoveMode = false;
    }
}
*/