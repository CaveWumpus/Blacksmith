using UnityEngine;
using UnityEngine.UI;

public class MiningInventorySlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image icon;
    [SerializeField] private Image rarityBorder;
    [SerializeField] private Image orderMarker;

    [Header("Highlight Layers")]
    [SerializeField] private Image selectedHighlight;
    [SerializeField] private Image smartDropHighlight;
    [SerializeField] private Image dropModeDimmer;

    private bool isOrderRelevant = false;

    // ---------------------------------------------------------
    // Item Assignment
    // ---------------------------------------------------------
    public void SetItem(InventoryItemData data)
    {
        icon.enabled = true;
        icon.sprite = data.icon;

        rarityBorder.color = GetRarityColor(data.rarity);

        isOrderRelevant = data.isOrderRelevant;
        orderMarker.enabled = isOrderRelevant;
    }

    public void Clear()
    {
        icon.enabled = false;
        orderMarker.enabled = false;
        rarityBorder.color = Color.clear;

        selectedHighlight.enabled = false;
        smartDropHighlight.enabled = false;
        dropModeDimmer.enabled = false;
    }

    // ---------------------------------------------------------
    // Drop Mode Visuals
    // ---------------------------------------------------------
    public void SetDropModeActive(bool active)
    {
        dropModeDimmer.enabled = active;
    }

    public void SetSelected(bool selected)
    {
        selectedHighlight.enabled = selected;
    }

    public void SetSmartDropHighlight(bool active)
    {
        // Never highlight order items as junk
        if (isOrderRelevant)
        {
            smartDropHighlight.enabled = false;
            return;
        }

        smartDropHighlight.enabled = active;
    }

    // ---------------------------------------------------------
    // Rarity Colors
    // ---------------------------------------------------------
    private Color GetRarityColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return new Color(0.8f, 0.8f, 0.8f);
            case ItemRarity.Uncommon: return new Color(0.2f, 1f, 0.2f);
            case ItemRarity.Rare: return new Color(0.2f, 0.4f, 1f);
            case ItemRarity.Epic: return new Color(0.7f, 0.2f, 1f);
            case ItemRarity.Legendary: return new Color(1f, 0.5f, 0f);

            default: return Color.white;
        }
    }
}
