using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;        // drag the Icon child here
    public TMP_Text countText; // drag the Count child here

    public string itemName;
    public int count;

    // Update slot with item data
    public void SetSlot(Sprite itemIcon, int newCount, string newName = "")
    {
        itemName = newName;
        count = newCount;

        icon.sprite = itemIcon;
        icon.enabled = itemIcon != null;
        countText.text = count > 1 ? "x" + count : "";
    }

    public void OnSlotClicked()
    {
        var pm = PauseManager.Instance;

        if (pm.currentMode == PauseManager.PauseMenuMode.Inventory &&
            pm.inventoryMode == PauseManager.InventoryMode.Navigation)
        {
            InventoryContextMenu.Instance.Open(this);
        }
        else if (pm.inventoryMode == PauseManager.InventoryMode.MovePending)
        {
            // Move logic is handled in PauseManager.OnSubmit
        }
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(itemName) || count <= 0;
    }

    public void ClearSlot()
    {
        Debug.Log($"[ClearSlot] {name} cleared. ItemName={itemName}, Count={count}");
        itemName = "";
        count = 0;
        icon.sprite = null;
        icon.enabled = false;
        countText.text = "";
    }


    public void Refresh()
    {
        countText.text = count > 1 ? "x" + count : "";
        icon.enabled = !string.IsNullOrEmpty(itemName);
    }
}
