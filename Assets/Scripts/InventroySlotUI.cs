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
        // Open a contextual menu for this slot
        InventoryContextMenu.Instance.Open(this);
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
