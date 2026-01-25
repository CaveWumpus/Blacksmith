using UnityEngine;

[CreateAssetMenu(menuName = "Tools/Tool Progression")]
public class ToolProgressionDefinition : ScriptableObject
{
    [Header("XP Settings")]
    public int maxLevel = 5;
    public int[] xpThresholds; // size = maxLevel

    [Header("Debug")]
    public bool startMaxed = false;
}
