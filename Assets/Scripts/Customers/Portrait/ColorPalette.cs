using UnityEngine;

[CreateAssetMenu(menuName = "Customers/ColorPalette")]
public class ColorPalette : ScriptableObject
{
    public Color[] colors;

    public Color GetRandom(System.Random rng)
    {
        return colors[rng.Next(0, colors.Length)];
    }
}
