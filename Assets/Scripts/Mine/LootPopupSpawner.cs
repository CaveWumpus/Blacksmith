using UnityEngine;
using System.Collections.Generic;

public class LootPopupSpawner : MonoBehaviour
{
    public static LootPopupSpawner Instance;

    [System.Serializable]
    public class RarityColor
    {
        public RarityTier rarity;
        public Color color;
    }

    public List<RarityColor> rarityColors = new List<RarityColor>();

    public LootPopup popupPrefab;
    public Canvas popupCanvas; // Screen Space - Overlay canvas

    void Awake()
    {
        Instance = this;
    }

    public void Spawn(string itemName, int amount, Vector3 worldPos, RarityTier rarity)
    {
        Debug.Log($"[LootPopupSpawner] Spawn called for '{itemName}' x{amount}, rarity={rarity}");

        if (popupPrefab == null || popupCanvas == null)
            return;

        // Create popup under the canvas
        LootPopup popup = Instantiate(popupPrefab, popupCanvas.transform);

        // Convert world → screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // Convert screen → canvas local coordinates
        RectTransform canvasRect = popupCanvas.transform as RectTransform;
        RectTransform popupRect = popup.GetComponent<RectTransform>();

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null, // null because Overlay canvas ignores camera
            out anchoredPos
        );

        popupRect.anchoredPosition = anchoredPos;

        // Apply rarity color
        Color color = GetColorForRarity(rarity);

        // Initialize popup
        popup.Initialize($"+{amount} {itemName}", color);
    }

    public Color GetColorForRarity(RarityTier rarity)
    {
        foreach (var rc in rarityColors)
            if (rc.rarity == rarity)
                return rc.color;

        return Color.white;
    }
}
