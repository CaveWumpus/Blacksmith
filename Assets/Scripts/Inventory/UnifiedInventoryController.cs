using UnityEngine;
using UnityEngine.EventSystems;

public class UnifiedInventoryController : MonoBehaviour
{
    public InventoryUI inventoryUI;
    public BaseInventoryData inventoryData;

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

        var rules = targetSlot.GetComponent<SlotRules>();
        if (rules == null)
        {
            targetSlot.FlashInvalid();
            return;
        }

        // Trash behavior
        if (rules.slotType == SlotType.Trash)
        {
            sourceData.Clear();
            inventoryUI.RefreshAllSlots();
            CancelMoveMode();
            return;
        }

        // Validate
        if (!rules.Accepts(sourceData.item))
        {
            targetSlot.FlashInvalid();
            return;
        }

        // Swap or move
        bool targetOccupied = targetData.occupied;

        if (!targetOccupied)
        {
            targetData.item = sourceData.item;
            targetData.count = sourceData.count;
            sourceData.Clear();
        }
        else
        {
            var tempItem = targetData.item;
            var tempCount = targetData.count;

            targetData.item = sourceData.item;
            targetData.count = sourceData.count;

            sourceData.item = tempItem;
            sourceData.count = tempCount;
        }

        inventoryUI.RefreshAllSlots();
        CancelMoveMode();
    }

    private void CancelMoveMode()
    {
        if (moveSourceSlot != null)
            moveSourceSlot.SetSelectedVisual(false);

        moveSourceSlot = null;
        isInMoveMode = false;
    }
}
