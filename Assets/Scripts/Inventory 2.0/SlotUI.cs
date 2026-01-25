using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    [Header("References")]
    public Image background;
    public Image rarityBar;
    public Image icon;
    public TextMeshProUGUI stackCountText;
    public TextMeshProUGUI itemNameText;
    public CanvasGroup smartDropGlowGroup;
    public CanvasGroup selectionFrameGroup;
    public CanvasGroup rootCanvasGroup;
    public LayoutElement layoutElement;


    private InventoryItemData data;

    // ---------------------------------------------------------
    // Public API
    // ---------------------------------------------------------
    public void SetData(InventoryItemData newData)
    {
        data = newData;

        icon.sprite = data.icon;
        itemNameText.text = data.itemName;

        // Stack count
        if (data.currentStack > 1)
        {
            stackCountText.text = "x" + data.currentStack;
            stackCountText.enabled = true;
        }
        else
        {
            stackCountText.enabled = false;
        }

        // Rarity color
        rarityBar.color = GetRarityColor(data.rarity);
    }

    public void SetSelected(bool selected)
    {
        selectionFrameGroup.alpha = selected ? 1f : 0f;

        // Slight scale bump
        transform.localScale = selected ? Vector3.one * 1.05f : Vector3.one;
    }

    public void SetSmartDrop(bool enabled)
    {
        smartDropGlowGroup.alpha = enabled ? 1f : 0f;
    }

    public void SetDisabled(bool disabled)
    {
        rootCanvasGroup.alpha = disabled ? 0.4f : 1f;
    }

    // ---------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------
    private Color GetRarityColor(RarityTier rarity)
    {
        switch (rarity)
        {
            case RarityTier.Regular: return new Color(0.8f, 0.8f, 0.8f);
            case RarityTier.Rare: return new Color(0.2f, 0.4f, 1f);
            case RarityTier.Exotic: return new Color(1f, 0.5f, 0f);
        }
        return Color.white;
    }
    public void SetHUDSize()
    {
        layoutElement.preferredWidth = 230;
        layoutElement.preferredHeight = 40;
    }

    public void SetDropModeSize()
    {
        layoutElement.preferredWidth = 420;
        layoutElement.preferredHeight = 60;
    }


}
