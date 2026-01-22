[System.Serializable]
public class CustomerProfile
{
    // -----------------------------
    // Identity
    // -----------------------------
    public string customerID;       // Unique ID for saving / tracking
    public string displayName;      // Optional: random name generator

    // -----------------------------
    // Portrait (Mr. Potato Head System)
    // -----------------------------
    public int seed;                // Ensures portrait is reproducible

    public CustomerHeadShape headShape;
    public CustomerEyes eyes;
    public CustomerNose nose;
    public CustomerMouth mouth;
    public CustomerHair hair;
    public CustomerAccessory accessory;

    //public Color skinColor;
    //public Color shirtColor;

    // -----------------------------
    // Personality Traits (Affect Satisfaction)
    // -----------------------------
    public float accuracyTolerance;         // How picky about correct materials
    public float timelinessImportance;      // How strict about deadlines
    public float polishExpectation;         // How much quality matters
    public float colorPreferenceStrength;   // How much color matching matters
    public float extraRequestsChance;       // Chance customer wants extras

    // -----------------------------
    // Optional: Customer Archetype
    // -----------------------------
    //public CustomerArchetype archetype;     // Noble, Cheapskate, Collector, etc.

    // -----------------------------
    // Optional: Flavor
    // -----------------------------
    public string personalityTagline;       // "A picky noble with refined taste"
}
