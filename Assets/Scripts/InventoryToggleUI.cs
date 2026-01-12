using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryToggleUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject backpackPanel;
    public GameObject relicPanel;

    public bool IsBackpackActive { get; private set; }

    public void ShowBackpack()
    {
        backpackPanel.SetActive(true);
        relicPanel.SetActive(false);
        IsBackpackActive = true;

        // Reset focus to first player slot if available
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.UISlots.Count > 0)
        {
            var firstSlot = PlayerInventory.Instance.UISlots[0].gameObject;
            EventSystem.current.SetSelectedGameObject(firstSlot);
            EventSystem.current.firstSelectedGameObject = firstSlot;
        }
    }

    public void ShowRelics()
    {
        backpackPanel.SetActive(false);
        relicPanel.SetActive(true);
        IsBackpackActive = false;

        // Reset focus to first relic slot if available
        if (RelicInventory.Instance != null && RelicInventory.Instance.UISlots.Count > 0)
        {
            var firstRelic = RelicInventory.Instance.UISlots[0].gameObject;
            EventSystem.current.SetSelectedGameObject(firstRelic);
            EventSystem.current.firstSelectedGameObject = firstRelic;
        }
    }

    public void HideAllPanels()
    {
        backpackPanel.SetActive(false);
        relicPanel.SetActive(false);
        IsBackpackActive = false;
    }


    public void ToggleLeftRight(Vector2 nav)
    {
        if (nav.x > 0.7f)
            ShowRelics();
        else if (nav.x < -0.7f)
            ShowBackpack();
    }
}
