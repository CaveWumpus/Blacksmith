/*using UnityEngine;

[CreateAssetMenu(fileName = "TilePool", menuName = "Mine/TilePool")]
public class TilePool : ScriptableObject
{
    public TileDefinition[] tileDefinitions;

    public TileDefinition GetRandomTileDefinition(int playerLevel = 1)
    {
        // Filter by unlock level
        var candidates = new System.Collections.Generic.List<TileDefinition>();
        foreach (var def in tileDefinitions)
        {
            if (def.unlockLevel <= playerLevel)
                candidates.Add(def);
        }

        if (candidates.Count == 0) return null;

        // Weighted random by spawnChance
        float totalWeight = 0f;
        foreach (var def in candidates) totalWeight += def.spawnChance;

        float roll = Random.value * totalWeight;
        foreach (var def in candidates)
        {
            if (roll < def.spawnChance)
                return def;
            roll -= def.spawnChance;
        }

        return candidates[0]; // fallback
    }
}*/
