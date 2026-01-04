using UnityEngine;

public enum DropType { Ore, Gem, Relic }

[CreateAssetMenu(fileName = "NewMineableDrop", menuName = "Mine/MineableDrop")]
public class MineableDrop : ScriptableObject
{
    [Header("Basic Info")]
    public string dropName;
    public DropType dropType;

    [Header("Visuals")]
    public Sprite icon;          // UI icon
    public GameObject prefab;    // world prefab to spawn when mined

    [Header("Mining Settings")]
    public int minHits = 1;      // minimum items dropped
    public int maxHits = 4;      // maximum items dropped
    public int unlockLevel = 1;  // progression level required
}
