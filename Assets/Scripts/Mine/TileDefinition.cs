using UnityEngine;
using UnityEngine.Tilemaps;

public enum WeakPointDirection
{
    None,
    Left,
    Right,
    Up,
    Down
}

public class TileDefinition : ScriptableObject
{
    [Header("Identity")]
    public string tileName;
    public TileType tileType;
    public TileBase tileAsset;

    [Header("Level Range")]
    public int levelStart;
    public int levelEnd;

    [Header("Weak Point Settings")]
    public WeakPointDirection weakPointDirection = WeakPointDirection.None;
    public float weakPointMultiplier = 1.0f;

    [Header("Weak Point Visuals")]
    public GameObject weakPointVFX;
    public GameObject weakPointIndicatorPrefab;

}
