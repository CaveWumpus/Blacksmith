using UnityEngine;

[CreateAssetMenu(menuName = "Tools/Aura Profile")]
public class AuraProfile : ScriptableObject
{
    [Header("Timing (0–1 normalized)")]
    public float sweetStart = 0.4f;
    public float sweetEnd = 0.6f;

    public bool usePerfectWindow = false;
    public float perfectStart = 0.48f;
    public float perfectEnd = 0.52f;

    [Header("Colors")]
    public Color idleColor = Color.white;
    public Color sweetColor = new Color(1f, 0.8f, 0.2f);
    public Color perfectColor = new Color(1f, 1f, 0.4f);

    [Header("Heat Mode (Drill)")]
    public bool isHeatBased = false;
    public Color heatMinColor = Color.white;
    public Color heatMaxColor = Color.red;
}
