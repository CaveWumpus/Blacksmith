using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileDurabilityManager : MonoBehaviour
{
    public static TileDurabilityManager Instance;

    [Header("Tile Definitions")]
    public List<TileDefinition> allDefinitions;

    private Dictionary<TileBase, TileDefinition> tileLookup = new Dictionary<TileBase, TileDefinition>();
    private Dictionary<Vector3Int, int> durabilityMap = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, DropResult> dropMap = new Dictionary<Vector3Int, DropResult>();

    void Awake()
    {
        Instance = this;

        // Build lookup table
        foreach (var def in allDefinitions)
        {
            if (def.tileAsset != null)
                tileLookup[def.tileAsset] = def;
        }
    }

    // ---------------------------------------------------------
    // Assign durability + drop result from MineGenerator
    // ---------------------------------------------------------
    public void AssignTile(Vector3Int cellPos, TileDefinition def, DropResult drop)
    {
        // Assign durability
        int durability = 1;

        if (def is RockDefinition rock)
            durability = Random.Range(rock.minDurability, rock.maxDurability + 1);

        else if (def is VeinDefinition vein)
            durability = Random.Range(vein.minDurability, vein.maxDurability + 1);

        durabilityMap[cellPos] = durability;

        // Assign drop result
        dropMap[cellPos] = drop;
    }

    // ---------------------------------------------------------
    // Damage tile
    // ---------------------------------------------------------
    public void Damage(Vector3Int cellPos, Tilemap tilemap)
    {
        if (!durabilityMap.ContainsKey(cellPos))
            return;

        durabilityMap[cellPos]--;

        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null) return;

        if (!tileLookup.TryGetValue(tile, out TileDefinition def))
            return;

        // Handle tile type
        switch (def.tileType)
        {
            case TileType.Rock:
                HandleRockDamage(cellPos, tilemap, def);
                break;

            case TileType.Ore:
            case TileType.Gem:
                HandleVeinDamage(cellPos, tilemap, def as VeinDefinition);
                break;

            case TileType.Relic:
                HandleRelicTile(cellPos, tilemap, def as RelicDefinition);
                break;

            case TileType.Recipe:
                HandleRecipeTile(cellPos, tilemap, def as RecipeDefinition);
                break;

            case TileType.Pattern:
                HandlePatternTile(cellPos, tilemap, def as PatternDefinition);
                break;
        }
    }

    // ---------------------------------------------------------
    // ROCK DAMAGE → reveal vein or clear
    // ---------------------------------------------------------
    private void HandleRockDamage(Vector3Int cellPos, Tilemap tilemap, TileDefinition def)
    {
        if (durabilityMap[cellPos] > 0)
            return;

        DropResult drop = dropMap[cellPos];

        // If nothing was rolled → clear tile
        if (drop.dropType == TileType.Ground)
        {
            ClearTile(cellPos, tilemap);
            return;
        }

        // If ore/gem → spawn vein tile
        if (drop.dropType == TileType.Ore || drop.dropType == TileType.Gem)
        {
            VeinDefinition vein = drop.vein;
            tilemap.SetTile(cellPos, vein.tileAsset);

            durabilityMap[cellPos] = Random.Range(vein.minDurability, vein.maxDurability + 1);
            return;
        }

        // If relic/recipe/pattern → spawn special tile
        if (drop.dropType == TileType.Relic)
        {
            tilemap.SetTile(cellPos, drop.relic.tileAsset);
            durabilityMap[cellPos] = 1;
            return;
        }

        if (drop.dropType == TileType.Recipe)
        {
            tilemap.SetTile(cellPos, drop.recipe.tileAsset);
            durabilityMap[cellPos] = 1;
            return;
        }

        if (drop.dropType == TileType.Pattern)
        {
            tilemap.SetTile(cellPos, drop.pattern.tileAsset);
            durabilityMap[cellPos] = 1;
            return;
        }
    }

    // ---------------------------------------------------------
    // VEIN DAMAGE → yield items until empty
    // ---------------------------------------------------------
    private void HandleVeinDamage(Vector3Int cellPos, Tilemap tilemap, VeinDefinition vein)
    {
        if (vein == null) Debug.LogError("VEIN IS NULL at " + cellPos); if (vein != null && vein.item == null) Debug.LogError("VEIN.ITEM IS NULL for vein: " + vein.tileName); if (UnifiedInventoryController.Instance == null) Debug.LogError("INVENTORY CONTROLLER INSTANCE IS NULL");
        
        // Give item every hit
        int yield = Random.Range(vein.minYield, vein.maxYield + 1);
        UnifiedInventoryController.Instance.TryAddItem(vein.item, yield);

        if (durabilityMap[cellPos] > 0)
            return;

        ClearTile(cellPos, tilemap);
    }

    // ---------------------------------------------------------
    // RELIC TILE → collect once
    // ---------------------------------------------------------
    private void HandleRelicTile(Vector3Int cellPos, Tilemap tilemap, RelicDefinition relic)
    {
        bool added = UnifiedInventoryController.Instance.TryAddRelic(relic.item);

        if (!added)
        {
            Debug.Log($"No space for relic {relic.tileName} – TODO: spawn pickup here.");
        }

        ClearTile(cellPos, tilemap);
    }


    // ---------------------------------------------------------
    // RECIPE TILE → unlock permanently
    // ---------------------------------------------------------
    private void HandleRecipeTile(Vector3Int cellPos, Tilemap tilemap, RecipeDefinition recipe) 
    { 
        RecipeManager.Instance.UnlockRecipe(recipe); 
        ClearTile(cellPos, tilemap); 
    }

    // ---------------------------------------------------------
    // PATTERN TILE → unlock permanently
    // ---------------------------------------------------------
    private void HandlePatternTile(Vector3Int cellPos, Tilemap tilemap, PatternDefinition pattern) 
    { 
        PatternManager.Instance.UnlockPattern(pattern); 
        ClearTile(cellPos, tilemap); 
    }

    // ---------------------------------------------------------
    // Clear tile + cleanup
    // ---------------------------------------------------------
    private void ClearTile(Vector3Int cellPos, Tilemap tilemap)
    {
        tilemap.SetTile(cellPos, null);
        durabilityMap.Remove(cellPos);
        dropMap.Remove(cellPos);
    }
}
