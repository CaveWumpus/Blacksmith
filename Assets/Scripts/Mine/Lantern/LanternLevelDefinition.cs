using UnityEngine;

[System.Serializable]
public class LanternLevelDefinition
{
    public string levelName = "Level 0";

    [Header("Light Settings")]
    public float radius = 2f;
    public float intensity = 1f;

    [Header("Fuel Settings")]
    public float fuelCostPerSecond = 0.1f;

    [Header("Optional Visuals")]
    public bool enableFlicker = false;
    public float flickerAmount = 0.1f;
    public float flickerSpeed = 20f;
}
