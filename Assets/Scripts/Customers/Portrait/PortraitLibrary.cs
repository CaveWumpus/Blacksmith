using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class PortraitLibrary<T> : ScriptableObject where T : PortraitPart
{
    public List<T> parts;

    public T GetRandom(System.Random rng)
    {
        float total = parts.Sum(p => p.weight);
        float roll = (float)(rng.NextDouble() * total);

        foreach (var part in parts)
        {
            if (roll < part.weight)
                return part;
            roll -= part.weight;
        }

        return parts[0];
    }
}
