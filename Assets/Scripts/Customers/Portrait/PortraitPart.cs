using UnityEngine;

public abstract class PortraitPart : ScriptableObject
{
    public Sprite sprite;
    public float weight = 1f;       // For weighted random selection
    public string[] tags;           // Optional: "cute", "strict", "fancy"
}
