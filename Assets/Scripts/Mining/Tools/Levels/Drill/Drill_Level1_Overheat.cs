using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tools/Drill/Level1_Overheat")]
public class Drill_Level1_Overheat : ToolLevelBehaviour
{
    [Header("Heat Settings")]
    public float heatPerTick = 1f;
    public float maxHeat = 999f;
    public float heatCooldownRate = 2f;
    public float weakSpotHeatReduction = 1f;
    public float DebugHeat => currentHeat;

    private float currentHeat = 0f;

    public override void PerformMining(
        PlayerMiningController controller,
        Vector3Int cellPos,
        Tilemap tilemap,
        int finalDamage)
    {
        Debug.Log("Drill Level 1 running. Heat = " + currentHeat);

        // Overheated?
        if (currentHeat >= maxHeat)
        {
            Debug.Log("Drill overheated at heat = " + currentHeat);
            return;
        }
        // After heat changes
        currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat);

        //Debug.Log("Drill Level 1 running. Heat = " + currentHeat);

        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null)
            return;

        // Weak spot check
        Vector2 direction = controller.GetMiningDirection();
        WeakPointDirection weakDir = TileDurabilityManager.Instance.GetWeakPoint(cellPos);
        bool hitWeak = controller.HitWeakPoint(direction, weakDir);

        // Apply damage
        Debug.Log("Applying damage to " + cellPos);
        TileDurabilityManager.Instance.Damage(cellPos, tilemap, finalDamage - 1);


        // Flash effect
        var flash = tilemap.GetComponent<TileFlashEffect>();
        if (flash != null)
            flash.FlashTile(cellPos);

        // Heat logic
        currentHeat += heatPerTick;

        if (hitWeak)
            currentHeat = Mathf.Max(0f, currentHeat - weakSpotHeatReduction);
    }
    public void ResetHeat()
    {
        currentHeat = 0f;
    }
    public void CoolDown(float dt)
    {
        currentHeat = Mathf.Max(0f, currentHeat - heatCooldownRate * dt);
        currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat);
        //Debug.Log("Cooling... heat = " + currentHeat);

    }



}
