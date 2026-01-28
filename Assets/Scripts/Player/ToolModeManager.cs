using UnityEngine;

public class ToolModeManager : MonoBehaviour
{
    [Header("Tool Modes")]
    public ToolModeDefinition[] toolModes;

    [Header("Current Tool")]
    public int currentIndex = 0;
    public ToolModeDefinition CurrentTool => toolModes[currentIndex];

    [Header("Input")]
    public PlayerInputHandler input;

    public float sweetSpotBonus = 1.2f;
    public float perfectSpotBonus = 1.4f;
    public AuraProfile auraProfile;

    public int currentLevelIndex = 0;
    public ToolLevelBehaviour CurrentLevel => CurrentTool.levels[currentLevelIndex];


    //[Header("Drill State")]
    //public float drillTimer = 0f;

    void Update()
    {
        // Example: RB = next tool, LB = previous tool
        if (input.toolNextPressed)
            NextTool();

        if (input.toolPrevPressed)
            PreviousTool();


    }

    public void NextTool()
    {
        // TODO: play swap sound, flash UI, show icon

        currentIndex++;
        if (currentIndex >= toolModes.Length)
            currentIndex = 0;

        Debug.Log("Switched to tool: " + CurrentTool.mode);
        var mining = Object.FindFirstObjectByType<PlayerMiningController>();
        if (mining != null)
            mining.ResetDrillTimer();


    }

    public void PreviousTool()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = toolModes.Length - 1;

        Debug.Log("Switched to tool: " + CurrentTool.mode);
        var mining = Object.FindFirstObjectByType<PlayerMiningController>();
        if (mining != null)
            mining.ResetDrillTimer();


    }
    public bool IsAbilityUnlocked(ToolMode tool, int abilityIndex)
    {
        var state = ToolXPManager.Instance.GetState(tool);
        return abilityIndex switch
        {
            1 => state.ability1Unlocked,
            2 => state.ability2Unlocked,
            3 => state.ability3Unlocked,
            _ => false
        };
    }
    public void SetToolLevel(int level)
    {
        currentLevelIndex = Mathf.Clamp(level, 0, CurrentTool.levels.Length - 1);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 300, 40), "Tool: " + CurrentTool.mode);
    }

}
