using UnityEngine;

[CreateAssetMenu(fileName = "ToolModeDefinition", menuName = "Mining/Tool Mode Definition")]
public class ToolModeDefinition : ScriptableObject
{
    [Header("Identity")]
    public ToolMode mode;

    [Header("Charge Behavior")]
    public float chargeSpeedMultiplier = 1f;   // 1 = normal, <1 = slower, >1 = faster
    public float damageMultiplier = 1f;        // base damage scaling
    public float weakPointBonus = 0f;          // extra multiplier when hitting weak points

    [Header("Area of Effect")]
    public int aoeRadius = 0;                  // 0 = single tile, >0 = AoE

    [Header("Stamina / Cost")]
    public float staminaCost = 0f;             // future-proofing

    [Header("Visuals & Audio")]
    public GameObject hitVFX;
    public AudioClip hitSFX;
}
