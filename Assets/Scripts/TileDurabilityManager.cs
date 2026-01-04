using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileDurabilityManager : MonoBehaviour
{
    public static TileDurabilityManager Instance;

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
    /// Assign a hidden drop to a rock tile.
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

        durabilityMap[cellPos]--;

        if (durabilityMap[cellPos] <= 0)
        {
            BreakTile(cellPos, tilemap);
        }
    }

    /// <summary>
    /// Handles breaking a tile: either reveal vein or spawn resources.
    /// </summary>
    private void BreakTile(Vector3Int cellPos, Tilemap tilemap)
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
