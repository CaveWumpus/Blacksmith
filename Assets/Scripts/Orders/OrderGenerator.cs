/*using UnityEngine;

public static class OrderGenerator
{
    public static CustomerOrder Generate(int currentDay, int playerLevel)
    {
        // -----------------------------
        // 1. Generate Customer Profile
        // -----------------------------
        CustomerProfile profile = CustomerGenerator.GenerateProfile(playerLevel);

        // -----------------------------
        // 2. Choose Item Pattern
        // -----------------------------
        ItemPatternDefinition pattern = PatternLibrary.GetRandomPattern(playerLevel);

        // -----------------------------
        // 3. Choose Materials
        // -----------------------------
        VeinDefinition ore1 = OreLibrary.GetRandomOre(playerLevel);
        VeinDefinition ore2 = OreLibrary.GetRandomOre(playerLevel);
        VeinDefinition gem  = GemLibrary.GetRandomGem(playerLevel);
        GemCutType cut      = GemCutLibrary.GetRandomCut();

        // -----------------------------
        // 4. Knowledge Flags
        // -----------------------------
        bool knowsOre1 = RecipeManager.Instance.HasRecipe(ore1);
        bool knowsOre2 = RecipeManager.Instance.HasRecipe(ore2);
        bool knowsCut  = PatternManager.Instance.HasPattern(cut);
        bool knowsItem = PatternManager.Instance.HasPattern(pattern);

        // -----------------------------
        // 5. Deadline
        // -----------------------------
        int daysToComplete = DeadlineCalculator.GenerateDeadline(pattern, profile, playerLevel);
        int dayDue = currentDay + daysToComplete;

        // -----------------------------
        // 6. Rewards
        // -----------------------------
        int money = RewardCalculator.CalculateMoney(pattern, ore1, ore2, gem, profile);
        int rep   = RewardCalculator.CalculateReputation(pattern, profile);

        // -----------------------------
        // 7. Build Order
        // -----------------------------
        CustomerOrder order = new CustomerOrder()
        {
            orderID = System.Guid.NewGuid().ToString(),
            customer = profile,

            itemPattern = pattern,
            ore1 = ore1,
            ore2 = ore2,
            gem = gem,
            preferredCut = cut,

            knowsOre1Recipe = knowsOre1,
            knowsOre2Recipe = knowsOre2,
            knowsGemCut = knowsCut,
            knowsItemPattern = knowsItem,

            accuracyRequirement = profile.accuracyTolerance,
            polishRequirement = profile.polishExpectation,
            colorPreferenceStrength = profile.colorPreferenceStrength,
            extraQualityBonus = profile.extraRequestsChance,

            dayAccepted = -1,
            dayDue = dayDue,
            daysToComplete = daysToComplete,

            baseMoneyReward = money,
            baseReputationReward = rep,

            accepted = false,
            completed = false,
            failed = false
        };

        return order;
    }
}
*/