[System.Serializable]
public class InventorySlot
{
    public ItemDefinition item;
    public int count;

    public bool occupied => item != null && count > 0;
}
