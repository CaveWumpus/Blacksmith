using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileDurabilityManager : MonoBehaviour
{
    public static TileDurabilityManager Instance;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    // Track durability per cell position
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    [Header("Vein Pools")]
    public TilePool oreVeinPool;
    public TilePool gemVeinPool;
    public TilePool relicVeinPool;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    private Dictionary<Vector3Int, int> durabilityMap = new Dictionary<Vector3Int, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    /// Damage a tile at a given cell position.
    /// </summary>
    public void Damage(Vector3Int cellPos, Tilemap tilemap, int maxDurability, GameObject dropPrefab = null)
    {
        // Initialize durability if not tracked yet
        if (!durabilityMap.ContainsKey(cellPos))
        {
            durabilityMap[cellPos] = maxDurability;
        }
=======
    /// Assign durability from TileDefinition (rock or vein).
    /// </summary>
    public void AssignDrop(Vector3Int cellPos, MineableDrop drop, TileDefinition def)
    {
=======
    /// Assign durability from TileDefinition (rock or vein).
    /// </summary>
    public void AssignDrop(Vector3Int cellPos, MineableDrop drop, TileDefinition def)
    {
>>>>>>> Stashed changes
=======
    /// Assign durability from TileDefinition (rock or vein).
    /// </summary>
    public void AssignDrop(Vector3Int cellPos, MineableDrop drop, TileDefinition def)
    {
>>>>>>> Stashed changes
        if (drop != null) dropMap[cellPos] = drop;

        if (def.randomDurabilityRange.x > 0 || def.randomDurabilityRange.y > 0)
            durabilityMap[cellPos] = Random.Range(def.randomDurabilityRange.x, def.randomDurabilityRange.y + 1);
        else
            durabilityMap[cellPos] = def.fixedDurability;

        Debug.Log($"[AssignDrop] {def.tileName} at {cellPos} durability {durabilityMap[cellPos]}");
    }

    /// <summary>
    /// Damage a tile at the given cell position.
    /// Rocks: no inventory add, only reveal vein when destroyed.
    /// Ore/Gem veins: add item each hit until durability reaches 0.
    /// Relic veins: add relic once, then clear tile immediately.
    /// </summary>
    public void Damage(Vector3Int cellPos, Tilemap tilemap)
    {
        if (!durabilityMap.ContainsKey(cellPos)) return;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

        // Reduce durability
        durabilityMap[cellPos]--;
        Debug.Log($"[Damage] Cell {cellPos} hit. Remaining durability: {durabilityMap[cellPos]}");

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        Debug.Log($"Damaged tile at {cellPos}, remaining HP: {durabilityMap[cellPos]}");

        // If durability reaches zero, break the tile
        if (durabilityMap[cellPos] <= 0)
=======
=======
        if (dropMap.ContainsKey(cellPos))
        {
            MineableDrop drop = dropMap[cellPos];
            TileBase currentTile = tilemap.GetTile(cellPos);

            // Ore/Gem veins: add item each hit
            if (IsVeinTile(currentTile) && (drop.dropType == DropType.Ore || drop.dropType == DropType.Gem))
            {
                PlayerInventory.Instance.AddItem(drop);
                Debug.Log($"[Damage] Added {drop.dropName} to inventory");
            }

            // Relic veins: add relic once, then clear immediately
            if (IsVeinTile(currentTile) && drop.dropType == DropType.Relic)
            {
                RelicInventory.Instance.AddRelic(drop);
                Debug.Log($"[Damage] Added relic {drop.dropName} to relic inventory");

                // Clear relic tile immediately
                tilemap.SetTile(cellPos, null);
                durabilityMap.Remove(cellPos);
                dropMap.Remove(cellPos);
                return;
            }
        }

=======

        durabilityMap[cellPos]--;
        Debug.Log($"[Damage] Cell {cellPos} hit. Remaining durability: {durabilityMap[cellPos]}");

        if (dropMap.ContainsKey(cellPos))
        {
            MineableDrop drop = dropMap[cellPos];
            TileBase currentTile = tilemap.GetTile(cellPos);

            // Ore/Gem veins: add item each hit
            if (IsVeinTile(currentTile) && (drop.dropType == DropType.Ore || drop.dropType == DropType.Gem))
            {
                PlayerInventory.Instance.AddItem(drop);
                Debug.Log($"[Damage] Added {drop.dropName} to inventory");
            }

            // Relic veins: add relic once, then clear immediately
            if (IsVeinTile(currentTile) && drop.dropType == DropType.Relic)
            {
                RelicInventory.Instance.AddRelic(drop);
                Debug.Log($"[Damage] Added relic {drop.dropName} to relic inventory");

                // Clear relic tile immediately
                tilemap.SetTile(cellPos, null);
                durabilityMap.Remove(cellPos);
                dropMap.Remove(cellPos);
                return;
            }
        }

>>>>>>> Stashed changes
        if (durabilityMap.ContainsKey(cellPos) && durabilityMap[cellPos] <= 0)
        {
            BreakTile(cellPos, tilemap);
        }
    }

    /// <summary>
    /// Handles breaking a tile:
    /// Rocks: reveal vein tile only.
    /// Veins: clear tile when durability reaches 0.
    /// </summary>
    private void BreakTile(Vector3Int cellPos, Tilemap tilemap)
    {
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        if (dropMap.ContainsKey(cellPos))
        {
            MineableDrop drop = dropMap[cellPos];
            TileBase currentTile = tilemap.GetTile(cellPos);

<<<<<<< Updated upstream
<<<<<<< Updated upstream
            // Ore/Gem veins: add item each hit
            if (IsVeinTile(currentTile) && (drop.dropType == DropType.Ore || drop.dropType == DropType.Gem))
            {
                PlayerInventory.Instance.AddItem(drop);
                Debug.Log($"[Damage] Added {drop.dropName} to inventory");
            }

            // Relic veins: add relic once, then clear immediately
            if (IsVeinTile(currentTile) && drop.dropType == DropType.Relic)
            {
                RelicInventory.Instance.AddRelic(drop);
                Debug.Log($"[Damage] Added relic {drop.dropName} to relic inventory");

                // Clear relic tile immediately
                tilemap.SetTile(cellPos, null);
                durabilityMap.Remove(cellPos);
                dropMap.Remove(cellPos);
                return;
            }
        }

        if (durabilityMap.ContainsKey(cellPos) && durabilityMap[cellPos] <= 0)
>>>>>>> Stashed changes
        {
            BreakTile(cellPos, tilemap, dropPrefab);
        }
    }

<<<<<<< Updated upstream
    private void BreakTile(Vector3Int cellPos, Tilemap tilemap, GameObject dropPrefab)
    {
        // Remove tile
        tilemap.SetTile(cellPos, null);
        durabilityMap.Remove(cellPos);

        // Spawn drop if assigned
        if (dropPrefab != null)
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
            Instantiate(dropPrefab, worldPos, Quaternion.identity);
        }

        Debug.Log($"Tile at {cellPos} destroyed!");
=======
    /// <summary>
    /// Handles breaking a tile:
    /// Rocks: reveal vein tile only.
    /// Veins: clear tile when durability reaches 0.
    /// </summary>
    private void BreakTile(Vector3Int cellPos, Tilemap tilemap)
    {
        if (dropMap.ContainsKey(cellPos))
        {
            MineableDrop drop = dropMap[cellPos];
            TileBase currentTile = tilemap.GetTile(cellPos);

            // Rock breaking: reveal vein, no inventory add
            if (IsRockTile(currentTile))
            {
=======
            // Rock breaking: reveal vein, no inventory add
            if (IsRockTile(currentTile))
            {
>>>>>>> Stashed changes
=======
            // Rock breaking: reveal vein, no inventory add
            if (IsRockTile(currentTile))
            {
>>>>>>> Stashed changes
                Debug.Log($"[BreakTile] Rock at {cellPos} destroyed, revealing vein of type {drop.dropType}");

                if (drop.dropType == DropType.Ore || drop.dropType == DropType.Gem || drop.dropType == DropType.Relic)
                {
                    TileDefinition veinDef = GetVeinDefinitionForDrop(drop.dropType);
                    if (veinDef != null)
                    {
                        tilemap.SetTile(cellPos, veinDef.tileAsset);
                        durabilityMap[cellPos] = Random.Range(
                            veinDef.randomDurabilityRange.x,
                            veinDef.randomDurabilityRange.y + 1
                        );
                        Debug.Log($"[BreakTile] Spawned vein {veinDef.tileName} at {cellPos} with durability {durabilityMap[cellPos]}");
                        return;
                    }
                }
            }
            else
            {
                // Vein breaking: clear tile
                Debug.Log($"[BreakTile] Vein at {cellPos} destroyed, clearing tile.");
                tilemap.SetTile(cellPos, null);
                durabilityMap.Remove(cellPos);
                dropMap.Remove(cellPos);
            }
        }
        else
        {
            // No drop assigned: just clear tile
            tilemap.SetTile(cellPos, null);
            durabilityMap.Remove(cellPos);
        }
    }

    private TileDefinition GetVeinDefinitionForDrop(DropType type)
    {
        switch (type)
        {
            case DropType.Ore: return oreVeinPool.GetRandomTileDefinition();
            case DropType.Gem: return gemVeinPool.GetRandomTileDefinition();
            case DropType.Relic: return relicVeinPool.GetRandomTileDefinition();
            default: return null;
        }
>>>>>>> Stashed changes
    }

    private bool IsRockTile(TileBase tile)
    {
        return tile != null && tile.name.Contains("Rock");
    }

    private bool IsVeinTile(TileBase tile)
    {
        return tile != null && tile.name.Contains("Vein");
    }

    private bool IsRockTile(TileBase tile)
    {
        return tile != null && tile.name.Contains("Rock");
    }

    private bool IsVeinTile(TileBase tile)
    {
        return tile != null && tile.name.Contains("Vein");
    }

    private bool IsRockTile(TileBase tile)
    {
        return tile != null && tile.name.Contains("Rock");
    }

    private bool IsVeinTile(TileBase tile)
    {
        return tile != null && tile.name.Contains("Vein");
    }
}
