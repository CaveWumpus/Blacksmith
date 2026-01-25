using UnityEngine;

[System.Serializable]
public struct InventoryItemData
{
    public string itemID;
    public string itemName;
    public Sprite icon;
    public RarityTier rarity;
    public ItemType itemType;
    public bool isOrderRelevant;

    public int maxStack;
    public int currentStack;

    public InventoryItemData(
        string id,
        string name,
        Sprite icon,
        RarityTier rarity,
        ItemType type,
        bool orderRelevant,
        int maxStack = 20)
    {
        this.itemID = id;
        this.itemName = name;
        this.icon = icon;
        this.rarity = rarity;
        this.itemType = type;
        this.isOrderRelevant = orderRelevant;

        this.maxStack = maxStack;
        this.currentStack = 1;
    }
}

//public enum ItemRarity { Regular,Rare, Exotic }
public enum ItemType { Ore, Gem, Alloy, Special }
