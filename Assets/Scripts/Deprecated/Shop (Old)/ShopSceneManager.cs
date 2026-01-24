using UnityEngine;
using UnityEngine.EventSystems;

public class ShopSceneManager : MonoBehaviour
{
    [Header("Managers")]
    public TransferPanelManager transferPanelManager;   // drag TransferPanelManager here
    public ChestPanelManager chestPanelManager;         // drag ChestPanelManager here

    [Header("Panels")]
    public GameObject transferPanel;     // drag TransferPanel UI here
    public GameObject chestPanel;        // drag ChestPanel UI here
    public GameObject shopOptionsPanel;  // drag ShopOptionsPanel UI here
    public GameObject craftingMenuPanel; // drag CraftingMenuPanel UI here

    [Header("Buttons")]
    public GameObject acceptButton;           // drag AcceptButton here
    public GameObject shopOptionsFirstButton; // drag first shop option button here

    void Start()
    {
        // Initial panel states
        transferPanel.SetActive(true);
        shopOptionsPanel.SetActive(false);
        craftingMenuPanel.SetActive(false);

        // Populate transfer panel from PlayerInventory
        if (PlayerInventory.Instance != null)
        {
            transferPanelManager.PopulateFromPlayerInventory(PlayerInventory.Instance.oreStacks);
            transferPanelManager.PopulateFromPlayerInventory(PlayerInventory.Instance.gemStacks);
        }
        else
        {
            Debug.LogError("PlayerInventory.Instance is null! Make sure PlayerInventory exists in the first scene.");
        }

        // Populate chest panel from ShopStorageChest
        if (ShopStorageChest.Instance != null)
        {
            chestPanelManager.RefreshChestUI();
        }
        else
        {
            Debug.LogError("ShopStorageChest.Instance is null! Make sure ShopStorageChest exists in the first scene.");
        }

        // Fallback focus
        if (transferPanel.transform.childCount > 0)
            EventSystem.current.SetSelectedGameObject(transferPanel.transform.GetChild(0).gameObject);
        else
            EventSystem.current.SetSelectedGameObject(acceptButton);
    }

    public void OnAcceptTransfer()
    {
        // Cleanup leftovers in transfer panel
        transferPanelManager.CleanupLeftovers();

        // Hide transfer panel, show shop options
        transferPanel.SetActive(false);
        shopOptionsPanel.SetActive(true);
        acceptButton.SetActive(false);
        chestPanel.SetActive(false); // assuming ChestPanelManager is attached to the ChestPanel GameObject

        // Refresh chest UI now that items were added
        chestPanelManager.RefreshChestUI();
        chestPanelManager.FocusFirstSlot();

        // Move focus to first shop option
        EventSystem.current.SetSelectedGameObject(shopOptionsFirstButton);
    }

    // Shop option handlers
    public void OpenCraftingMenu()
    {
        shopOptionsPanel.SetActive(false);
        craftingMenuPanel.SetActive(true);
    }

    public void CloseCraftingMenu()
    {
        craftingMenuPanel.SetActive(false);
        shopOptionsPanel.SetActive(true);
    }

    public void OpenChest()
    {
        chestPanelManager.RefreshChestUI();
        chestPanelManager.FocusFirstSlot();
        Debug.Log("Chest opened.");
    }

    public void OpenOrders()
    {
        Debug.Log("Orders opened.");
    }
}
