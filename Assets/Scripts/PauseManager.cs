using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public GameObject pauseMenuPanel;
    public InventoryToggleUI inventoryToggleUI;
    public PlayerController playerController;

    private PlayerControls controls;
    private bool isPaused = false;

    public enum PauseMenuMode { PanelSelect, Inventory }
    public enum InventoryMode { Navigation, ContextMenu, MovePending }

    public PauseMenuMode currentMode = PauseMenuMode.PanelSelect;
    public InventoryMode inventoryMode = InventoryMode.Navigation;

    // Source slots for move
    public InventorySlotUI moveSourceSlot;
    public RelicSlotUI moveSourceRelicSlot;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controls = new PlayerControls();

        if (playerController != null)
            playerController.controls = controls;
    }


    void OnEnable()
    {
        controls.Global.Pause.performed += OnPause;
        controls.Global.Enable();

        controls.UI.Navigate.performed += OnNavigate;
        controls.UI.Submit.performed += OnSubmit;
        controls.UI.Cancel.performed += OnCancel;
        controls.UI.Disable();

        controls.Player.Enable();
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Global.Pause.performed -= OnPause;
            controls.Global.Disable();

            controls.UI.Navigate.performed -= OnNavigate;
            controls.UI.Submit.performed -= OnSubmit;
            controls.UI.Cancel.performed -= OnCancel;
            controls.UI.Disable();

            controls.Player.Disable();
        }
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (currentMode == PauseMenuMode.PanelSelect)
        {
            Vector2 nav = ctx.ReadValue<Vector2>();

            // Only care about left/right input
            if (Mathf.Abs(nav.x) > 0.7f)
            {
                inventoryToggleUI.ToggleLeftRight(nav);
            }
        }
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        Debug.Log($"OnSubmit called. Mode={currentMode}, InvMode={inventoryMode}");

        // 1. Enter inventory from PanelSelect
        if (currentMode == PauseMenuMode.PanelSelect)
        {
            Debug.Log("Switching from PanelSelect to Inventory Navigation.");
            currentMode = PauseMenuMode.Inventory;
            inventoryMode = InventoryMode.Navigation;

            // Focus first slot of whichever panel is active
            if (inventoryToggleUI.IsBackpackActive && PlayerInventory.Instance.UISlots.Count > 0)
            {
                var firstSlot = PlayerInventory.Instance.UISlots[0].gameObject;
                EventSystem.current.SetSelectedGameObject(firstSlot);
                EventSystem.current.firstSelectedGameObject = firstSlot;
            }
            else if (!inventoryToggleUI.IsBackpackActive && RelicInventory.Instance.UISlots.Count > 0)
            {
                var firstRelic = RelicInventory.Instance.UISlots[0].gameObject;
                EventSystem.current.SetSelectedGameObject(firstRelic);
                EventSystem.current.firstSelectedGameObject = firstRelic;
            }
            return;
        }

        // 2. In inventory navigation: open context menu for selected slot
        if (currentMode == PauseMenuMode.Inventory && inventoryMode == InventoryMode.Navigation)
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            Debug.Log($"Inventory Navigation: Selected={selected?.name}");
            if (selected == null) return;

            // ✅ Use GetComponentInParent to catch child selections
            var playerSlot = selected.GetComponent<InventorySlotUI>() ?? selected.GetComponentInParent<InventorySlotUI>();
            if (playerSlot != null)
            {
                Debug.Log("Opening InventoryContextMenu for player slot.");
                InventoryContextMenu.Instance.Open(playerSlot);
                return;
            }

            var relicSlot = selected.GetComponent<RelicSlotUI>() ?? selected.GetComponentInParent<RelicSlotUI>();
            if (relicSlot != null)
            {
                Debug.Log("Opening RelicContextMenu for relic slot.");
                RelicContextMenu.Instance.Open(relicSlot);
                return;
            }

            // ✅ If neither slot type found, log error
            Debug.LogError("OnSubmit: Selected object has no slot UI component!");
            return;
        }

        // 3. Handle MovePending
        if (currentMode == PauseMenuMode.Inventory && inventoryMode == InventoryMode.MovePending)
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            Debug.Log($"[MovePending] Submit pressed. SourceSlot={moveSourceSlot?.name ?? "null"}, SourceRelic={moveSourceRelicSlot?.name ?? "null"}, Target={selected?.name ?? "null"}");

            // Player inventory move
            if (moveSourceSlot != null)
            {
                var targetSlot = selected?.GetComponent<InventorySlotUI>() ?? selected?.GetComponentInParent<InventorySlotUI>();
                Debug.Log($"[MovePending] Player move: TargetSlot={targetSlot?.name ?? "null"}");

                if (targetSlot != null)
                {
                    if (targetSlot.IsEmpty())
                    {
                        Debug.Log($"[MovePending] Moving {moveSourceSlot.itemName} x{moveSourceSlot.count} into empty slot {targetSlot.name}");
                        PlayerInventory.Instance.MoveItem(moveSourceSlot, targetSlot);
                    }
                    else
                    {
                        Debug.Log($"[MovePending] Swapping {moveSourceSlot.itemName} with {targetSlot.itemName}");
                        PlayerInventory.Instance.MoveItem(moveSourceSlot, targetSlot);
                    }
                }
                moveSourceSlot = null;
                inventoryMode = InventoryMode.Navigation;
                Debug.Log("[MovePending] Player move complete. Returning to Navigation mode.");
                return;
            }

            // Relic inventory move
            if (moveSourceRelicSlot != null)
            {
                var targetRelic = selected?.GetComponent<RelicSlotUI>() ?? selected?.GetComponentInParent<RelicSlotUI>();
                Debug.Log($"[MovePending] Relic move: TargetRelic={targetRelic?.name ?? "null"}");

                if (targetRelic != null)
                {
                    if (targetRelic.IsEmpty())
                    {
                        Debug.Log($"[MovePending] Moving {moveSourceRelicSlot.relicName} into empty relic slot {targetRelic.name}");
                        RelicInventory.Instance.MoveItem(moveSourceRelicSlot, targetRelic);
                    }
                    else
                    {
                        Debug.Log($"[MovePending] Swapping {moveSourceRelicSlot.relicName} with {targetRelic.relicName}");
                        RelicInventory.Instance.MoveItem(moveSourceRelicSlot, targetRelic);
                    }
                }
                moveSourceRelicSlot = null;
                inventoryMode = InventoryMode.Navigation;
                Debug.Log("[MovePending] Relic move complete. Returning to Navigation mode.");
                return;
            }
        }


    }



    private void OnCancel(InputAction.CallbackContext ctx)
    {
        // 1. If inside inventory
        if (currentMode == PauseMenuMode.Inventory)
        {
            // a) If a context menu is open → close it, return to slot navigation
            if (inventoryMode == InventoryMode.ContextMenu)
            {
                if (InventoryContextMenu.Instance != null && InventoryContextMenu.Instance.gameObject.activeSelf)
                    InventoryContextMenu.Instance.Close();
                if (RelicContextMenu.Instance != null && RelicContextMenu.Instance.gameObject.activeSelf)
                    RelicContextMenu.Instance.Close();

                inventoryMode = InventoryMode.Navigation;

                // Reset focus back to the currently selected slot
                var selected = EventSystem.current.currentSelectedGameObject;
                if (selected == null)
                {
                    if (inventoryToggleUI.IsBackpackActive && PlayerInventory.Instance.UISlots.Count > 0)
                        EventSystem.current.SetSelectedGameObject(PlayerInventory.Instance.UISlots[0].gameObject);
                    else if (!inventoryToggleUI.IsBackpackActive && RelicInventory.Instance.UISlots.Count > 0)
                        EventSystem.current.SetSelectedGameObject(RelicInventory.Instance.UISlots[0].gameObject);
                }
            }
            // b) If move is pending → cancel move, return to slot navigation
            else if (inventoryMode == InventoryMode.MovePending)
            {
                moveSourceSlot = null;
                moveSourceRelicSlot = null;
                inventoryMode = InventoryMode.Navigation;
            }
            // c) Otherwise → exit inventory back to PanelSelect
            else
            {
                currentMode = PauseMenuMode.PanelSelect;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        // 2. If in PanelSelect → pressing B resumes the game
        else if (currentMode == PauseMenuMode.PanelSelect)
        {
            ResumeGame();
        }
    }


    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);

        controls.Player.Disable();
        controls.UI.Enable();

        currentMode = PauseMenuMode.PanelSelect;
        inventoryMode = InventoryMode.Navigation;

        // Default to backpack visible
        inventoryToggleUI.ShowBackpack();
    }



    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);

        controls.UI.Disable();
        controls.Player.Enable();

        InputSystem.ResetDevice(Gamepad.current);
    }

    public bool IsPaused => isPaused;
}
