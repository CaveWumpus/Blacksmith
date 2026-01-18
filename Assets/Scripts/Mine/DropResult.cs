using UnityEngine;

public class DropResult
{
    public TileType dropType = TileType.Ground;

    public VeinDefinition vein;
    public RelicDefinition relic;
    public RecipeDefinition recipe;
    public PatternDefinition pattern;

    public static DropResult Nothing => new DropResult { dropType = TileType.Ground };
}
