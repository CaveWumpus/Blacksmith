using UnityEngine;
using System.Collections.Generic;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }

    [Header("All Relics In The Game")]
    public List<RelicDefinition> allRelics = new List<RelicDefinition>();

    [Header("Player-Owned Relics (Persistent)")]
    public List<RelicDefinition> ownedRelics = new List<RelicDefinition>();

    [Header("Equipped Relics (Active Next Run)")]
    public List<RelicDefinition> equippedRelics = new List<RelicDefinition>();

    [Header("Lock State (Shop = unlocked, Mine = locked)")]
    public bool relicsLocked = false;

    [Header("Relic Timer Settings")]
    public float relicTimer = 0f;
    public float relicTimerMax = 600f;
    public float relicTimerMin = 120f;
    public float relicTimerGrowth = 60f;

    [Header("Pending Relic")]
    public bool relicPending = false;
    public RelicDefinition pendingRelic;

    [Header("Performance")]
    public float performanceMultiplier = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LockRelics()  => relicsLocked = true;
    public void UnlockRelics() => relicsLocked = false;

    public bool OwnsRelic(string relicID)
    {
        return ownedRelics.Exists(r => r.relicID == relicID);
    }

    public void AddOwnedRelic(RelicDefinition relic)
    {
        if (relic == null) return;
        if (!ownedRelics.Contains(relic))
            ownedRelics.Add(relic);
    }

    public void TickRelicTimer(float deltaTime)
    {
        float bonus = RelicEffectResolver.GetRelicDiscoveryBonus();
        relicTimer += deltaTime * performanceMultiplier * (1f + bonus);

        if (!relicsLocked)
            return;

        if (relicPending)
            return;

        relicTimer += deltaTime * performanceMultiplier;

        if (relicTimer >= relicTimerMax)
        {
            relicTimer = 0f;
            relicTimerMax += relicTimerGrowth;
            relicPending = true;
            pendingRelic = null; // chosen at force time
        }
    }

    public RelicDefinition ChooseRelicForBiome(RelicBiome biome, int playerLevel)
    {
        var pool = allRelics.FindAll(r =>
            !ownedRelics.Contains(r) &&
            (r.biome == RelicBiome.Universal || r.biome == biome) &&
            playerLevel >= r.levelStart &&
            playerLevel <= r.levelEnd
        );

        if (pool.Count == 0)
            return null;

        return pool[Random.Range(0, pool.Count)];
    }

    public void ApplyMiningSpeedBonus(float speedFactor)
    {
        performanceMultiplier = speedFactor;
    }

    public void ApplyDamagePenalty(float penalty)
    {
        relicTimer = Mathf.Max(0, relicTimer - penalty);
    }

    public void ApplyRareOreBonus(float bonus)
    {
        relicTimer += bonus;
    }

    public void ResetTimerAfterRelic()
    {
        relicTimer = 0f;
    }
}
