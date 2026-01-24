using System;
using System.Collections.Generic;
using UnityEngine;

public class MiningInventoryController : MonoBehaviour
{
    public event Action OnInventoryChanged;
    public event Action OnInventoryFull;
    public event Action OnInventoryNearFull;

    [SerializeField] private int capacity = 20;
    [SerializeField] private float nearFullThreshold = 0.8f;

    private List<InventoryItemData> items = new List<InventoryItemData>();

    public int Capacity => capacity;
    public int Count => items.Count;
    public bool IsFull => items.Count >= capacity;
    

    public static MiningInventoryController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // ---------------------------------------------------------
    // Public API
    // ---------------------------------------------------------
    public bool TryAddItem(InventoryItemData item)
    {
        // Relics should never enter the mining inventory
        if (item.itemType == ItemType.Special)
        {
            Debug.LogWarning("Attempted to add a relic to mining inventory. Ignored.");
            return false;
        }

        // 1. Try stacking first
        for (int i = 0; i < items.Count; i++)
        {
            InventoryItemData existing = items[i];

            if (existing.itemID == item.itemID &&
                existing.currentStack < existing.maxStack)
            {
                existing.currentStack++;
                items[i] = existing; // structs must be reassigned
                OnInventoryChanged?.Invoke();
                CheckFullnessWarnings();
                return true;
            }
        }

        // 2. If no stack found, try adding a new slot
        if (IsFull)
        {
            OnInventoryFull?.Invoke();
            return false;
        }

        // Create a new instance for the slot
        InventoryItemData newItem = item;
        newItem.currentStack = 1;

        items.Add(newItem);
        AutoSort();
        CheckFullnessWarnings();
        OnInventoryChanged?.Invoke();
        return true;
    }



    public InventoryItemData DropItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return default;

        InventoryItemData removed = items[index];
        items.RemoveAt(index);

        AutoSort();
        OnInventoryChanged?.Invoke();
        return removed;
    }

    public List<InventoryItemData> GetItemsSorted()
    {
        return new List<InventoryItemData>(items);
    }

    public List<int> GetSmartDropCandidates()
    {
        List<int> indices = new List<int>();

        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i].isOrderRelevant)
                indices.Add(i);
        }

        return indices;
    }

    // ---------------------------------------------------------
    // Sorting
    // ---------------------------------------------------------
    private void AutoSort()
    {
        items.Sort((a, b) =>
        {
            int orderCompare = b.isOrderRelevant.CompareTo(a.isOrderRelevant);
            if (orderCompare != 0) return orderCompare;

            int rarityCompare = b.rarity.CompareTo(a.rarity);
            if (rarityCompare != 0) return rarityCompare;

            return a.itemType.CompareTo(b.itemType);
        });
    }

    // ---------------------------------------------------------
    // Fullness Warnings
    // ---------------------------------------------------------
    private void CheckFullnessWarnings()
    {
        float fullness = (float)items.Count / capacity;

        if (fullness >= 1f)
            OnInventoryFull?.Invoke();
        else if (fullness >= nearFullThreshold)
            OnInventoryNearFull?.Invoke();
    }
}
