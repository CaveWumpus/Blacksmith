using UnityEngine;
using System.Collections.Generic;

public class PatternManager : MonoBehaviour
{
    public static PatternManager Instance;

    private HashSet<PatternDefinition> unlocked = new HashSet<PatternDefinition>();

    void Awake() => Instance = this;

    public void UnlockPattern(PatternDefinition pattern)
    {
        if (unlocked.Add(pattern))
            Debug.Log("Unlocked pattern: " + pattern.tileName);
    }
}
