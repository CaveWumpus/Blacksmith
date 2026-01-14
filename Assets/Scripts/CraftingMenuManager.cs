using UnityEngine;
using TMPro;

public class CraftingMenuManager : MonoBehaviour
{
    [Header("Tab Panels")]
    public GameObject ordersPanel;
    public GameObject recipesPanel;
    public GameObject smeltingPanel;
    public GameObject gemCuttingPanel;
    public GameObject patternsPanel;

    [Header("UI Controls")]
    public TMP_InputField searchBox;
    public TMP_Dropdown sortDropdown;

    private GameObject currentPanel;

    void Start()
    {
        ShowPanel(ordersPanel); // default tab

        // Wire events
        sortDropdown.onValueChanged.AddListener(OnSortChanged);
        searchBox.onValueChanged.AddListener(OnSearchChanged);
    }

    // Tab switching
    public void ShowOrders() => ShowPanel(ordersPanel);
    public void ShowRecipes() => ShowPanel(recipesPanel);
    public void ShowSmelting() => ShowPanel(smeltingPanel);
    public void ShowGemCutting() => ShowPanel(gemCuttingPanel);
    public void ShowPatterns() => ShowPanel(patternsPanel);

    private void ShowPanel(GameObject panel)
    {
        if (currentPanel != null) currentPanel.SetActive(false);
        currentPanel = panel;
        currentPanel.SetActive(true);

        // Reapply search/sort when switching tabs
        OnSortChanged(sortDropdown.value);
        OnSearchChanged(searchBox.text);
    }

    // Sorting logic
    public void OnSortChanged(int index)
    {
        switch (index)
        {
            case 0: SortByMaterialsInInventory(); break;
            case 1: SortByLevelRange(); break;
            case 2: SortByAll(); break;
        }
    }

    private void SortByMaterialsInInventory()
    {
        foreach (var itemUI in currentPanel.GetComponentsInChildren<ItemUI>())
        {
            bool canCraft = ShopStorageChest.Instance.HasMaterials(itemUI.requiredMaterials);
            itemUI.gameObject.SetActive(canCraft);
        }
    }

    private void SortByLevelRange()
    {
        int playerLevel = PlayerStats.Instance.Level;
        int minLevel = playerLevel - 5;
        int maxLevel = playerLevel + 5;

        foreach (var itemUI in currentPanel.GetComponentsInChildren<ItemUI>())
        {
            itemUI.gameObject.SetActive(itemUI.requiredLevel >= minLevel && itemUI.requiredLevel <= maxLevel);
        }
    }

    private void SortByAll()
    {
        foreach (var itemUI in currentPanel.GetComponentsInChildren<ItemUI>())
        {
            itemUI.gameObject.SetActive(true);
        }
    }

    // Search logic
    public void OnSearchChanged(string query)
    {
        query = query.ToLower();

        foreach (var itemUI in currentPanel.GetComponentsInChildren<ItemUI>())
        {
            bool matches = itemUI.itemName.ToLower().Contains(query);
            itemUI.gameObject.SetActive(matches);
        }
    }
}
