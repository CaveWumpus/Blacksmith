using UnityEngine;

public enum RelicBiome
{
    Universal,
    Fire,
    Ice,
    Ruins,
    Crystal,
    Forest,
    Desert
}

[CreateAssetMenu(menuName = "Relics/Relic Definition")]
public class RelicDefinition : TileDefinition
{
    [Header("Relic Identity")]
    public string relicID;
    public string relicName;
    public string description;
    public Sprite icon;

    [Header("Biome Drop Settings")]
    public RelicBiome biome = RelicBiome.Universal;

    [Header("Relic Categories")]
    public bool isMiningRelic;
    public bool isShopRelic;

    [Header("Mining Effects")]
    public bool autoDestroyCommonItems;
    public bool extendDropModeDuration;
    public int extraInventorySlots;

    [Header("Shop Effects")]
    public float customerPatienceBonus;
}
