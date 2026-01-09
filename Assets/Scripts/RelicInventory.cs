using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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

        // ✅ Wire navigation after slots exist
        int rows = relicSlotCount / 6; // assuming 6 columns
        WireSlotNavigation(rows, cols: 6); // 2 rows × 6 columns = 12 slots
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

    public void MoveItem(RelicSlotUI from, RelicSlotUI to)
    {
        int fromIndex = uiSlots.IndexOf(from);
        int toIndex = uiSlots.IndexOf(to);

        if (fromIndex == -1 || toIndex == -1) return;
        var fromSlot = relicSlots[fromIndex];
        var toSlot = relicSlots[toIndex];
        if (!toSlot.occupied)
        {
            // Move into empty slot
            toSlot.relicName = fromSlot.relicName;
            toSlot.occupied = true;
            uiSlots[toIndex].SetRelic(from.icon.sprite, fromSlot.relicName);

            // Clear source
            fromSlot.relicName = "";
            fromSlot.occupied = false;
            from.ClearSlot();
        }
        else
        {
            // Swap relics
            string tempName = toSlot.relicName;
            Sprite tempIcon = to.icon.sprite;

            toSlot.relicName = fromSlot.relicName;
            uiSlots[toIndex].SetRelic(from.icon.sprite, fromSlot.relicName);

            fromSlot.relicName = tempName;
            uiSlots[fromIndex].SetRelic(tempIcon, tempName);
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
    public void WireSlotNavigation(int rows, int cols)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                var button = uiSlots[index].GetComponent<Button>();
                var nav = new Navigation { mode = Navigation.Mode.Explicit };
                // Up
                if (row > 0)
                    nav.selectOnUp = uiSlots[(row - 1) * cols + col].GetComponent<Button>();

                // Down
                if (row < rows - 1)
                    nav.selectOnDown = uiSlots[(row + 1) * cols + col].GetComponent<Button>();

                // Left
                if (col > 0)
                    nav.selectOnLeft = uiSlots[row * cols + (col - 1)].GetComponent<Button>();

                // Right
                if (col < cols - 1)
                    nav.selectOnRight = uiSlots[row * cols + (col + 1)].GetComponent<Button>();

                button.navigation = nav;
            }
        }   
    }
}

[System.Serializable]
public class RelicSlot
{
    public string relicName;
    public bool occupied;
}
