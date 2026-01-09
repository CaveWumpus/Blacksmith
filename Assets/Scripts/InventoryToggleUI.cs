using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryToggleUI : MonoBehaviour
{
    public GameObject backpackPanel;
    public GameObject relicPanel;

    // Track which panel is active
    public bool IsBackpackActive { get; private set; }

    // Optional: reference to the toggle buttons if you have them
    public GameObject backpackButton;
    public GameObject relicButton;

    public GameObject CurrentPanelButton
    {
        get
        {
            return IsBackpackActive ? backpackButton : relicButton;
        }
    }

    public void ShowBackpack()
    {
        backpackPanel.SetActive(true);
        relicPanel.SetActive(false);
        IsBackpackActive = true;
    }

    public void ShowRelics()
    {
        backpackPanel.SetActive(false);
        relicPanel.SetActive(true);
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
