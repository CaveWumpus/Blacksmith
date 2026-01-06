using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;        // drag the Icon child here
    public TMP_Text countText; // drag the Count child here

    // Called when updating the slot with item data
    public void SetSlot(Sprite itemIcon, int count)
    {
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
}
