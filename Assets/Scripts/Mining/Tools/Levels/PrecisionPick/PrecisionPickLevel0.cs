using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/PrecisionPick/Level1")]
public class PrecisionPickLevel1 : ToolLevelBehaviour
{
    public override void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        float chargeMultiplier)
    {
        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null)
            return;

        TileDefinition def;
        if (!TileDurabilityManager.Instance.TryGetDefinition(tile, out def))
            return;

        int finalDamage = controller.ComputeBaseDamage(chargeMultiplier);

        TileDurabilityManager.Instance.Damage(cellPos, tilemap, finalDamage - 1);
    }
}
