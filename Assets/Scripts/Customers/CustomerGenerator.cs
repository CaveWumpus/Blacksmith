/*using UnityEngine;
   
public static class CustomerGenerator
{
    public static CustomerProfile GenerateProfile(int playerLevel)
    {
        CustomerProfile profile = new CustomerProfile();
        profile.customerID = System.Guid.NewGuid().ToString();
        profile.seed = Random.Range(0, int.MaxValue);

        // Generate traits
        profile.accuracyTolerance = Random.Range(0.7f, 1.3f);
        profile.timelinessImportance = Random.Range(0.7f, 1.3f);
        profile.polishExpectation = Random.Range(0.7f, 1.3f);
        profile.colorPreferenceStrength = Random.Range(0f, 1f);
        profile.extraRequestsChance = Random.Range(0f, 0.5f);

        // Portrait generation happens here
        PortraitGenerator.GeneratePortrait(
            profile,
            Libraries.HeadShapes,
            Libraries.Eyes,
            Libraries.Noses,
            Libraries.Mouths,
            Libraries.Hair,
            Libraries.Accessories,
            Libraries.SkinColors,
            Libraries.ShirtColors
        );

        return profile;
    }
}
*/