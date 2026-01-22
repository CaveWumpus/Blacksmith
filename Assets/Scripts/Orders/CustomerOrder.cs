[System.Serializable]
public class CustomerOrder
{
    // -----------------------------
    // Identity
    // -----------------------------
    public string orderID;              // Unique ID for saving/tracking
    public CustomerProfile customer;    // Who placed the order

    // -----------------------------
    // Requested Item
    // -----------------------------
    //public ItemPatternDefinition itemPattern;  // e.g., Pauldrons, Boots, etc.
    public VeinDefinition ore1;                // First ore type
    public VeinDefinition ore2;                // Second ore type
    public VeinDefinition gem;                 // Gem type
    //public GemCutType preferredCut;            // Preferred gem cut

    // -----------------------------
    // Knowledge Flags (UI gold/grey)
    // -----------------------------
    public bool knowsOre1Recipe;
    public bool knowsOre2Recipe;
    public bool knowsGemCut;
    public bool knowsItemPattern;

    // -----------------------------
    // Customer Expectations
    // -----------------------------
    public float accuracyRequirement;          // How precise the player must be
    public float polishRequirement;            // How high quality the item must be
    public float colorPreferenceStrength;      // How much color matters
    public float extraQualityBonus;            // Bonus if extras are fulfilled

    // -----------------------------
    // Deadlines
    // -----------------------------
    public int dayAccepted;                    // Day the player accepted the order
    public int dayDue;                         // Day the order is due
    public int daysToComplete;                 // Derived from generator

    // -----------------------------
    // Rewards
    // -----------------------------
    public int baseMoneyReward;
    public int baseReputationReward;

    // -----------------------------
    // State
    // -----------------------------
    public bool accepted;
    public bool completed;
    public bool failed;

    // -----------------------------
    // Result Data (filled after crafting)
    // -----------------------------
    public float finalQuality;                 // 0–100
    public float satisfactionScore;            // 0–100
    public bool exceptional;                   // Did the player exceed expectations?
}
