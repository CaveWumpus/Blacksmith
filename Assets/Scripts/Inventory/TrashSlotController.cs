/*using UnityEngine;

public class TrashSlotController : InventoryController
{
    public override void TryPlaceItem(InventorySlotUI targetSlot)
    {
        if (moveSourceSlot == null)
        {
            CancelMoveMode();
            return;
        }

        // Delete the item
        moveSourceSlot.ClearSlot();

        // Refresh the UI for the inventory the item came from
        inventoryUI.RefreshAllSlots();

        // Also refresh the other inventory (backpack or relic)
        PauseManager.Instance.backpackInventoryController.inventoryUI.RefreshAllSlots();
        PauseManager.Instance.relicInventoryController.inventoryUI.RefreshAllSlots();

        CancelMoveMode();
    }

    public override bool CanAccept(ItemDefinition item)
    {
        // Trash accepts everything
        return true;
    }
}*/
