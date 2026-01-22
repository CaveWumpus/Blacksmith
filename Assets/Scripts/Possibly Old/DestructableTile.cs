using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTilemap : MonoBehaviour
{
    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Called when mining raycast hits a wall tile
    public void MineTile(Vector3 worldPos)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        TileBase tile = tilemap.GetTile(cellPos);

        if (tile != null)
        {
            tilemap.SetTile(cellPos, null); // remove tile
            GameManager.Instance.AddOre("Stone", 1); // add resource to inventory
        }
    }
}
