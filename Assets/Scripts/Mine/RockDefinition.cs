using UnityEngine;

[CreateAssetMenu(menuName = "Mine/Rock Definition")]
public class RockDefinition : TileDefinition
{
    [Header("Durability")]
    public int minDurability;
    public int maxDurability;

    [Header("Spawn Weight")]
    public float spawnWeight = 1f;
    [Header("XP Reward")]
    public int xpReward = 1;

    [Header("Drop Chances")]
    [Range(0,1)] public float chanceRegularOre;
    [Range(0,1)] public float chanceRareOre;
    [Range(0,1)] public float chanceExoticOre;

    [Range(0,1)] public float chanceRegularGem;
    [Range(0,1)] public float chanceRareGem;
    [Range(0,1)] public float chanceExoticGem;

    [Range(0,1)] public float chanceRelic;
    [Range(0,1)] public float chanceRecipe;
    [Range(0,1)] public float chancePattern;

    [Header("Nothing Chance")]
    [Range(0,1)] public float chanceNothing = 0.8f;
    
}
