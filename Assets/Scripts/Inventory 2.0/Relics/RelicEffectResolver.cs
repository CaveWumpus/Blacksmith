using UnityEngine;

public static class RelicEffectResolver
{
    public static float GetMiningDamageMultiplier()
    {
        float total = 1f;
        foreach (var relic in RelicLoadoutController.Instance.Equipped)
            if (relic != null)
                total *= relic.miningDamageMultiplier;
        return total;
    }

    public static float GetMiningSpeedMultiplier()
    {
        float total = 1f;
        foreach (var relic in RelicLoadoutController.Instance.Equipped)
            if (relic != null)
                total *= relic.miningSpeedMultiplier;
        return total;
    }

    public static float GetRelicDiscoveryBonus()
    {
        float total = 0f;
        foreach (var relic in RelicLoadoutController.Instance.Equipped)
            if (relic != null)
                total += relic.relicDiscoveryBonus;
        return total;
    }

    public static float GetGemDropBonus()
    {
        float total = 0f;
        foreach (var relic in RelicLoadoutController.Instance.Equipped)
            if (relic != null)
                total += relic.gemDropBonus;
        return total;
    }
}
