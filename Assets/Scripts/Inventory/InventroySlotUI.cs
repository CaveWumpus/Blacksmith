using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image icon;
    public TMP_Text countText;
    public Image highlightImage;

    [Header("Slot Data")]
    public int slotIndex;
    public BaseInventoryData inventoryData;

    // Setup is called by InventoryUI when building the grid
    public void Setup(int index, BaseInventoryData data)
    {
        slotIndex = index;
        inventoryData = data;
        ClearSlot();
    }

    // Updates the visuals for a filled slot
    public void SetSlot(Sprite iconSprite, int count, string itemName)
    {
        icon.sprite = iconSprite;
        icon.enabled = true;

        countText.text = count > 1 ? count.ToString() : "";
    }

    // Clears the visuals for an empty slot
    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;

        countText.text = "";

        // Always disable highlight when empty
        highlightImage.enabled = false;
    }

    // Called by UnifiedInventoryController when entering/exiting move mode
    public void SetSelectedVisual(bool on)
    {
        highlightImage.enabled = on;
    }

    // Checks if the underlying data slot is empty
    public bool IsEmpty()
    {
        var dataSlot = inventoryData.slots[slotIndex];
        return !dataSlot.occupied;
    }

    // Flash red when the player tries an invalid move
    public void FlashInvalid()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        Color original = highlightImage.color;

        highlightImage.color = Color.red;
        highlightImage.enabled = true;

        yield return new WaitForSeconds(0.15f);

        highlightImage.enabled = false;
        highlightImage.color = original;
    }

    // Optional: useful for debugging navigation
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log($"Slot selected: {slotIndex}");
    }
}
