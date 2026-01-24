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
    private Dictionary<Vector3Int, GameObject> weakPointIndicators = new Dictionary<Vector3Int, GameObject>();

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

    public void Damage(Vector3Int cellPos, Tilemap tilemap, int bonusDamage = 0)
    {
        if (!durabilityMap.ContainsKey(cellPos))
            return;

        durabilityMap[cellPos] -= (1 + bonusDamage);

        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null) return;

        if (!tileLookup.TryGetValue(tile, out TileDefinition def))
            return;

        // If this is a rock and a relic is pending, force a relic tile when it breaks
        if (def.tileType == TileType.Rock &&
            RelicManager.Instance != null &&
            RelicManager.Instance.relicPending &&
            durabilityMap[cellPos] <= 0)
        {
            if (TryForceRelicTile(cellPos, tilemap))
                return;
        }

        switch (def.tileType)
        {
            case TileType.Rock:
                HandleRockDamage(cellPos, tilemap, def);
                break;

            case TileType.Ore:
            case TileType.Gem:
            {
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

        if (drop.dropType == TileType.Ore || drop.dropType == TileType.Gem)
        {
            VeinDefinition vein = drop.vein;

            GenericVeinDefinition genericDef = null;
            if (GenericVeinLibrary.Instance != null)
            {
                genericDef = GenericVeinLibrary.Instance.GetGenericDefinition(
                    drop.dropType,
                    vein.rarity
                );
            }

            TileBase tileToSet = null;

            if (genericDef != null)
                tileToSet = genericDef.tileAsset;
            else if (vein != null)
                tileToSet = vein.tileAsset;

            tilemap.SetTile(cellPos, tileToSet);

            durabilityMap[cellPos] = Random.Range(vein.minDurability, vein.maxDurability + 1);
            return;
        }

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

    private void HandleVeinDamage(Vector3Int cellPos, Tilemap tilemap, VeinDefinition vein)
    {
        if (vein == null)
        {
            Debug.LogError("VEIN IS NULL at " + cellPos);
            return;
        }

        if (vein.item == null)
            Debug.LogError("VEIN.ITEM IS NULL for vein: " + vein.tileName);

        // Add ore/gem to the mining inventory
        if (MiningInventoryController.Instance == null)
        {
            Debug.LogError("MINING INVENTORY CONTROLLER INSTANCE IS NULL");
        }
        else
        {
            int yield = Random.Range(vein.minYield, vein.maxYield + 1);

            for (int i = 0; i < yield; i++)
            {
                InventoryItemData data = vein.item.ToInventoryItemData();
                MiningInventoryController.Instance.TryAddItem(data);
            }

            // Popup
            if (LootPopupSpawner.Instance != null)
            {
                Vector3 popupPos = tilemap.GetCellCenterWorld(cellPos);
                LootPopupSpawner.Instance.Spawn(
                    vein.item.itemName,
                    yield,
                    popupPos,
                    vein.rarity
                );
            }


        }

        if (durabilityMap[cellPos] > 0)
            return;

        ClearTile(cellPos, tilemap);
    }


    private void HandleRelicTile(Vector3Int cellPos, Tilemap tilemap, RelicDefinition relic)
    {
        if (relic == null)
        {
            Debug.LogError("RelicDefinition is NULL at " + cellPos);
            ClearTile(cellPos, tilemap);
            return;
        }

        // Add to owned relics (shop chest / meta progression)
        RelicManager.Instance.AddOwnedRelic(relic);
        RelicManager.Instance.ResetTimerAfterRelic();

        if (LootPopupSpawner.Instance != null)
        {
            Vector3 popupPos = tilemap.GetCellCenterWorld(cellPos);
            LootPopupSpawner.Instance.Spawn(relic.relicName, 1, popupPos, RarityTier.Rare);
        }

        ClearTile(cellPos, tilemap);
    }

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

    private void ClearTile(Vector3Int cellPos, Tilemap tilemap)
    {
        tilemap.SetTile(cellPos, null);
        durabilityMap.Remove(cellPos);
        dropMap.Remove(cellPos);

        if (weakPointIndicators.TryGetValue(cellPos, out GameObject indicator))
        {
            Destroy(indicator);
            weakPointIndicators.Remove(cellPos);
        }
    }

    private bool TryForceRelicTile(Vector3Int cellPos, Tilemap tilemap)
    {
        if (RelicManager.Instance == null)
            return false;

        // Determine biome from current mine context
        // Here we infer via RelicBiome.Universal; if you have a BiomeManager, plug it in.
        RelicBiome biome = RelicBiome.Universal;

        int playerLevel = 1; // If you track this globally, plug it in here.

        RelicDefinition relic = RelicManager.Instance.ChooseRelicForBiome(biome, playerLevel);
        if (relic == null)
        {
            RelicManager.Instance.relicPending = false;
            return false;
        }

        // Place relic tile where the rock just broke
        tilemap.SetTile(cellPos, relic.tileAsset);
        durabilityMap[cellPos] = 1;

        // Update drop map so HandleRockDamage won't overwrite it
        dropMap[cellPos] = new DropResult
        {
            dropType = TileType.Relic,
            relic = relic
        };

        RelicManager.Instance.relicPending = false;
        RelicManager.Instance.pendingRelic = relic;

        return true;
    }
}
