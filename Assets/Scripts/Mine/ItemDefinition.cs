using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public enum DefinitionItemType { Item, Relic }

    public bool isRelic;
    public string itemName;
    public DefinitionItemType itemType;
    public int maxStack = 20;
    public Sprite icon;

    public InventoryItemData ToInventoryItemData(RarityTier rarity)
    {
        ItemType mappedType = isRelic ? ItemType.Special : ItemType.Ore;

        return new InventoryItemData(
            id: this.itemName,
            name: this.itemName,
            icon: this.icon,
            rarity: rarity,          // ← now using the passed rarity
            type: mappedType,
            orderRelevant: false,
            maxStack: this.maxStack
        );
    }

}

