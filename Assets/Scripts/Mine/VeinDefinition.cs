using UnityEngine;
public enum RarityTier { Regular, Rare, Exotic }



[CreateAssetMenu(menuName = "Mine/Ore or Gem Definition")]

public class VeinDefinition : TileDefinition
{
    public RarityTier rarity;
    public ItemDefinition item;   // add this

    [Header("Visuals")] 
    public Sprite icon;

    [Header("Durability")]
    public int minDurability;
    public int maxDurability;

    [Header("Yield")]
    public int minYield;
    public int maxYield;
}
