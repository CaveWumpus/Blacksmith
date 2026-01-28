using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/WideSwing/Level2_HighArc")]
public class WideSwing_Level2_HighArc : ToolLevelBehaviour
{
    [Header("Arc Settings")]
    public float arcAngle = 90f;      // big arc above
    public float arcRange = 2.5f;
    public int maxTilesHit = 4;

    public override void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        int finalDamage)
    {
        if (tilemap == null)
            return;

        Vector2 origin = controller.mineOrigin.position;

        // Center mostly above, slightly biased toward facing
        Vector2 facing = controller.GetMiningDirection();
        Vector2 centerDir = (Vector2.up + 0.3f * facing).normalized;

        var hitTiles = controller.GetTilesInArc(origin, centerDir, arcRange, arcAngle);

        int hits = 0;
        var flash = tilemap.GetComponent<TileFlashEffect>();

        foreach (var cell in hitTiles)
        {
            if (hits >= maxTilesHit)
                break;

            TileBase tile = tilemap.GetTile(cell);
            if (tile == null)
                continue;

            if (flash != null)
                flash.FlashTile(cell);

            TileDurabilityManager.Instance.Damage(cell, tilemap, finalDamage - 1);
            hits++;
        }
    }
}
