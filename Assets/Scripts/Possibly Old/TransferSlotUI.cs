using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransferSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image icon;
    public TMP_Text itemNameText;
    public TMP_Text itemCountText;

    [Header("Data")]
    public string itemName;
    public int count;

    // Called by TransferPanelManager to fill the slot
    public void SetSlot(string name, int amount, Sprite sprite = null)
    {
        itemName = name;
        count = amount;

        itemNameText.text = name;
        itemCountText.text = amount.ToString();

        if (sprite != null) icon.sprite = sprite;
        else icon.sprite = null;
    }

    // Called by TransferPanelManager to clear the slot
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

        // Move item into persistent chest
        ShopStorageChest.Instance.AddItem(itemName, count);

        // Clear this slot visually
        ClearSlot();
        gameObject.SetActive(false);
    }
}
