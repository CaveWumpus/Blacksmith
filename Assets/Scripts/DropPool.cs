using UnityEngine;
using System.Linq;

[System.Serializable]
public class DropCategoryChance
{
    public DropType type;                // Ore, Gem, Relic
    [Range(0f, 1f)] public float chance; // percent weight
}

[CreateAssetMenu(fileName = "DropPool", menuName = "Mine/DropPool")]
public class DropPool : ScriptableObject
{
    [Header("Category Chances")]
    public DropCategoryChance[] categoryChances;

    [Header("Drops by Category")]
    public MineableDrop[] ores;
    public MineableDrop[] gems;
    public MineableDrop[] relics;

    public MineableDrop GetRandomDrop(int playerLevel)
    {
        // Weighted roll between Ore/Gem/Relic
        float total = 0f;
        foreach (var c in categoryChances) total += c.chance;

        if (total <= 0f)
        {
            // No categories enabled, return null
            return null;
        }

        float roll = Random.value * total;
        DropType chosenType = DropType.Ore;

        foreach (var c in categoryChances)
        {
            if (roll < c.chance)
            {
                chosenType = c.type;
                break;
            }
            roll -= c.chance;
        }

        // Pick a random drop from that category, filtered by unlockLevel
        switch (chosenType)
        {
            case DropType.Ore:
                var oreCandidates = System.Linq.Enumerable.Where(ores, o => o.unlockLevel <= playerLevel).ToList();
                return oreCandidates.Count > 0 ? oreCandidates[Random.Range(0, oreCandidates.Count)] : null;

            case DropType.Gem:
                var gemCandidates = System.Linq.Enumerable.Where(gems, g => g.unlockLevel <= playerLevel).ToList();
                return gemCandidates.Count > 0 ? gemCandidates[Random.Range(0, gemCandidates.Count)] : null;

            case DropType.Relic:
                var relicCandidates = System.Linq.Enumerable.Where(relics, r => r.unlockLevel <= playerLevel).ToList();
                return relicCandidates.Count > 0 ? relicCandidates[Random.Range(0, relicCandidates.Count)] : null;

            default:
                return null;
        }
    }
}
