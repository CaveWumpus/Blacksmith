using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ToolLevelBehaviour : ScriptableObject
{
    public abstract void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        int finalDamage);
}
