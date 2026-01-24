/*using UnityEngine;
using UnityEngine.EventSystems;


public class InventoryController : MonoBehaviour
{
    [Header("Inventory Data + UI")]
    public BaseInventoryData inventoryData;
    public InventoryUI inventoryUI;

    [Header("Inventory Rules")]
    public bool acceptsItems = true;   // Backpack = true
    public bool acceptsRelics = false; // Relic inventory = true

    private bool isMoveMode = false;
    protected InventorySlotUI moveSourceSlot;
    

    public bool OwnsSlot(InventorySlotUI slot)
    {
        Debug.Log($"OwnsSlot? controller={inventoryData}, slot={slot.inventoryData}");
        return inventoryUI.UISlots.Contains(slot);
    }

    public void OnSubmit()
    {
        Debug.Log("Submit pressed");
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        var slot = selected.GetComponent<InventorySlotUI>();
        if (slot == null) return;

        // MOVE MODE
        if (isMoveMode)
        {
            TryPlaceItem(slot);
            return;
        }

        // NORMAL MODE
        if (!slot.IsEmpty())
        {
            StartMoveMode(slot);
        }
    }

    public void OnCancel()
    {
        if (isMoveMode)
        {
            CancelMoveMode();
        }
    }

    // -------------------------
    // MOVE MODE LOGIC
    // -------------------------

    public void StartMoveMode(InventorySlotUI sourceSlot)
    {
        isMoveMode = true;
        moveSourceSlot = sourceSlot;

        // Visual feedback
        sourceSlot.SetSelectedVisual(true);
    }

    public void CancelMoveMode()
    {
        if (moveSourceSlot != null)
            moveSourceSlot.SetSelectedVisual(false);

        moveSourceSlot = null;
        isMoveMode = false;
    }

    public virtual void TryPlaceItem(InventorySlotUI targetSlot)
    {
        Debug.Log($"TryPlaceItem called on {gameObject.name}");

        if (moveSourceSlot == null)
        {
            CancelMoveMode();
            return;
        }

        if (targetSlot == moveSourceSlot)
        {
            CancelMoveMode();
            return;
        }

        // Get the actual data backing the source slot
        var sourceData = moveSourceSlot.inventoryData.slots[moveSourceSlot.slotIndex];
        ItemDefinition def = sourceData.item;

        if (!CanAccept(def))
        {
            targetSlot.FlashInvalid();
            return;
        }

        BaseInventoryData.MoveItem(
            moveSourceSlot,
            targetSlot,
            inventoryData
        );

        inventoryUI.RefreshAllSlots();
        PauseManager.Instance.backpackInventoryController.inventoryUI.RefreshAllSlots();
        PauseManager.Instance.relicInventoryController.inventoryUI.RefreshAllSlots();

        CancelMoveMode();
    }


    public virtual bool CanAccept(ItemDefinition item)
    {
        if (item == null) 
            return false;

        // Relic
        if (item.itemType == ItemDefinition.ItemType.Relic)
            return acceptsRelics;

        // Normal item
        if (item.itemType == ItemDefinition.ItemType.Item)
            return acceptsItems;

        return false;
    }
    void OnSelect()
    {
        Debug.Log($"Controller active: {gameObject.name}");
    }




}*/
