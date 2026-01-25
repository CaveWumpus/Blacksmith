using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class MiningInventoryOverlayUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject backgroundDim;          // The dim panel behind the list
    public RectTransform smartListPanel;      // The panel that resizes in HUD/Drop Mode

    [Header("List")]
    public Transform contentParent;
    public GameObject slotPrefab;

    [Header("Scroll")]
    public ScrollRect scrollRect;
    public Scrollbar verticalScrollbar;


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

        //if (slotPrefab != null)
            //slotPrefab.sizeDelta = new Vector2(240, 40);

        // Disable scrolling in HUD mode
        scrollRect.vertical = false;
        verticalScrollbar.gameObject.SetActive(false);

        foreach (var slot in slots)
        {
            slot.SetHUDSize();
            slot.itemNameText.gameObject.SetActive(true);
            slot.selectionFrameGroup.alpha = 0;
            slot.smartDropGlowGroup.alpha = 0;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);


    }

    public void SetDropMode()
    {
        if (backgroundDim != null)
            backgroundDim.SetActive(true);

        if (smartListPanel != null)
            smartListPanel.sizeDelta = new Vector2(450, 600);

        // Enable scrolling in Drop Mode
        scrollRect.vertical = true;
        verticalScrollbar.gameObject.SetActive(true);

        foreach (var slot in slots)
        {
            slot.SetDropModeSize();
            Debug.Log("Drop mode size applied");
            slot.itemNameText.gameObject.SetActive(true);
        }
        //LayoutRebuilder.ForceRebuildLayoutImmediate(smartListPanel);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);


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
    public void ScrollToSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return;

        RectTransform slot = slots[index].GetComponent<RectTransform>();
        RectTransform content = contentParent.GetComponent<RectTransform>();
        RectTransform viewport = scrollRect.viewport;

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;
        float scrollableHeight = contentHeight - viewportHeight;

        if (scrollableHeight <= 0f)
        {
            // Nothing to scroll
            scrollRect.verticalNormalizedPosition = 1f;
            return;
        }

        // Slot position in content space (pivot top, y goes negative downward)
        Vector2 slotLocalPos = content.InverseTransformPoint(slot.position);

        // Distance from top of content to slot center
        float slotCenterFromTop = -slotLocalPos.y - (slot.rect.height * 0.5f);

        // We want the slot center roughly in the middle of the viewport
        float desiredFromTop = slotCenterFromTop - (viewportHeight * 0.5f);

        // Clamp so we don't scroll past top or bottom
        desiredFromTop = Mathf.Clamp(desiredFromTop, 0f, scrollableHeight);

        // ScrollRect: 1 = top, 0 = bottom
        float normalized = 1f - (desiredFromTop / scrollableHeight);

        scrollRect.verticalNormalizedPosition = normalized;
    }


}
