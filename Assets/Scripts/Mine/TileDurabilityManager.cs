using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TileDurabilityManager : MonoBehaviour
{
    public static TileDurabilityManager Instance;

    [Header("Definitions (Manual Mode)")]
    public bool useAutoLoad = true;

    [Header("Tile Definitions")]
    public List<TileDefinition> allDefinitions = new List<TileDefinition>();

    private Dictionary<TileBase, TileDefinition> tileLookup = new Dictionary<TileBase, TileDefinition>();
    private Dictionary<Vector3Int, int> durabilityMap = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, DropResult> dropMap = new Dictionary<Vector3Int, DropResult>();
    // Weak‑point indicators tied to tile positions
    private Dictionary<Vector3Int, GameObject> weakPointIndicators 
        = new Dictionary<Vector3Int, GameObject>();


    private void Awake()
    {
        Instance = this;

#if UNITY_EDITOR
        if (useAutoLoad)
            AutoLoadDefinitions();
#endif

        BuildLookupTable();
    }

#if UNITY_EDITOR
    private void AutoLoadDefinitions()
    {
        allDefinitions = LoadAllAssetsOfType<TileDefinition>();
        Debug.Log($"[TileDurabilityManager] Auto‑loaded {allDefinitions.Count} TileDefinitions.");
    }

    private List<T> LoadAllAssetsOfType<T>() where T : ScriptableObject
    {
        var assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                assets.Add(asset);
        }

        return assets;
    }
#endif

    private void BuildLookupTable()
    {
        tileLookup.Clear();

        foreach (var def in allDefinitions)
        {
            if (def != null && def.tileAsset != null)
                tileLookup[def.tileAsset] = def;
        }

        Debug.Log($"[TileDurabilityManager] Built lookup for {tileLookup.Count} tile assets.");
    }

    // ---------------------------------------------------------
    // Assign durability + drop result from MineGenerator
    // ---------------------------------------------------------
    public void AssignTile(Vector3Int cellPos, TileDefinition def, DropResult drop)
    {
        int durability = 1;

        if (def is RockDefinition rock)
            durability = Random.Range(rock.minDurability, rock.maxDurability + 1);
        else if (def is VeinDefinition vein)
            durability = Random.Range(vein.minDurability, vein.maxDurability + 1);

        durabilityMap[cellPos] = durability;
        dropMap[cellPos] = drop;
    }

    public void RegisterIndicator(Vector3Int cellPos, GameObject indicator)
    {
        weakPointIndicators[cellPos] = indicator;
    }


    // ---------------------------------------------------------
    // Damage tile
    // ---------------------------------------------------------
    public void Damage(Vector3Int cellPos, Tilemap tilemap, int bonusDamage = 0)
    {
        if (!durabilityMap.ContainsKey(cellPos))
            return;

        durabilityMap[cellPos] -= (1 + bonusDamage);

        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null) return;

        if (!tileLookup.TryGetValue(tile, out TileDefinition def))
            return;

        switch (def.tileType)
        {
            case TileType.Rock:
                HandleRockDamage(cellPos, tilemap, def);
                break;

            case TileType.Ore:
            case TileType.Gem:
            {
                // Always pull the REAL vein definition from dropMap
                if (!dropMap.TryGetValue(cellPos, out DropResult drop) || drop.vein == null)
                {
                    Debug.LogError("DropResult.vein is NULL at " + cellPos);
                    return;
                }

                HandleVeinDamage(cellPos, tilemap, drop.vein);
                break;
            }


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

        if (drop.dropType == TileType.Ground)
        {
            ClearTile(cellPos, tilemap);
            return;
        }

        // ORE / GEM → use generic vein tiles
        if (drop.dropType == TileType.Ore || drop.dropType == TileType.Gem)
        {
            VeinDefinition vein = drop.vein;

            GenericVeinDefinition genericDef = null;
            if (GenericVeinLibrary.Instance != null)
            {
                genericDef = GenericVeinLibrary.Instance.GetGenericDefinition(
                    drop.dropType,   // Ore or Gem
                    vein.rarity
                );
            }

            TileBase tileToSet = null;

            if (genericDef != null)
                tileToSet = genericDef.tileAsset;
            else if (vein != null)
                tileToSet = vein.tileAsset; // fallback to specific tile if needed

            tilemap.SetTile(cellPos, tileToSet);

            // Durability still comes from the specific vein definition
            durabilityMap[cellPos] = Random.Range(vein.minDurability, vein.maxDurability + 1);
            return;
        }


        // RELIC / RECIPE / PATTERN → use their specific tiles
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
        if (vein == null)
        {
            Debug.LogError("VEIN IS NULL at " + cellPos);
            return;
        }

        if (vein.item == null)
            Debug.LogError("VEIN.ITEM IS NULL for vein: " + vein.tileName);

        if (UnifiedInventoryController.Instance == null)
            Debug.LogError("INVENTORY CONTROLLER INSTANCE IS NULL");

        int yield = Random.Range(vein.minYield, vein.maxYield + 1);
        UnifiedInventoryController.Instance.TryAddItem(vein.item, yield);

        // Spawn loot popup
        if (LootPopupSpawner.Instance != null)
        {
            Vector3 popupPos = tilemap.GetCellCenterWorld(cellPos);
            LootPopupSpawner.Instance.Spawn(vein.item.itemName, yield, popupPos, vein.rarity);
        }


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

        if (LootPopupSpawner.Instance != null)
        {
            Vector3 popupPos = tilemap.GetCellCenterWorld(cellPos);
            LootPopupSpawner.Instance.Spawn(relic.item.itemName, 1, popupPos, RarityTier.Regular);
        }

        if (!added)
            Debug.Log($"No space for relic {relic.tileName} – TODO: spawn pickup here.");

        ClearTile(cellPos, tilemap);
    }


    // ---------------------------------------------------------
    // RECIPE TILE → unlock permanently
    // ---------------------------------------------------------
    private void HandleRecipeTile(Vector3Int cellPos, Tilemap tilemap, RecipeDefinition recipe) 
    { 
        RecipeManager.Instance.UnlockRecipe(recipe); 

        if (LootPopupSpawner.Instance != null)
        {
            Vector3 popupPos = tilemap.GetCellCenterWorld(cellPos);
            LootPopupSpawner.Instance.Spawn(recipe.name, 1, popupPos, RarityTier.Regular);
        }

        ClearTile(cellPos, tilemap); 
    }


    // ---------------------------------------------------------
    // PATTERN TILE → unlock permanently
    // ---------------------------------------------------------
    private void HandlePatternTile(Vector3Int cellPos, Tilemap tilemap, PatternDefinition pattern) 
    { 
        PatternManager.Instance.UnlockPattern(pattern); 

        if (LootPopupSpawner.Instance != null)
        {
            Vector3 popupPos = tilemap.GetCellCenterWorld(cellPos);
            LootPopupSpawner.Instance.Spawn(pattern.name, 1, popupPos, RarityTier.Regular);
        }

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

        // Remove weak‑point indicator if present
        if (weakPointIndicators.TryGetValue(cellPos, out GameObject indicator))
        {
            Destroy(indicator);
            weakPointIndicators.Remove(cellPos);
        }
    }

}
