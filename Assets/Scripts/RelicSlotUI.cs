using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;

    public string relicName;   // <-- add this
    public bool occupied;      // optional mirror of RelicSlot

    public void SetRelic(Sprite relicIcon, string newName)
    {
        relicName = newName;
        occupied = true;

        if (relicIcon != null)
        {
            icon.sprite = relicIcon;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }

        nameText.text = newName;
    }

    public void ClearSlot()
    {
        relicName = "";
        occupied = false;
        icon.sprite = null;
        icon.enabled = false;
        nameText.text = "";
    }

    public void OnSlotClicked()
    {
        var pm = PauseManager.Instance;

        if (pm.currentMode == PauseManager.PauseMenuMode.Inventory)
        {
            if (pm.inventoryMode == PauseManager.InventoryMode.Navigation)
            {
                RelicContextMenu.Instance.Open(this);
                pm.inventoryMode = PauseManager.InventoryMode.ContextMenu;
            }
            else if (pm.inventoryMode == PauseManager.InventoryMode.MovePending)
            {
                RelicInventory.Instance.MoveItem(pm.moveSourceRelicSlot, this);
                pm.moveSourceRelicSlot = null;
                pm.inventoryMode = PauseManager.InventoryMode.Navigation;
            }
        }
    }
}
