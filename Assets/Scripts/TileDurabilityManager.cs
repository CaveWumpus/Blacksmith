using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileDurabilityManager : MonoBehaviour
{
    public static TileDurabilityManager Instance;

    // Track durability per cell position
    private Dictionary<Vector3Int, int> durabilityMap = new Dictionary<Vector3Int, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Damage a tile at a given cell position.
    /// </summary>
    public void Damage(Vector3Int cellPos, Tilemap tilemap, int maxDurability, GameObject dropPrefab = null)
    {
        // Initialize durability if not tracked yet
        if (!durabilityMap.ContainsKey(cellPos))
        {
            durabilityMap[cellPos] = maxDurability;
        }

        // Reduce durability
        durabilityMap[cellPos]--;

        Debug.Log($"Damaged tile at {cellPos}, remaining HP: {durabilityMap[cellPos]}");

        // If durability reaches zero, break the tile
        if (durabilityMap[cellPos] <= 0)
        {
            BreakTile(cellPos, tilemap, dropPrefab);
        }
    }

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
    }
}
