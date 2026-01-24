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

    public InventoryItemData ToInventoryItemData()
    {
        // Use the global ItemType from InventoryItemData.cs
        ItemType mappedType = isRelic ? ItemType.Special : ItemType.Ore; // adjust later if you add Gem/Alloy

        return new InventoryItemData(
            id: this.itemName,
            name: this.itemName,
            icon: this.icon,
            rarity: ItemRarity.Common,   // placeholder until you add rarity here
            type: mappedType,
            orderRelevant: false,
            maxStack: this.maxStack
        );
    }
}

