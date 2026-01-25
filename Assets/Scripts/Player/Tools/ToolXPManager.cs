using UnityEngine;
using System;

public class ToolXPManager : MonoBehaviour
{
    public static ToolXPManager Instance { get; private set; }

    [Header("Progression Definitions")]
    public ToolProgressionDefinition precisionPickProgression;
    public ToolProgressionDefinition wideSwingProgression;
    public ToolProgressionDefinition drillProgression;
    public ToolProgressionDefinition hammerProgression;

    [Header("Runtime States")]
    public ToolProgressionState precisionPickState = new ToolProgressionState();
    public ToolProgressionState wideSwingState = new ToolProgressionState();
    public ToolProgressionState drillState = new ToolProgressionState();
    public ToolProgressionState hammerState = new ToolProgressionState();

    public event Action<ToolMode, int> OnToolLevelUp;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDebug();
    }

    private void InitializeDebug()
    {
        if (precisionPickProgression.startMaxed)
            MaxOut(precisionPickState, precisionPickProgression);

        if (wideSwingProgression.startMaxed)
            MaxOut(wideSwingState, wideSwingProgression);

        if (drillProgression.startMaxed)
            MaxOut(drillState, drillProgression);

        if (hammerProgression.startMaxed)
            MaxOut(hammerState, hammerProgression);
    }

    private void MaxOut(ToolProgressionState state, ToolProgressionDefinition def)
    {
        state.currentLevel = def.maxLevel;
        state.currentXP = def.xpThresholds[def.maxLevel - 1];
        state.ability1Unlocked = true;
        state.ability2Unlocked = true;
        state.ability3Unlocked = true;
    }

    public void AddXP(ToolMode tool, int amount)
    {
        var (state, def) = GetStateAndDef(tool);

        if (state.currentLevel >= def.maxLevel)
            return;

        state.currentXP += amount;

        if (state.currentXP >= def.xpThresholds[state.currentLevel - 1])
        {
            state.currentLevel++;
            UnlockAbilities(tool, state);
            OnToolLevelUp?.Invoke(tool, state.currentLevel);
        }
    }

    private void UnlockAbilities(ToolMode tool, ToolProgressionState state)
    {
        if (state.currentLevel == 2) state.ability1Unlocked = true;
        if (state.currentLevel == 3) state.ability2Unlocked = true;
        if (state.currentLevel == 4) state.ability3Unlocked = true;
    }

    private (ToolProgressionState, ToolProgressionDefinition) GetStateAndDef(ToolMode tool)
    {
        return tool switch
        {
            ToolMode.PrecisionPick => (precisionPickState, precisionPickProgression),
            ToolMode.WideSwing => (wideSwingState, wideSwingProgression),
            ToolMode.Drill => (drillState, drillProgression),
            ToolMode.Hammer => (hammerState, hammerProgression),
            _ => (precisionPickState, precisionPickProgression)
        };
    }
    public ToolProgressionState GetState(ToolMode tool)
    {
        return tool switch
        {
            ToolMode.PrecisionPick => precisionPickState,
            ToolMode.WideSwing => wideSwingState,
            ToolMode.Drill => drillState,
            ToolMode.Hammer => hammerState,
            _ => precisionPickState
        };
    }
    public bool IsAbilityUnlocked(ToolMode tool, int abilityIndex)
    {
        var state = GetState(tool);

        return abilityIndex switch
        {
            1 => state.ability1Unlocked,
            2 => state.ability2Unlocked,
            3 => state.ability3Unlocked,
            _ => false
        };
    }


}
