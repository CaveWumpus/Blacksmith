using UnityEngine;
using System.Collections.Generic;

public class ShopStorageChest : MonoBehaviour
{
    public static ShopStorageChest Instance;
    public List<ItemStack> storedItems = new List<ItemStack>();

    [System.Serializable]
    public class ItemStack
    {
        public string itemName;
        public int count;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void AddItem(string itemName, int count)
    {
        var stack = storedItems.Find(s => s.itemName == itemName);
        if (stack != null) stack.count += count;
        else storedItems.Add(new ItemStack { itemName = itemName, count = count });
    }
    
    public bool HasMaterials(ItemUI.MaterialRequirement[] requirements)
    {
        foreach (var req in requirements)
        {
            var stack = storedItems.Find(s => s.itemName == req.materialName);
            if (stack == null || stack.count < req.amount)
            {
                return false; // requirement not met
            }
        }
        return true;
    }

}
