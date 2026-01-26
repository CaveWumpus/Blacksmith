using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/PrecisionPick/Level2")]
public class PrecisionPickLevel2 : ToolLevelBehaviour
{
    public override void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        float chargeMultiplier)
    {
        // Step 1: Do Level 1 damage
        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null)
            return;

        TileDefinition def;
        if (!TileDurabilityManager.Instance.TryGetDefinition(tile, out def))
            return;

        int finalDamage = controller.ComputeBaseDamage(chargeMultiplier);

        TileDurabilityManager.Instance.Damage(cellPos, tilemap, finalDamage - 1);

        // Step 2: Weak spot chaining
        Vector2 direction = controller.GetMiningDirection();
        WeakPointDirection weakDir = TileDurabilityManager.Instance.GetWeakPoint(cellPos);

        if (!controller.HitWeakPoint(direction, weakDir))
        {
            Debug.Log("Not a weak spot hit — no chaining");
            return;
        }
        else
        {
            Debug.Log("Weak spot hit — chaining!");
        }

        // Chain 2 tiles
        Vector3Int current = cellPos;
        for (int i = 0; i < 2; i++)
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

