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

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 300, 40), "Tool: " + CurrentTool.mode);
    }

}
