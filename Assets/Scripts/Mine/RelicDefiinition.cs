using UnityEngine;



[CreateAssetMenu(menuName = "Mine/Relic Definition")]

public class RelicDefinition : TileDefinition
{
    public ItemDefinition item;   // add this
    [Header("Uniqueness")]
    public bool isUnique = true;
    public int maxCopies = 1;

    [Header("Drop Weight")]
    public float dropWeight = 1f;
}
