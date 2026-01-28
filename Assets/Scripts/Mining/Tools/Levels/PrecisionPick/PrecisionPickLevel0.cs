/*using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/PrecisionPick/Level1")]
public class PrecisionPickLevel1 : ToolLevelBehaviour
{
    public override void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        int finalDamage)

    {
        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null)
            return;

        TileDefinition def;
        if (!TileDurabilityManager.Instance.TryGetDefinition(tile, out def))
            return;


        TileDurabilityManager.Instance.Damage(cellPos, tilemap, finalDamage - 1);
    }
}
*/