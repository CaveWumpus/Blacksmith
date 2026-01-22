using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TransferPanelManager : MonoBehaviour
{
    [Header("Transfer Panel Settings")]
    public int transferSlotCount = 20;   // adjustable in Inspector
    public int columns = 5;              // adjustable in Inspector

    [Header("UI References")]
    public Transform transferPanel;        // drag TransferPanel (GridLayoutGroup) here
    public GameObject transferSlotPrefab;  // drag TransferSlotPrefab here

    private List<TransferSlotUI> transferSlots = new List<TransferSlotUI>();

    public void PopulateFromPlayerInventory(List<PlayerInventory.ItemStack> items)
    {
        foreach (Transform child in transferPanel) Destroy(child.gameObject);
        transferSlots.Clear();

        for (int i = 0; i < transferSlotCount; i++)
        {
            GameObject slotObj = Instantiate(transferSlotPrefab, transferPanel);
            var slotUI = slotObj.GetComponent<TransferSlotUI>();
            transferSlots.Add(slotUI);

            if (i < items.Count)
            {
                var stack = items[i];
                slotUI.SetSlot(stack.itemName, stack.count);
            }
            else
            {
                slotUI.ClearSlot();
            }
        }

        WireSlotNavigation();
        FocusFirstSlot();
    }

    public void CleanupLeftovers()
    {
        foreach (var slot in transferSlots)
        {
            if (!string.IsNullOrEmpty(slot.itemName))
            {
                slot.ClearSlot();
            }
        }
    }

    private void WireSlotNavigation()
    {
        int rows = Mathf.CeilToInt((float)transferSlots.Count / columns);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                if (index >= transferSlots.Count) continue;

                var button = transferSlots[index].GetComponent<Button>();
                if (button == null) continue;

                var nav = new Navigation { mode = Navigation.Mode.Explicit };

                if (row > 0)
                {
                    int upIndex = (row - 1) * columns + col;
                    if (upIndex < transferSlots.Count)
                        nav.selectOnUp = transferSlots[upIndex].GetComponent<Button>();
                }
                if (row < rows - 1)
                {
                    int downIndex = (row + 1) * columns + col;
                    if (downIndex < transferSlots.Count)
                        nav.selectOnDown = transferSlots[downIndex].GetComponent<Button>();
                }
                if (col > 0)
                {
                    int leftIndex = row * columns + (col - 1);
                    if (leftIndex < transferSlots.Count)
                        nav.selectOnLeft = transferSlots[leftIndex].GetComponent<Button>();
                }
                if (col < columns - 1)
                {
                    int rightIndex = row * columns + (col + 1);
                    if (rightIndex < transferSlots.Count)
                        nav.selectOnRight = transferSlots[rightIndex].GetComponent<Button>();
                }

                button.navigation = nav;
            }
        }
    }

    public void FocusFirstSlot()
    {
        if (transferSlots.Count > 0 && transferSlots[0] != null)
        {
            var firstButton = transferSlots[0].GetComponent<Button>();
            if (firstButton != null)
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }
}

