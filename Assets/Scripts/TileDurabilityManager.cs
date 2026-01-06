using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileDurabilityManager : MonoBehaviour
{
    public static TileDurabilityManager Instance;

    // Track durability per cell position

    [Header("Vein Pools")]
    public TilePool oreVeinPool;
    public TilePool gemVeinPool;
    public TilePool relicVeinPool;



    [Header("Durability Settings")]
    public int defaultRockDurability = 3; // hits required for rocks
    public int veinDurability = 2;        // hits required for ore/gem veins
    public int relicDurability = 1;       // hits required for relics

    [Header("Vein Tiles")]
    public TileBase oreVeinTile;
    public TileBase gemVeinTile;
    public TileBase relicVeinTile;

    // Track durability and drops per cell

    private Dictionary<Vector3Int, int> durabilityMap = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, MineableDrop> dropMap = new Dictionary<Vector3Int, MineableDrop>();

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Damage a tile at a given cell position.
    /// </summary>
    public void AssignDrop(Vector3Int cellPos, MineableDrop drop)
    {
        if (drop == null) return;
        dropMap[cellPos] = drop;
        durabilityMap[cellPos] = defaultRockDurability;
    }

    /// <summary>
    /// Damage a tile at the given cell position.
    /// </summary>
    public void Damage(Vector3Int cellPos, Tilemap tilemap)
    {
        if (!durabilityMap.ContainsKey(cellPos))
        {
            durabilityMap[cellPos] = defaultRockDurability;
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

    /// Assign durability from TileDefinition (rock or vein).
    /// </summary>
    public void AssignDrop(Vector3Int cellPos, MineableDrop drop, TileDefinition def)
    {

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


        durabilityMap[cellPos]--;
        Debug.Log($"[Damage] Cell {cellPos} hit. Remaining durability: {durabilityMap[cellPos]}");

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

        if (durabilityMap.ContainsKey(cellPos) && durabilityMap[cellPos] <= 0)

        {
            BreakTile(cellPos, tilemap);
        }
    }

    private void BreakTile(Vector3Int cellPos, Tilemap tilemap, GameObject dropPrefab)
    {
        // If this cell has a drop assigned
        if (dropMap.ContainsKey(cellPos))
        {
            MineableDrop drop = dropMap[cellPos];
            dropMap.Remove(cellPos);

            // If the tile is still a rock, reveal vein
            if (tilemap.GetTile(cellPos) != null &&
                (drop.dropType == DropType.Ore || drop.dropType == DropType.Gem || drop.dropType == DropType.Relic))
            {
                TileBase veinTile = GetVeinTileForDrop(drop);
                tilemap.SetTile(cellPos, veinTile);

                // Reset durability for vein
                if (drop.dropType == DropType.Relic)
                    durabilityMap[cellPos] = relicDurability;
                else
                    durabilityMap[cellPos] = veinDurability;

                // Store drop again so vein knows what to spawn
                dropMap[cellPos] = drop;
                return;
            }

            // If the tile is already a vein, spawn resources
            Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
            tilemap.SetTile(cellPos, null);
            durabilityMap.Remove(cellPos);

            if (drop.dropType == DropType.Relic)
            {
                if (drop.prefab != null)
                    Instantiate(drop.prefab, worldPos, Quaternion.identity);
            }
            else
            {
                int hits = Random.Range(drop.minHits, drop.maxHits + 1);
                for (int i = 0; i < hits; i++)
                {
                    if (drop.prefab != null)
                        Instantiate(drop.prefab, worldPos, Quaternion.identity);
                }
            }
        }
        else
        {
            // No drop assigned: just clear tile
            tilemap.SetTile(cellPos, null);
            durabilityMap.Remove(cellPos);
        }
    }

        Debug.Log($"Tile at {cellPos} destroyed!");
=======
            // Rock breaking: reveal vein, no inventory add
            if (IsRockTile(currentTile))
            {

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
=======
    private TileBase GetVeinTileForDrop(MineableDrop drop)
    {
        switch (drop.dropType)
        {
            case DropType.Ore: return oreVeinTile;
            case DropType.Gem: return gemVeinTile;
            case DropType.Relic: return relicVeinTile;
            default: return null;
        }

    }
}
