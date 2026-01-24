using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ChestPanelManager : MonoBehaviour
{
    [Header("Chest Panel Settings")]
    public int chestSlotCount = 50;      // adjustable in Inspector
    public int columns = 5;              // adjustable in Inspector

    [Header("UI References")]
    public Transform chestPanel;        // drag ChestPanel (GridLayoutGroup) here
    public GameObject chestSlotPrefab;  // drag ChestSlotPrefab here

    private List<ChestSlotUI> chestSlots = new List<ChestSlotUI>();

    public void RefreshChestUI()
    {
        foreach (Transform child in chestPanel) Destroy(child.gameObject);
        chestSlots.Clear();

        for (int i = 0; i < chestSlotCount; i++)
        {
            GameObject slotObj = Instantiate(chestSlotPrefab, chestPanel);
            var slotUI = slotObj.GetComponent<ChestSlotUI>();
            chestSlots.Add(slotUI);

            if (ShopStorageChest.Instance != null && i < ShopStorageChest.Instance.storedItems.Count)
            {
                var stack = ShopStorageChest.Instance.storedItems[i];
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

    private void WireSlotNavigation()
    {
        int rows = Mathf.CeilToInt((float)chestSlots.Count / columns);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                if (index >= chestSlots.Count) continue;

                var button = chestSlots[index].GetComponent<Button>();
                if (button == null) continue;

                var nav = new Navigation { mode = Navigation.Mode.Explicit };

                if (row > 0)
                {
                    int upIndex = (row - 1) * columns + col;
                    if (upIndex < chestSlots.Count)
                        nav.selectOnUp = chestSlots[upIndex].GetComponent<Button>();
                }
                if (row < rows - 1)
                {
                    int downIndex = (row + 1) * columns + col;
                    if (downIndex < chestSlots.Count)
                        nav.selectOnDown = chestSlots[downIndex].GetComponent<Button>();
                }
                if (col > 0)
                {
                    int leftIndex = row * columns + (col - 1);
                    if (leftIndex < chestSlots.Count)
                        nav.selectOnLeft = chestSlots[leftIndex].GetComponent<Button>();
                }
                if (col < columns - 1)
                {
                    int rightIndex = row * columns + (col + 1);
                    if (rightIndex < chestSlots.Count)
                        nav.selectOnRight = chestSlots[rightIndex].GetComponent<Button>();
                }

                button.navigation = nav;
            }
        }
    }

    public void FocusFirstSlot()
    {
        if (chestSlots.Count > 0 && chestSlots[0] != null)
        {
            var firstButton = chestSlots[0].GetComponent<Button>();
            if (firstButton != null)
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }
}

