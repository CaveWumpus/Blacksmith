using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class TileDefinition : ScriptableObject
{
    public string tileName;
    public TileType tileType;
    public TileBase tileAsset;

    [Header("Level Range")]
    public int levelStart;
    public int levelEnd;
}
