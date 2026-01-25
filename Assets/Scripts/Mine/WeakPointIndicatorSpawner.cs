using UnityEngine;
using UnityEngine.Tilemaps;

public class WeakPointIndicatorSpawner : MonoBehaviour
{
    public Tilemap tilemap;
    public TileData tileData;

    private GameObject indicator;

    void Start()
    {
        

        if (tilemap == null)
            tilemap = GetComponentInParent<Tilemap>();

        if (tileData == null)
            tileData = GetComponent<TileData>();

        if (tileData == null || tileData.tileDefinition == null)
            return;

        if (!tileData.hasWeakPoint)
            return;

        TileDefinition def = tileData.tileDefinition;
        WeakPointDirection dir = tileData.weakPointDirection;

        if (def.weakPointIndicatorPrefab == null)
            return;

        Vector3Int cellPos = tilemap.WorldToCell(transform.position);
        Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);

        indicator = Instantiate(def.weakPointIndicatorPrefab, worldPos, Quaternion.identity, transform);
        Debug.Log($"Spawner sees weak point {tileData.weakPointDirection} on {gameObject.name}");

        RotateIndicator(dir);
    }



    void RotateIndicator(WeakPointDirection dir)
    {
        if (indicator == null)
            return;

        switch (dir)
        {
            case WeakPointDirection.Left:  indicator.transform.rotation = Quaternion.Euler(0, 0, 180); break;
            case WeakPointDirection.Right: indicator.transform.rotation = Quaternion.Euler(0, 0, 0);   break;
            case WeakPointDirection.Up:    indicator.transform.rotation = Quaternion.Euler(0, 0, 90);  break;
            case WeakPointDirection.Down:  indicator.transform.rotation = Quaternion.Euler(0, 0, 270); break;
        }
    }
}
