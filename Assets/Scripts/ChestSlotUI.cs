using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image icon;
    public TMP_Text itemNameText;
    public TMP_Text itemCountText;

    [Header("Data")]
    public string itemName;
    public int count;

    // Called by ChestPanelManager to fill the slot
    public void SetSlot(string name, int amount, Sprite sprite = null)
    {
        itemName = name;
        count = amount;

        itemNameText.text = name;
        itemCountText.text = amount.ToString();

        if (sprite != null) icon.sprite = sprite;
        else icon.sprite = null;
    }

    // Called by ChestPanelManager to clear the slot
    public void ClearSlot()
    {
        itemName = "";
        count = 0;
        itemNameText.text = "";
        itemCountText.text = "";
        icon.sprite = null;
    }

    // Optional: what happens when player presses A on this slot
    public void OnSubmit()
    {
        if (string.IsNullOrEmpty(itemName)) return;

        Debug.Log($"Chest slot selected: {itemName} x{count}");
        // You could add logic here to move items back to inventory or open details
    }
}
