using UnityEngine;

[CreateAssetMenu(
    fileName = "NewInventoryData",
    menuName = "Inventory/Unified Inventory Data"
)]
public class BaseInventoryData : ScriptableObject
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemDefinition item;
        public int count;

        public bool occupied => item != null && count > 0;

        public void Clear()
        {
            item = null;
            count = 0;
        }
    }

    [Header("Inventory Settings")]
    public int capacity = 30;

    [Header("Runtime Data")]
    public InventorySlot[] slots;

    private void OnValidate()
    {
        // Ensure slots array always matches capacity
        if (slots == null || slots.Length != capacity)
        {
            slots = new InventorySlot[capacity];
            for (int i = 0; i < capacity; i++)
            {
                if (slots[i] == null)
                    slots[i] = new InventorySlot();
            }
        }
    }
    public void ClearAllSlots()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Clear();
    }


    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
        {
            Debug.LogError($"Inventory index out of range: {index}");
            return null;
        }
        return slots[index];
    }
}
