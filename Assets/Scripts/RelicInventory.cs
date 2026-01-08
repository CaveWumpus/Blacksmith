using UnityEngine;
using System.Collections.Generic;

public class RelicInventory : MonoBehaviour
{
    public static RelicInventory Instance;

    [Header("Relic Settings")]
    public int relicSlotCount = 12;   // adjustable in Inspector

    [Header("UI References")]
    public Transform relicPanel;      // drag RelicPanel here
    public GameObject relicSlotPrefab; // drag RelicSlotPrefab here

    private List<RelicSlot> relicSlots = new List<RelicSlot>();
    private List<RelicSlotUI> uiSlots = new List<RelicSlotUI>();
    public IReadOnlyList<RelicSlotUI> UISlots => uiSlots;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize empty relic slots
        for (int i = 0; i < relicSlotCount; i++)
        {
            relicSlots.Add(new RelicSlot());

            GameObject slotObj = Instantiate(relicSlotPrefab, relicPanel);
            RelicSlotUI ui = slotObj.GetComponent<RelicSlotUI>();
            uiSlots.Add(ui);
            ui.ClearSlot(); // start empty
        }
    }

    public void AddRelic(MineableDrop drop)
    {
        for (int i = 0; i < relicSlots.Count; i++)
        {
            if (!relicSlots[i].occupied)
            {
                relicSlots[i].relicName = drop.dropName;
                relicSlots[i].occupied = true;
                uiSlots[i].SetRelic(drop.icon, drop.dropName);
                return;
            }
        }

        Debug.Log("[RelicInventory] Relic inventory full!");
    }

    // ✅ Remove relic
    public void RemoveRelic(string relicName)
    {
        for (int i = 0; i < relicSlots.Count; i++)
        {
            if (relicSlots[i].occupied && relicSlots[i].relicName == relicName)
            {
                relicSlots[i].occupied = false;
                relicSlots[i].relicName = "";
                uiSlots[i].ClearSlot();
                return;
            }
        }
    }

    // ✅ Query relic ownership
    public bool HasRelic(string relicName)
    {
        for (int i = 0; i < relicSlots.Count; i++)
        {
            if (relicSlots[i].occupied && relicSlots[i].relicName == relicName)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class RelicSlot
{
    public string relicName;
    public bool occupied;
}
