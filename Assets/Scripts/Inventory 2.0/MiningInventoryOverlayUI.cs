using UnityEngine;
using System.Collections.Generic;

public class MiningInventoryOverlayUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject backgroundDim;          // The dim panel behind the list
    public RectTransform smartListPanel;      // The panel that resizes in HUD/Drop Mode

    [Header("List")]
    public Transform contentParent;
    public GameObject slotPrefab;

    private List<SlotUI> slots = new List<SlotUI>();


    // ---------------------------------------------------------
    // Build / Refresh
    // ---------------------------------------------------------
    public void BuildList(List<InventoryItemData> items)
    {
        // Clear old
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        slots.Clear();

        // Build new
        foreach (var item in items)
        {
            GameObject go = Instantiate(slotPrefab, contentParent);
            SlotUI ui = go.GetComponent<SlotUI>();
            ui.SetData(item);
            slots.Add(ui);
        }
    }

    // ---------------------------------------------------------
    // Selection
    // ---------------------------------------------------------
    public void HighlightSlot(int index)
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].SetSelected(i == index);
    }

    // ---------------------------------------------------------
    // Smart Drop
    // ---------------------------------------------------------
    public void HighlightSmartDropCandidates(List<int> indices)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            bool isSmart = indices.Contains(i);
            slots[i].SetSmartDrop(isSmart);
        }
    }

    // ---------------------------------------------------------
    // Drop Animation
    // ---------------------------------------------------------
    public void PlayDropAnimation(int index)
    {
        if (index < 0 || index >= slots.Count)
            return;

        // Simple fade-out
        slots[index].rootCanvasGroup.alpha = 0.2f;
    }

    // ---------------------------------------------------------
    // Drop Mode Visuals
    // ---------------------------------------------------------
    public void EnableDropModeVisuals()
    {
        gameObject.SetActive(true);
    }

    public void DisableDropModeVisuals()
    {
        gameObject.SetActive(false);
    }
    public void SetHUDMode()
    {
        if (backgroundDim != null)
            backgroundDim.SetActive(false);

        if (smartListPanel != null)
            smartListPanel.sizeDelta = new Vector2(260, 220);

        foreach (var slot in slots)
        {
            slot.itemNameText.gameObject.SetActive(true);
            slot.selectionFrameGroup.alpha = 0;
            slot.smartDropGlowGroup.alpha = 0;
        }
    }

    public void SetDropMode()
    {
        if (backgroundDim != null)
            backgroundDim.SetActive(true);

        if (smartListPanel != null)
            smartListPanel.sizeDelta = new Vector2(450, 600);

        foreach (var slot in slots)
        {
            slot.itemNameText.gameObject.SetActive(true);
        }
    }
    private void OnEnable()
    {
        MiningInventoryController.Instance.OnInventoryChanged += RebuildHUD;
    }

    private void OnDisable()
    {
        MiningInventoryController.Instance.OnInventoryChanged -= RebuildHUD;
    }
    private void RebuildHUD()
    {
        BuildList(MiningInventoryController.Instance.GetItemsSorted());
        SetHUDMode();
    }

}
