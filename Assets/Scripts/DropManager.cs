using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance;

    [Header("Drop Pools")]
    public MineableDrop[] ores;
    public MineableDrop[] gems;
    public MineableDrop[] relics;

    void Awake()
    {
        Instance = this;
    }

    // --- Category-based selection ---
    public MineableDrop GetDropForCategory(TileCategory category, int playerLevel)
    {
        switch (category)
        {
            case TileCategory.OreVein:
            case TileCategory.SoftRock:
            case TileCategory.HardRock:
                return GetRandomOre(playerLevel);

            case TileCategory.GemVein:
                return GetRandomGem(playerLevel);

            case TileCategory.RelicVein:
                return GetRandomRelic(playerLevel);

            default:
                return null;
        }
    }

    // --- Ore pool ---
    public MineableDrop GetRandomOre(int playerLevel)
    {
        var candidates = ores.Where(o => o.unlockLevel <= playerLevel).ToList();
        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : null;
    }

    // --- Gem pool ---
    public MineableDrop GetRandomGem(int playerLevel)
    {
        var candidates = gems.Where(g => g.unlockLevel <= playerLevel).ToList();
        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : null;
    }

    // --- Relic pool ---
    public MineableDrop GetRandomRelic(int playerLevel)
    {
        var candidates = relics.Where(r => r.unlockLevel <= playerLevel).ToList();
        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : null;
    }
}
   