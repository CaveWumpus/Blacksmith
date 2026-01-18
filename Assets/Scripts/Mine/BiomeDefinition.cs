using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Mine/Biome Definition")]
public class BiomeDefinition : ScriptableObject
{
    public string biomeName;

    [Header("Rock Types Allowed")]
    public List<RockDefinition> allowedRocks;

    [Header("Drop Multipliers")]
    public float oreMultiplier = 1f;
    public float gemMultiplier = 1f;
    public float relicMultiplier = 1f;
    public float recipeMultiplier = 1f;
    public float patternMultiplier = 1f;

    [System.Serializable]
    public class RockWeightModifier
    {
        public RockDefinition rock;
        public float multiplier = 1f;
    }

    [Header("Rock Spawn Weight Modifiers")]
    public List<RockWeightModifier> rockModifiers;
}
