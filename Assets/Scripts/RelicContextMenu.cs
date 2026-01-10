using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RelicContextMenu : MonoBehaviour
{
    public static RelicContextMenu Instance;
    private RelicSlotUI currentSlot;

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

    public void Open(RelicSlotUI slot)
    {
        Debug.Log("RelicContextMenu.Open called.");
        currentSlot = slot;
        gameObject.SetActive(true);
        Debug.Log("RelicContextMenu activated.");

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
        Debug.Log("RelicContextMenu.Close called. Deactivating menu.", this);

        gameObject.SetActive(false);
        //PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.Navigation;

        // Reset focus back to the slot that opened the menu
        if (currentSlot != null)
        {
            Debug.Log($"RelicContextMenu.Close: returning focus to slot {currentSlot.name}", this);
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
        Debug.Log($"[OnNavigate] RelicContextMenu selectedIndex={selectedIndex}");
    }
    private void UpdateHighlight()
    {
        for (int i = 0; i < options.Length; i++)
            options[i].color = (i == selectedIndex) ? Color.yellow : Color.gray;
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
        if (selectedIndex == 0) OnMove();
        else if (selectedIndex == 1) OnDelete();
    }

    private void OnMove()
    {
        PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.MovePending;
        PauseManager.Instance.moveSourceRelicSlot = currentSlot;
        Close();
    }

    private void OnDelete()
    {
        Debug.Log($"[Delete] Relic slot {currentSlot?.name} contains {currentSlot?.relicName} x{currentSlot?.count}. Clearing now.");
        if (currentSlot != null)
        {
            RelicInventory.Instance.RemoveRelic(currentSlot.relicName);
        }
        Close();
    }

}