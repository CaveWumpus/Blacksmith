using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/WideSwing/Level3_StepCutter")]
public class WideSwing_Level3_StepCutter : ToolLevelBehaviour
{
    [Header("Pattern Settings")]
    public bool hitAbove = true;
    public bool hitFront = true;
    public bool hitAboveFront = true;

    public override void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        int finalDamage)
    {
        if (tilemap == null)
            return;

        Vector2 dir = controller.GetMiningDirection();
        int facingX = Mathf.RoundToInt(Mathf.Sign(dir.x == 0 ? controller.transform.localScale.x : dir.x));

        // Base cell is the one we targeted
        Vector3Int center = cellPos;

        var flash = tilemap.GetComponent<TileFlashEffect>();

        void HitCell(Vector3Int c)
        {
            TileBase t = tilemap.GetTile(c);
            if (t == null)
                return;

            if (flash != null)
                flash.FlashTile(c);

            TileDurabilityManager.Instance.Damage(c, tilemap, finalDamage - 1);
        }

        if (hitFront)
        {
            Vector3Int front = new Vector3Int(center.x + facingX, center.y, center.z);
            HitCell(front);
        }

        if (hitAbove)
        {
            Vector3Int above = new Vector3Int(center.x, center.y + 1, center.z);
            HitCell(above);
        }

        if (hitAboveFront)
        {
            Vector3Int aboveFront = new Vector3Int(center.x + facingX, center.y + 1, center.z);
            HitCell(aboveFront);
        }
    }
}
