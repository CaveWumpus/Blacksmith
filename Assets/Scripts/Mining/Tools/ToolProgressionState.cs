using UnityEngine;

[System.Serializable]
public class ToolProgressionState
{
    public int currentXP = 0;
    public int currentLevel = 1;

    // Ability unlock flags
    public bool ability1Unlocked = false;
    public bool ability2Unlocked = false;
    public bool ability3Unlocked = false;

    public void Reset()
    {
        currentXP = 0;
        currentLevel = 1;
        ability1Unlocked = false;
        ability2Unlocked = false;
        ability3Unlocked = false;
    }
}
