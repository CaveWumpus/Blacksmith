using UnityEngine;
using System.Collections.Generic;


public class DropManager : MonoBehaviour
{
    public static DropManager Instance;

    [Header("Drop Pools")]
    public List<MineableDrop> oreDrops;
    public List<MineableDrop> gemDrops;
    public List<MineableDrop> relicDrops;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Get a random drop of a specific type, filtered by player level.
    /// </summary>
    public MineableDrop GetDropForType(DropType type, int playerLevel)
    {
        List<MineableDrop> candidates = new List<MineableDrop>();

        switch (type)
        {
            case DropType.Ore:
                foreach (var d in oreDrops)
                    if (d.unlockLevel <= playerLevel) candidates.Add(d);
                break;

            case DropType.Gem:
                foreach (var d in gemDrops)
                    if (d.unlockLevel <= playerLevel) candidates.Add(d);
                break;

            case DropType.Relic:
                foreach (var d in relicDrops)
                    if (d.unlockLevel <= playerLevel) candidates.Add(d);
                break;

            default:
                return null;
        }

        if (candidates.Count == 0) return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    /// <summary>
    /// Get a random drop from any category, filtered by player level.
    /// </summary>
    public MineableDrop GetDrop(int playerLevel)
    {
        List<MineableDrop> candidates = new List<MineableDrop>();

        foreach (var d in oreDrops)
            if (d.unlockLevel <= playerLevel) candidates.Add(d);

        foreach (var d in gemDrops)
            if (d.unlockLevel <= playerLevel) candidates.Add(d);

        foreach (var d in relicDrops)
            if (d.unlockLevel <= playerLevel) candidates.Add(d);

        if (candidates.Count == 0) return null;

        return candidates[Random.Range(0, candidates.Count)];
    }
}
 
