using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public enum ItemType { Item, Relic }
    public bool isRelic;
    public string itemName;
    public ItemType itemType;
    public int maxStack = 20;
    public Sprite icon;
}
