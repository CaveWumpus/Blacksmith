using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/WideSwing/Level1_Diagonal")]
public class WideSwing_Level1_Diagonal : ToolLevelBehaviour
{
    [Header("Pattern Settings")]
    public bool hitPrimary = true;       // front/up/down
    public bool hitSecondary = true;     // aboveFront / side tile

    public override void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        int finalDamage)
    {
        if (tilemap == null)
            return;

        // Use the player's position as the base
        Vector3Int center = tilemap.WorldToCell(controller.mineOrigin.position);

        // Determine mining direction
        Vector2 dir = controller.GetMiningDirection();

        // Determine facing for left/right side hits
        int facingX = Mathf.RoundToInt(
            Mathf.Sign(dir.x == 0 ? controller.transform.localScale.x : dir.x)
        );

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

        // -----------------------------------------
        // HORIZONTAL MINING (left/right)
        // -----------------------------------------
        if (dir.x != 0)
        {
            // Primary: tile in front
            if (hitPrimary)
            {
                Vector3Int front = new Vector3Int(center.x + facingX, center.y, center.z);
                HitCell(front);
            }

            // Secondary: tile above the front tile
            if (hitSecondary)
            {
                Vector3Int aboveFront = new Vector3Int(center.x + facingX, center.y + 1, center.z);
                HitCell(aboveFront);
            }

            return;
        }

        // -----------------------------------------
        // VERTICAL MINING (up/down)
        // -----------------------------------------
        if (dir.y != 0)
        {
            int vertical = Mathf.RoundToInt(Mathf.Sign(dir.y));

            // Primary: tile above or below
            if (hitPrimary)
            {
                Vector3Int verticalCell = new Vector3Int(center.x, center.y + vertical, center.z);
                HitCell(verticalCell);
            }

            // Secondary: tile to the side of the vertical tile
            if (hitSecondary)
            {
                Vector3Int sideCell = new Vector3Int(center.x + facingX, center.y + vertical, center.z);
                HitCell(sideCell);
            }

            return;
        }
    }
}
