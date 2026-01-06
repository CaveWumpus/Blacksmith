using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileCategory
{
    SoftRock,
    HardRock,
    OreVein,
    GemVein,
    RelicVein
}

[CreateAssetMenu(fileName = "TileDefinition", menuName = "Mine/TileDefinition")]
public class TileDefinition : ScriptableObject
{
    [Header("Basic Info")]
    public string tileName;
    public TileCategory category;
    public TileBase tileAsset;

    [Header("Spawn Settings")]
    [Range(0f, 1f)] public float spawnChance = 1f;

    [Header("Durability Settings")]
    // For rocks: use fixedDurability
    public int fixedDurability = 1;

    // For veins: use randomDurabilityRange
    public Vector2Int randomDurabilityRange = new Vector2Int(0, 0);

    [Header("Progression Settings")]
    public int unlockLevel = 1; // level at which this tile becomes available
}
