using UnityEngine;

public class ComboMiningModule : MonoBehaviour
{
    [Header("Combo Mining Settings")]
    public bool comboEnabled = true;

    [Tooltip("Time window after a successful hit where the next hit gets a bonus.")]
    public float comboWindow = 0.35f;

    [Tooltip("How much faster mining becomes during a combo.")]
    public float comboSpeedMultiplier = 1.5f;

    [Tooltip("Optional: extra damage applied during a combo.")]
    public int comboBonusDamage = 0;


    [Tooltip("How long the combo streak lasts without hitting again.")]
    public float comboResetTime = 1.2f;
    [Header("Scaling Damage")]
    public float bonusPerCombo = 0.2f;
    public int maxComboCount = 15;
    [Header("UI Hook")]
    public ComboCounterUI comboUI;


    private float fractionalDamageCarry = 0f;


    [Header("Debug")]
    public int currentComboCount = 0;
    public bool inCombo = false;

    private float comboTimer = 0f;
    private float resetTimer = 0f;

    // Called every frame by PlayerMiningController
    public void UpdateComboState(float deltaTime)
    {
        if (inCombo)
            Debug.Log("Combo window ticking...");

        
        if (!comboEnabled)
            return;

        if (inCombo)
        {
            comboTimer -= deltaTime;
            resetTimer -= deltaTime;

            if (comboTimer <= 0f)
                inCombo = false;

            if (resetTimer <= 0f)
            {
                inCombo = false;
                currentComboCount = 0;
            }
        }
    }

    // Called by PlayerMiningController when a mining hit lands
    public void RegisterSuccessfulHit()
    {
        if (!comboEnabled)
            return;

        if (inCombo)
            currentComboCount++;
        else
        {
            currentComboCount = 1;
            inCombo = true;
        }

        if (comboUI != null)
        comboUI.Pulse();

        // Cap combo
        currentComboCount = Mathf.Min(currentComboCount, maxComboCount);

        comboTimer = comboWindow;
        resetTimer = comboResetTime;
    }


    // Returns modified cooldown based on combo state
    public float GetModifiedCooldown(float baseCooldown)
    {
        if (inCombo)
            Debug.Log("Combo active: faster mining!");

        if (!comboEnabled)
            return baseCooldown;

        if (inCombo)
            return baseCooldown / comboSpeedMultiplier;

        return baseCooldown;
    }

    // Returns bonus damage if in combo
    public int GetBonusDamage()
    {
        if (!comboEnabled || !inCombo)
            return 0;

        // Calculate fractional bonus
        float totalBonus = currentComboCount * bonusPerCombo;

        // Add fractional carryover
        totalBonus += fractionalDamageCarry;

        // Extract whole damage points
        int wholeDamage = Mathf.FloorToInt(totalBonus);

        // Store leftover fraction
        fractionalDamageCarry = totalBonus - wholeDamage;

        return wholeDamage;
    }


}
