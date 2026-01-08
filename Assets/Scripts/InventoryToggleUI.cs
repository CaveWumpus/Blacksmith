using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryToggleUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject backpackPanel;
    public GameObject relicPanel;

    //private bool inInventory = false;

    private bool showingBackpack = true;

    public void ShowBackpack()
    {
        backpackPanel.SetActive(true);
        relicPanel.SetActive(false);

        // Select first backpack slot
        var eventSystem = EventSystem.current;
        if (eventSystem != null && PlayerInventory.Instance.UISlots.Count > 0)
        {
            eventSystem.SetSelectedGameObject(PlayerInventory.Instance.UISlots[0].gameObject);
        }
    }

    public void ShowRelics()
    {
        backpackPanel.SetActive(false);
        relicPanel.SetActive(true);

        // Select first relic slot
        var eventSystem = EventSystem.current;
        if (eventSystem != null && RelicInventory.Instance.UISlots.Count > 0)
        {
            eventSystem.SetSelectedGameObject(RelicInventory.Instance.UISlots[0].gameObject);
        }
    }

    public void ToggleLeftRight(Vector2 nav)
    {
        if (nav.x > 0.7f)
            ShowRelics();
        else if (nav.x < -0.7f)
            ShowBackpack();
    }

    public void EnterBackpackInventory()
    {
        backpackPanel.SetActive(true);
        relicPanel.SetActive(false);

        // Assuming PlayerInventory.Instance.uiSlots holds your slot UIs
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.UISlots.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(PlayerInventory.Instance.UISlots[0].gameObject);
        }
    }

    public void EnterRelicInventory()
    {
        backpackPanel.SetActive(false);
        relicPanel.SetActive(true);
        if (RelicInventory.Instance != null && RelicInventory.Instance.UISlots.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(RelicInventory.Instance.UISlots[0].gameObject);
        }
    }
}
