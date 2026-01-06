using UnityEngine;

public class InventoryToggleUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject backpackPanel;  // drag BackpackPanel here
    public GameObject relicPanel;     // drag RelicPanel here

    public void ShowBackpack()
    {
        backpackPanel.SetActive(true);
        relicPanel.SetActive(false);
    }

    public void ShowRelics()
    {
        backpackPanel.SetActive(false);
        relicPanel.SetActive(true);
    }
}
