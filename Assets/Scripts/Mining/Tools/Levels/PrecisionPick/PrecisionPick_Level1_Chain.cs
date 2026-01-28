using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/PrecisionPick/Level1_Chain")]
public class PrecisionPick_Level1_Chain : ToolLevelBehaviour
{
    [Header("Chain Settings")]
    public int chainLength = 2;
    public float falloff = 1f;

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

        // ⭐ Now apply damage
        TileDurabilityManager.Instance.Damage(cellPos, tilemap, finalDamage - 1);

        if (!hitWeak)
            return;

        // Chain forward
        Vector3Int current = cellPos;
        float currentDamage = finalDamage;

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

            int dmgToApply = Mathf.Max(1, Mathf.RoundToInt(currentDamage));
            TileDurabilityManager.Instance.Damage(current, tilemap, dmgToApply - 1);

            currentDamage *= falloff;
        }
    }
}

