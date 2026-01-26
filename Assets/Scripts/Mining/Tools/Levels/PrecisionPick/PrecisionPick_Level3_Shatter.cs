using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/PrecisionPick/Level3_Shatter")]
public class PrecisionPick_Level3_Shatter : ToolLevelBehaviour
{
    [Header("Shatter Settings")]
    public int shatterRadius = 1;
    public float falloff = 0.8f;
    public bool directionalOnly = true;

    public override void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        float chargeMultiplier)
    {
        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null)
            return;

        int damage = controller.ComputeBaseDamage(chargeMultiplier);
        TileDurabilityManager.Instance.Damage(cellPos, tilemap, damage - 1);

        // Weak spot check
        Vector2 direction = controller.GetMiningDirection();
        WeakPointDirection weakDir = TileDurabilityManager.Instance.GetWeakPoint(cellPos);
        DualTileDebugHelper.RecordHitTile(cellPos, weakDir, controller.HitWeakPoint(direction, weakDir));

        if (!controller.HitWeakPoint(direction, weakDir))
            return;

        // Directional AoE
        Vector3Int dir = new Vector3Int(
            Mathf.RoundToInt(direction.x),
            Mathf.RoundToInt(direction.y),
            0
        );

        for (int x = -shatterRadius; x <= shatterRadius; x++)
        {
            for (int y = -shatterRadius; y <= shatterRadius; y++)
            {
                Vector3Int offset = new Vector3Int(x, y, 0);
                Vector3Int target = cellPos + offset;
                Debug.Log("Shatter: Applying damage to " + cellPos);

                if (directionalOnly)
                {
                    // Only damage tiles in front of the hit
                    if (Vector3.Dot(offset, dir) <= 0)
                        continue;
                }

                TileBase t = tilemap.GetTile(target);
                if (t == null)
                    continue;

                float dist = offset.magnitude;
                int dmgToApply = Mathf.Max(1, Mathf.RoundToInt(damage * Mathf.Pow(falloff, dist)));

                TileDurabilityManager.Instance.Damage(target, tilemap, dmgToApply - 1);
            }
        }
    }
}
