using UnityEngine;

public class InventoryToggleUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject inventoryPage;     // The unified inventory grid
    public GameObject evacuationPanel;   // The evacuation UI

    public void ShowInventory()
    {
        inventoryPage.SetActive(true);
        if (evacuationPanel != null)
            evacuationPanel.SetActive(false);
    }

    public void ShowEvacuation()
    {
        inventoryPage.SetActive(false);
        if (evacuationPanel != null)
            evacuationPanel.SetActive(true);
    }

    public void HideAll()
    {
        inventoryPage.SetActive(false);
        if (evacuationPanel != null)
            evacuationPanel.SetActive(false);
    }
}
