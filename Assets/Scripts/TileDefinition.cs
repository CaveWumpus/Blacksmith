using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileCategory { Ground, SoftRock, HardRock, OreVein, GemVein, RelicVein }

[CreateAssetMenu(fileName = "NewTileDefinition", menuName = "Mine/TileDefinition")]
public class TileDefinition : ScriptableObject
{
    public TileCategory category;
    public TileBase tileAsset;
    [Range(0f, 1f)] public float spawnChance = 0.1f;
}
