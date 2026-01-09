using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;        // drag the Icon child here
    public TMP_Text countText; // drag the Count child here

    // Add these so context menu can read them
    public string itemName;
    public int count;

    // Called when updating the slot with item data
    public void SetSlot(Sprite itemIcon, int newCount, string newName = "")
    {
        itemName = newName;
        count = newCount;
        
        if (itemIcon != null)
        {
            icon.sprite = itemIcon;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }

        countText.text = count > 1 ? "x" + count : "";
    }

    public void OnSlotClicked()
    {
        var pm = PauseManager.Instance;

        if (pm.currentMode == PauseManager.PauseMenuMode.Inventory)
        {
            if (pm.inventoryMode == PauseManager.InventoryMode.Navigation)
            {
                // Open context menu
                InventoryContextMenu.Instance.Open(this);
                pm.inventoryMode = PauseManager.InventoryMode.ContextMenu;
            }
        
            else if (pm.inventoryMode == PauseManager.InventoryMode.MovePending)
            {
                // Perform move: swap if occupied, otherwise place
                PlayerInventory.Instance.MoveItem(pm.moveSourceSlot, this);
                pm.moveSourceSlot = null;
                pm.inventoryMode = PauseManager.InventoryMode.Navigation;
            }
        }
    }
    // ✅ Clear slot completely
    public void ClearSlot()
    {
        itemName = "";
        count = 0;
        icon.sprite = null;
        icon.enabled = false;
        countText.text = "";
    }
}
