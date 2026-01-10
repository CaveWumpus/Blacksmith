using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryContextMenu : MonoBehaviour
{
    public static InventoryContextMenu Instance;
    private InventorySlotUI currentSlot;

    [SerializeField] private Image moveOption;
    [SerializeField] private Image deleteOption;

    private int selectedIndex = 0; // 0 = Move, 1 = Delete
    private Image[] options;

    private PlayerControls controls;

    void Awake()
    {
        Instance = this;
        //gameObject.SetActive(false); // start hidden

        options = new Image[] { moveOption, deleteOption };

        controls = new PlayerControls();
    }

    void Start()
    {
        // Hide after Awake has finished
        gameObject.SetActive(false);
    }
    
    void OnEnable()
    {
        // Hook into UI actions
        controls.UI.Navigate.performed += OnNavigate;
        controls.UI.Submit.performed += OnConfirm;
        controls.UI.Cancel.performed += OnCancel;
        controls.UI.Enable();
    }

    void OnDisable()
    {
        controls.UI.Submit.performed -= OnConfirm;
        controls.UI.Cancel.performed -= OnCancel;
        controls.UI.Disable();
    }

    public void Open(InventorySlotUI slot)
    {
        Debug.Log("InventoryContextMenu.Open called.");
        currentSlot = slot;
        gameObject.SetActive(true);
        Debug.Log("InventoryContextMenu activated.");

        PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.ContextMenu;

        selectedIndex = 0;
        UpdateHighlight();

        // Focus first option (Move)
        EventSystem.current.SetSelectedGameObject(moveOption.gameObject);
        EventSystem.current.firstSelectedGameObject = moveOption.gameObject;
        Debug.Log("Focus set to Move option.");
    }

    public void Close()
    {
        Debug.Log("InventoryContextMenu.Close called. Deactivating menu.", this);

        gameObject.SetActive(false);
        //PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.Navigation;

        // Reset focus back to the slot that opened the menu
        if (currentSlot != null)
        {
            Debug.Log($"InventoryContextMenu.Close: returning focus to slot {currentSlot.name}", this);
            EventSystem.current.SetSelectedGameObject(currentSlot.gameObject);
            EventSystem.current.firstSelectedGameObject = currentSlot.gameObject;
        }

        currentSlot = null;
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (PauseManager.Instance.inventoryMode != PauseManager.InventoryMode.ContextMenu)
            return;

        Vector2 nav = ctx.ReadValue<Vector2>();
        if (nav.y < -0.5f) selectedIndex = 1; // down → Delete
        else if (nav.y > 0.5f) selectedIndex = 0; // up → Move

        UpdateHighlight();
        Debug.Log($"[OnNavigate] InventoryContextMenu selectedIndex={selectedIndex}");
    }
    private void UpdateHighlight()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].color = (i == selectedIndex) ? Color.yellow : Color.gray;
            Debug.Log($"[UpdateHighlight] Option={options[i].name}, Index={i}, Selected={i==selectedIndex}");
        }
    }

    // Called when Submit (A/Enter) is pressed
    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (PauseManager.Instance.inventoryMode != PauseManager.InventoryMode.ContextMenu)
            return;

        ExecuteOption();
    }

    // Called when Cancel (B/Escape) is pressed
    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (PauseManager.Instance.inventoryMode != PauseManager.InventoryMode.ContextMenu)
            return;

        Close();
    }

    private void ExecuteOption()
    {
        Debug.Log($"[ExecuteOption] selectedIndex={selectedIndex}");
        if (selectedIndex == 0) OnMove();
        else if (selectedIndex == 1) OnDelete();
    }

    private void OnMove()
    {
        PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.MovePending;
        PauseManager.Instance.moveSourceSlot = currentSlot;
        Close();
    }

    private void OnDelete()
    {
        Debug.Log($"[Delete] Player slot {currentSlot?.name} contains {currentSlot?.itemName} x{currentSlot?.count}. Clearing now.");
        if (currentSlot != null)
        {
            PlayerInventory.Instance.RemoveItem(currentSlot.itemName, currentSlot.count);
        }
        Close();
    }

}
