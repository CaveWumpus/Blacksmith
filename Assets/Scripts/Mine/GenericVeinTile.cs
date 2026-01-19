using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Mine/Generic Vein Tile")]
public class GenericVeinTile : ScriptableObject
{
    [Header("Visual")]
    public TileBase tileAsset;

    [Header("Classification")]
    public TileType tileType;   // Ore or Gem
    public RarityTier rarity;   // Regular, Rare, Exotic
}
