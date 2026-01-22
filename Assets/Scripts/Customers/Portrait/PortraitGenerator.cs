using UnityEngine;

public static class PortraitGenerator
{
    public static void GeneratePortrait(CustomerProfile profile,
        HeadShapeLibrary headLib,
        EyesLibrary eyesLib,
        NoseLibrary noseLib,
        MouthLibrary mouthLib,
        HairLibrary hairLib,
        AccessoryLibrary accessoryLib,
        ColorPalette skinPalette,
        ColorPalette shirtPalette)
    {
        System.Random rng = new System.Random(profile.seed);

        profile.headShape = headLib.GetRandom(rng);
        profile.eyes = eyesLib.GetRandom(rng);
        profile.nose = noseLib.GetRandom(rng);
        profile.mouth = mouthLib.GetRandom(rng);
        profile.hair = hairLib.GetRandom(rng);
        profile.accessory = accessoryLib.GetRandom(rng);

        //profile.skinColor = skinPalette.GetRandom(rng);
        //profile.shirtColor = shirtPalette.GetRandom(rng);
    }
}
