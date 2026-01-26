using UnityEngine;

public class LanternManager : MonoBehaviour
{
    [Header("References")]
    public LanternLightController lightController;
    public LanternFuelSystem fuelSystem;

    [Header("Brightness Levels")]
    public LanternLevelDefinition[] levels;
    public int startingLevel = 0;
    public int minimumLevel = 0;

    [Header("Boost Settings")]
    public bool allowBoost = true;
    public int boostLevel = 4;
    public float boostDuration = 3f;
    public float boostCooldown = 5f;

    private int currentLevel;
    private int targetLevel;

    private float boostTimer = 0f;
    private float boostCooldownTimer = 0f;
    private bool isBoosting = false;

    void Awake()
    {
        currentLevel = Mathf.Clamp(startingLevel, minimumLevel, levels.Length - 1);
        targetLevel = currentLevel;

        ApplyLevel(currentLevel);

        fuelSystem.OnFuelEmpty += HandleFuelEmpty;
    }

    void Update()
    {
        HandleInput();
        HandleBoostTimers();
        HandleFuelLogic();
    }

    // -----------------------------
    // INPUT (Option C Hybrid Model)
    // -----------------------------
    private void HandleInput()
    {
        var input = PlayerInputHandler.Instance;

        if (input.lanternIncreasePressed)
            SetTargetLevel(targetLevel + 1);

        if (input.lanternDecreasePressed)
            SetTargetLevel(targetLevel - 1);

        if (allowBoost && input.lanternBoostPressed)
            TryBoost();
    }


    // -----------------------------
    // FUEL + AUTO-DROP LOGIC
    // -----------------------------
    private void HandleFuelLogic()
    {
        var level = levels[currentLevel];

        // If boosting, fuel drain is handled normally
        if (fuelSystem.HasFuelForLevel(level.fuelCostPerSecond))
        {
            fuelSystem.Consume(level.fuelCostPerSecond);
        }
        else
        {
            DropToLowerLevel();
        }
    }

    private void HandleFuelEmpty()
    {
        ForceLevel(minimumLevel);
    }

    private void DropToLowerLevel()
    {
        if (currentLevel > minimumLevel)
        {
            SetTargetLevel(currentLevel - 1);
        }
    }

    // -----------------------------
    // BOOST LOGIC
    // -----------------------------
    private void TryBoost()
    {
        if (!allowBoost)
            return;

        if (isBoosting || boostCooldownTimer > 0)
            return;

        isBoosting = true;
        boostTimer = boostDuration;
        SetTargetLevel(boostLevel);
    }

    private void HandleBoostTimers()
    {
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 0)
            {
                isBoosting = false;
                boostCooldownTimer = boostCooldown;
                SetTargetLevel(startingLevel);
            }
        }
        else if (boostCooldownTimer > 0)
        {
            boostCooldownTimer -= Time.deltaTime;
        }
    }

    // -----------------------------
    // LEVEL MANAGEMENT
    // -----------------------------
    private void SetTargetLevel(int newLevel)
    {
        targetLevel = Mathf.Clamp(newLevel, minimumLevel, levels.Length - 1);

        if (targetLevel != currentLevel)
        {
            currentLevel = targetLevel;
            ApplyLevel(currentLevel);
        }
    }

    private void ForceLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, minimumLevel, levels.Length - 1);
        targetLevel = currentLevel;
        ApplyLevel(currentLevel);
    }

    private void ApplyLevel(int levelIndex)
    {
        var level = levels[levelIndex];

        lightController.ApplyLevel(level.radius, level.intensity);

        // Optional flicker support
        lightController.enableFlicker = level.enableFlicker;
        lightController.flickerAmount = level.flickerAmount;
        lightController.flickerSpeed = level.flickerSpeed;
    }
}
