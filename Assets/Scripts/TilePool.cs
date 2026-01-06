using UnityEngine;

[CreateAssetMenu(fileName = "TilePool", menuName = "Mine/TilePool")]
public class TilePool : ScriptableObject
{
    public TileDefinition[] tileDefinitions;
}
