using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/PrecisionPick/Level2_Crit")]
public class PrecisionPick_Level2_Crit : ToolLevelBehaviour
{
    [Header("Crit Settings")]
    public float critMultiplier = 2f;

    [Header("Chain Settings")]
    public int chainLength = 1;

    public override void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        int finalDamage)
    {
        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null)
            return;

        // ⭐ Weak spot check FIRST
        Vector2 direction = controller.GetMiningDirection();
        WeakPointDirection weakDir = TileDurabilityManager.Instance.GetWeakPoint(cellPos);
        bool hitWeak = controller.HitWeakPoint(direction, weakDir);

        DualTileDebugHelper.RecordHitTile(cellPos, weakDir, hitWeak);

        // finalDamage already includes sweet/perfect bonuses
        // and the crit multiplier is applied here
        int critDamage = hitWeak
            ? Mathf.RoundToInt(finalDamage * critMultiplier)
            : finalDamage;

        TileDurabilityManager.Instance.Damage(cellPos, tilemap, critDamage - 1);


        if (!hitWeak)
            return;

        // Small chain (weaker than Level 1)
        Vector3Int current = cellPos;

        for (int i = 0; i < chainLength; i++)
        {
            current += new Vector3Int(
                Mathf.RoundToInt(direction.x),
                Mathf.RoundToInt(direction.y),
                0
            );

            TileBase nextTile = tilemap.GetTile(current);
            if (nextTile == null)
                break;

            TileDurabilityManager.Instance.Damage(current, tilemap, finalDamage - 1);
        }
    }
}
