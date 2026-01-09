using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public GameObject pauseMenuPanel;   // drag your PauseMenuPanel here
    public InventoryToggleUI inventoryToggleUI; // drag your toggle script here
    public PlayerController playerController; // drag your PlayerController here

    private PlayerControls controls;
    private bool isPaused = false;

    public enum PauseMenuMode
    {
        PanelSelect,
        Inventory
    }
    public enum InventoryMode
    {
        Navigation,   // moving around slots
        ContextMenu,  // move/split/delete choices
        MovePending   // waiting for destination slot
    }
    public InventoryMode inventoryMode = InventoryMode.Navigation;

    // Keep track of source slot when moving
    public InventorySlotUI moveSourceSlot;
    public RelicSlotUI moveSourceRelicSlot;

    public PauseMenuMode currentMode = PauseMenuMode.PanelSelect;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controls = new PlayerControls();
        playerController.controls = controls; // inject shared instance
    }

    void OnEnable()
    {
        // Global pause always enabled
        controls.Global.Pause.performed += OnPause;
        controls.Global.Enable();

        // Only UI navigation enabled when paused
        controls.UI.Navigate.performed += OnNavigate;
        controls.UI.Submit.performed += OnSubmit;
        controls.UI.Cancel.performed += OnCancel;
        controls.UI.Disable();

        // Gameplay enabled by default
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Global.Pause.performed -= OnPause;
        controls.Global.Disable();

        controls.UI.Navigate.performed -= OnNavigate;
        controls.UI.Submit.performed -= OnSubmit;
        controls.UI.Cancel.performed -= OnCancel;
        controls.UI.Disable();

        controls.Player.Disable();
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 nav = ctx.ReadValue<Vector2>();
        //if (Mathf.Abs(nav.x) > 0.7f)
        //{
        //    inventoryToggleUI.ToggleLeftRight(nav);
        //}
        if (currentMode == PauseMenuMode.PanelSelect)
        {
            if (Mathf.Abs(nav.x) > 0.7f)
                inventoryToggleUI.ToggleLeftRight(nav);
        }
        else if (currentMode == PauseMenuMode.Inventory)
        {
        // Let EventSystem handle slot navigation (no left/right switching here)
        }
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (currentMode == PauseMenuMode.PanelSelect)
        {
            // Enter whichever panel is active
            currentMode = PauseMenuMode.Inventory;
            inventoryMode = InventoryMode.Navigation;
            if (inventoryToggleUI.IsBackpackActive)
                EventSystem.current.SetSelectedGameObject(PlayerInventory.Instance.UISlots[0].gameObject);
            else
                EventSystem.current.SetSelectedGameObject(RelicInventory.Instance.UISlots[0].gameObject);
        }
        //else if (currentMode == PauseMenuMode.Inventory)
        //{
            // Submit is handled by the selected slot’s Button → OnSlotClicked()
        //}
    }

    private void OnCancel(InputAction.CallbackContext ctx)
{
    if (currentMode == PauseMenuMode.Inventory)
    {
        if (inventoryMode == InventoryMode.ContextMenu)
        {
            InventoryContextMenu.Instance.Close();
            inventoryMode = InventoryMode.Navigation;
        }
        else if (inventoryMode == InventoryMode.MovePending)
        {
            moveSourceSlot = null;
            inventoryMode = InventoryMode.Navigation;
        }
        else
        {
            currentMode = PauseMenuMode.PanelSelect;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    else if (currentMode == PauseMenuMode.PanelSelect)
    {
        ResumeGame();
    }
}

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Debug.Log("Player map enabled? " + controls.Player.enabled);

        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);

        // Switch to UI controls only
        controls.Player.Disable();
        controls.UI.Enable();

        inventoryToggleUI.ShowBackpack();

        // 👉 Step 5: set the first slot selected so Submit works
        //var eventSystem = UnityEngine.EventSystems.EventSystem.current;
        //if (eventSystem != null && PlayerInventory.Instance.UISlots.Count > 0)
        //{
            //eventSystem.SetSelectedGameObject(PlayerInventory.Instance.UISlots[0].gameObject);
        //}
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);

        // Switch back to gameplay controls
        controls.UI.Disable();
        controls.Player.Enable();

        // Clear any queued inputs so A doesn’t trigger Jump immediately
        //controls.Player.Jump.Reset();
        //controls.Player.Mine.Reset();
        //controls.Player.Move.Reset();

        // Flush any carried-over input
        InputSystem.ResetDevice(Gamepad.current);
    }
    public bool IsPaused => isPaused;
}
