using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFlashEffect : MonoBehaviour
{
    public Tilemap tilemap;
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;

    private void Awake()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
    }

    public void FlashTile(Vector3Int cellPos)
    {
        StartCoroutine(FlashRoutine(cellPos));
    }

    private System.Collections.IEnumerator FlashRoutine(Vector3Int cellPos)
    {
        Color original = tilemap.GetColor(cellPos);
        tilemap.SetColor(cellPos, flashColor);

        yield return new WaitForSeconds(flashDuration);

        tilemap.SetColor(cellPos, original);
    }
}
