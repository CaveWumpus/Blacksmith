using UnityEngine;
using UnityEngine.Tilemaps;

public class WeakPointIndicatorSpawner : MonoBehaviour
{
    public Tilemap tilemap;
    public TileData tileData;

    private GameObject indicator;

    void Start()
    {
        if (tileData == null || tileData.tileDefinition == null)
            return;

        var def = tileData.tileDefinition;

        if (def.weakPointDirection == WeakPointDirection.None)
            return;

        if (def.weakPointIndicatorPrefab == null)
            return;

        // Get the tile cell under this tilemap
        Vector3Int cellPos = tilemap.WorldToCell(transform.position);

        // Convert tile cell to world position (center of tile)
        Vector3 worldPos = tilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0);

        // Spawn indicator at the tile’s world position
        indicator = Instantiate(def.weakPointIndicatorPrefab, worldPos, Quaternion.identity, transform);

        RotateIndicator(def.weakPointDirection);
    }

    void RotateIndicator(WeakPointDirection dir)
    {
        if (indicator == null)
            return;

        switch (dir)
        {
            case WeakPointDirection.Left:  indicator.transform.rotation = Quaternion.Euler(0,0,180); break;
            case WeakPointDirection.Right: indicator.transform.rotation = Quaternion.Euler(0,0,0); break;
            case WeakPointDirection.Up:    indicator.transform.rotation = Quaternion.Euler(0,0,90); break;
            case WeakPointDirection.Down:  indicator.transform.rotation = Quaternion.Euler(0,0,270); break;
        }
    }
}
