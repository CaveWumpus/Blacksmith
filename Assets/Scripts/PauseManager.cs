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
    private bool navHeld = false;

    public enum PauseMenuMode { PanelSelect, Inventory }
    // Add a new enum for panel cycling
    public enum PanelType
    {
        Backpack,
        Relic,
        EvacuationPrompt
    }
    private PanelType currentPanel = PanelType.Backpack;

    public enum InventoryMode { Navigation, ContextMenu, MovePending }

    public PauseMenuMode currentMode = PauseMenuMode.PanelSelect;
    public InventoryMode inventoryMode = InventoryMode.Navigation;

    [Header("Evacuation UI")]
    public GameObject evacuationPromptPanel;
    public TMPro.TextMeshProUGUI evacuationHeaderText;
    public TMPro.TextMeshProUGUI evacuationPromptText;

    public GameObject evacuationYesButton;
    public GameObject evacuationNoButton;
    public GameObject evacuationConfirmYesButton;
    public GameObject evacuationConfirmNoButton;
    public GameObject evacuationInitialYesButton;
    public GameObject evacuationInitialNoButton;

    private enum EvacuationStep { InitialPrompt, ConfirmPrompt }
    private EvacuationStep currentEvacuationStep = EvacuationStep.InitialPrompt;


    [Header("Evacuation Settings")]
    [Range(0,100)] public int evacuationLossPercent = 10;

    //[Header("Inventory Panels")]
    //public GameObject playerInventoryPanel;  // assign BackpackPanel in Inspector
    //public GameObject relicPanel;            // assign RelicPanel in Inspector


    
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

            if (!navHeld && Mathf.Abs(nav.x) > 0.7f)
            {
                if (nav.x < 0) CycleLeft();
                else CycleRight();
                navHeld = true; // lock until released
            }
            else if (Mathf.Abs(nav.x) < 0.2f)
            {
                navHeld = false; // reset when stick returns to center
            }
        }
    }

    private void CycleLeft()
    {
        if (currentPanel == PanelType.Backpack)
            currentPanel = PanelType.Relic;
        else if (currentPanel == PanelType.Relic)
            currentPanel = PanelType.EvacuationPrompt;
        else
            currentPanel = PanelType.Backpack;

        ShowCurrentPanel();
    }

    private void CycleRight()
    {
        if (currentPanel == PanelType.Backpack)
            currentPanel = PanelType.EvacuationPrompt;
        else if (currentPanel == PanelType.EvacuationPrompt)
            currentPanel = PanelType.Relic;
        else
            currentPanel = PanelType.Backpack;

        ShowCurrentPanel();
    }

    private void ShowCurrentPanel()
    {
        // Backpack panel
        if (currentPanel == PanelType.Backpack)
        {
            inventoryToggleUI.ShowBackpack();
            evacuationPromptPanel.SetActive(false);
        }
        // Relic panel
        else if (currentPanel == PanelType.Relic)
        {
            inventoryToggleUI.ShowRelics();
            evacuationPromptPanel.SetActive(false);
        }
        // Evacuation prompt panel
        else if (currentPanel == PanelType.EvacuationPrompt)
        {
            inventoryToggleUI.HideAllPanels();   // turn off Backpack + Relic
            evacuationPromptPanel.SetActive(true);
            BuildEvacuationText();
            EventSystem.current.SetSelectedGameObject(evacuationInitialYesButton);
        }
    }

    public void ShowEvacuationPrompt()
    {
        evacuationPromptPanel.SetActive(true);
        currentEvacuationStep = EvacuationStep.InitialPrompt;

        evacuationHeaderText.text = "Would you like to evacuate to Shop?";
        evacuationPromptText.text = ""; // 🔹 keep empty until Confirm step

        evacuationYesButton.SetActive(true);
        evacuationNoButton.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SetFocusNextFrame(evacuationYesButton));
    }



    public void OnEvacuationYes()
    {
        if (currentEvacuationStep == EvacuationStep.InitialPrompt)
        {
            // 🔹 Move to Confirm step
            currentEvacuationStep = EvacuationStep.ConfirmPrompt;
            evacuationHeaderText.text = "You will lose the following resources:";

            // Build and show breakdown only now
            evacuationPromptText.text = BuildEvacuationBreakdown();

            EventSystem.current.SetSelectedGameObject(null);
            StartCoroutine(SetFocusNextFrame(evacuationYesButton));
        }
        else if (currentEvacuationStep == EvacuationStep.ConfirmPrompt)
        {
            // 🔹 Apply losses and load Shop
            foreach (var stack in PlayerInventory.Instance.oreStacks)
            {
                int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
                stack.count -= lost;
            }
            foreach (var stack in PlayerInventory.Instance.gemStacks)
            {
                int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
                stack.count -= lost;
            }

            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("ShopScene");
        }
    }



    public void OnEvacuationNo()
    {
        if (currentEvacuationStep == EvacuationStep.InitialPrompt)
        {
            // 🔹 Cancel evacuation completely
            CancelEvacuation();
        }
        else if (currentEvacuationStep == EvacuationStep.ConfirmPrompt)
        {
            // 🔹 Reset back to initial prompt instead of closing
            currentEvacuationStep = EvacuationStep.InitialPrompt;
            evacuationHeaderText.text = "Would you like to evacuate to Shop?";
            evacuationPromptText.text = "";

            EventSystem.current.SetSelectedGameObject(null);
            StartCoroutine(SetFocusNextFrame(evacuationYesButton));
        }
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        // Always check what is currently selected
        var selectedGO = EventSystem.current.currentSelectedGameObject;

        // If a Unity UI Button is selected, let its onClick handle it
        if (selectedGO != null && selectedGO.GetComponent<UnityEngine.UI.Button>() != null)
        {
            selectedGO.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            return;
        }

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
            Debug.Log($"Inventory Navigation: Selected={selectedGO?.name}");
            if (selectedGO == null) return;

            var playerSlot = selectedGO.GetComponent<InventorySlotUI>() ?? selectedGO.GetComponentInParent<InventorySlotUI>();
            if (playerSlot != null)
            {
                Debug.Log("Opening InventoryContextMenu for player slot.");
                InventoryContextMenu.Instance.Open(playerSlot);
                return;
            }

            var relicSlot = selectedGO.GetComponent<RelicSlotUI>() ?? selectedGO.GetComponentInParent<RelicSlotUI>();
            if (relicSlot != null)
            {
                Debug.Log("Opening RelicContextMenu for relic slot.");
                RelicContextMenu.Instance.Open(relicSlot);
                return;
            }

            Debug.LogError("OnSubmit: Selected object has no slot UI component!");
            return;
        }

        // 3. Handle MovePending
        if (currentMode == PauseMenuMode.Inventory && inventoryMode == InventoryMode.MovePending)
        {
            Debug.Log($"[MovePending] Submit pressed. SourceSlot={moveSourceSlot?.name ?? "null"}, SourceRelic={moveSourceRelicSlot?.name ?? "null"}, Target={selectedGO?.name ?? "null"}");

            // Player inventory move
            if (moveSourceSlot != null)
            {
                var targetSlot = selectedGO?.GetComponent<InventorySlotUI>() ?? selectedGO?.GetComponentInParent<InventorySlotUI>();
                if (targetSlot != null)
                {
                    PlayerInventory.Instance.MoveItem(moveSourceSlot, targetSlot);
                }
                moveSourceSlot = null;
                inventoryMode = InventoryMode.Navigation;
                Debug.Log("[MovePending] Player move complete. Returning to Navigation mode.");
                return;
            }

            // Relic inventory move
            if (moveSourceRelicSlot != null)
            {
                var targetRelic = selectedGO?.GetComponent<RelicSlotUI>() ?? selectedGO?.GetComponentInParent<RelicSlotUI>();
                if (targetRelic != null)
                {
                    RelicInventory.Instance.MoveItem(moveSourceRelicSlot, targetRelic);
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

    private void BuildEvacuationText()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (var stack in PlayerInventory.Instance.oreStacks)
        {
            int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
            sb.AppendLine($"{stack.itemName}: Lose {lost} (from {stack.count})");
        }

        foreach (var stack in PlayerInventory.Instance.gemStacks)
        {
            int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
            sb.AppendLine($"{stack.itemName}: Lose {lost} (from {stack.count})");
        }

        //evacuationPromptText.text = sb.ToString();
    }


    public void ConfirmEvacuation()
    {
        foreach (var stack in PlayerInventory.Instance.oreStacks)
        {
            int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
            stack.count -= lost;
        }

        foreach (var stack in PlayerInventory.Instance.gemStacks)
        {
            int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
            stack.count -= lost;
        }

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("ShopScene");
    }

    public void CancelEvacuation()
    {
        evacuationPromptPanel.SetActive(false);
        currentEvacuationStep = EvacuationStep.InitialPrompt;
        evacuationPromptText.text = "";
        currentPanel = PanelType.Backpack;
        ShowCurrentPanel();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        evacuationPromptPanel.SetActive(false);
        currentEvacuationStep = EvacuationStep.InitialPrompt;
        evacuationPromptText.text = "";

        controls.UI.Disable();
        controls.Player.Enable();

        InputSystem.ResetDevice(Gamepad.current);
    }

    private System.Collections.IEnumerator SetFocusNextFrame(GameObject button)
    {
        yield return null; // wait one frame
        EventSystem.current.SetSelectedGameObject(button);
    }

    private string BuildEvacuationBreakdown()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (var stack in PlayerInventory.Instance.oreStacks)
        {
            int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
            sb.AppendLine($"{stack.itemName}: Lose {lost} (from {stack.count})");
        }

        foreach (var stack in PlayerInventory.Instance.gemStacks)
        {
            int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
            sb.AppendLine($"{stack.itemName}: Lose {lost} (from {stack.count})");
        }

        return sb.ToString();
    }

    public void ForceEvacuationOnTimer()
    {
        Time.timeScale = 0f;

        evacuationPromptPanel.SetActive(true);
        currentEvacuationStep = EvacuationStep.ConfirmPrompt;

        evacuationHeaderText.text = "Time expired! You must evacuate.";
        evacuationPromptText.text = BuildEvacuationBreakdown();

        pauseMenuPanel.SetActive(false);

        // Disable pause input so Start button does nothing
        controls.Global.Pause.Disable();

        // 🔹 Hide the No button so player cannot decline
        evacuationNoButton.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SetFocusNextFrame(evacuationYesButton));
    }

    public bool IsPaused => isPaused;
}
