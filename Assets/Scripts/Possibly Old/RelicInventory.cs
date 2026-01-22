using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class RelicInventory : MonoBehaviour
{
    public static RelicInventory Instance;

    [Header("Relic Settings")]
    public int relicSlotCount = 12;   // adjustable in Inspector
    public int columns = 6;           // adjustable in Inspector

    [Header("UI References")]
    public Transform relicPanel;       // drag RelicPanel here
    public GameObject relicSlotPrefab; // drag RelicSlotPrefab here

    private List<RelicSlot> relicSlots = new List<RelicSlot>();
    private List<RelicSlotUI> uiSlots = new List<RelicSlotUI>();
    public List<RelicSlotUI> UISlots => uiSlots;
    //public IReadOnlyList<RelicSlotUI> UISlots => uiSlots;

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

    void Start()
    {
        // ✅ Wire navigation AFTER slots exist
        int rows = Mathf.CeilToInt((float)relicSlotCount / columns);
        WireSlotNavigation(rows, columns);
    }

    public void AddRelic(MineableDrop drop)
    {
        for (int i = 0; i < relicSlots.Count; i++)
        {
            if (!relicSlots[i].occupied)
            {
                relicSlots[i].relicName = drop.dropName;
                relicSlots[i].occupied = true;
                relicSlots[i].count = 1;

                uiSlots[i].SetSlot(drop.icon, 1, drop.dropName);
                return;
            }
        }
    }

    public void RemoveRelic(string relicName)
    {
        for (int i = 0; i < relicSlots.Count; i++)
        {
            if (relicSlots[i].occupied && relicSlots[i].relicName == relicName)
            {
                relicSlots[i].occupied = false;
                relicSlots[i].relicName = "";
                relicSlots[i].count = 0;
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
            toSlot.count = fromSlot.count;
            toSlot.occupied = true;

            uiSlots[toIndex].SetSlot(from.icon.sprite, fromSlot.count, fromSlot.relicName);

            // Clear source
            fromSlot.relicName = "";
            fromSlot.count = 0;
            fromSlot.occupied = false;
            from.ClearSlot();
        }
        else
        {
            // Swap relics
            string tempName = toSlot.relicName;
            int tempCount = toSlot.count;
            Sprite tempIcon = to.icon.sprite;

            toSlot.relicName = fromSlot.relicName;
            toSlot.count = fromSlot.count;
            uiSlots[toIndex].SetSlot(from.icon.sprite, fromSlot.count, fromSlot.relicName);

            fromSlot.relicName = tempName;
            fromSlot.count = tempCount;
            uiSlots[fromIndex].SetSlot(tempIcon, tempCount, tempName);
        }
    }

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
        if (uiSlots == null || uiSlots.Count == 0) return;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                if (index >= uiSlots.Count) continue;

                var slotUI = uiSlots[index];
                if (slotUI == null) continue; // NEW

                var button = uiSlots[index].GetComponent<Button>();
                if (button == null) continue;

                var nav = new Navigation { mode = Navigation.Mode.Explicit };

                if (row > 0 && (row - 1) * cols + col < uiSlots.Count)
                {
                    var neighbor = uiSlots[(row - 1) * cols + col];
                    if (neighbor != null)
                        nav.selectOnUp = uiSlots[(row - 1) * cols + col].GetComponent<Button>();
                }

                if (row < rows - 1 && (row + 1) * cols + col < uiSlots.Count)
                {
                    var neighbor = uiSlots[(row + 1) * cols + col];
                    if (neighbor != null)
                        nav.selectOnDown = uiSlots[(row + 1) * cols + col].GetComponent<Button>();
                }

                if (col > 0 && row * cols + (col - 1) < uiSlots.Count)
                {
                    var neighbor = uiSlots[row * cols + (col - 1)];
                    if (neighbor != null)
                        nav.selectOnLeft = uiSlots[row * cols + (col - 1)].GetComponent<Button>();
                }

                if (col < cols - 1 && row * cols + (col + 1) < uiSlots.Count)
                {
                    var neighbor = uiSlots[row * cols + (col + 1)];
                    if (neighbor != null)
                        nav.selectOnRight = uiSlots[row * cols + (col + 1)].GetComponent<Button>();
                }

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
    public int count;
}
