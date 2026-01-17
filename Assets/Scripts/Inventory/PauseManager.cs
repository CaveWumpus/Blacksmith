using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;   

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public InventoryToggleUI inventoryToggleUI;

    [Header("Inventory")]
    public UnifiedInventoryController inventoryController;

    [Header("Evacuation")]
    public EvacuationController evacuationController;

    private PlayerControls controls;
    private bool isPaused = false;

    public enum PauseMenuMode
    {
        PanelSelect,
        Inventory,
        Evacuation
    }

    public PauseMenuMode currentMode = PauseMenuMode.PanelSelect;

    public bool IsPaused => isPaused;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Global.Pause.performed += OnPause;
        controls.Global.Enable();

        controls.UI.Navigate.performed += OnNavigate;
        controls.UI.Submit.performed += OnSubmit;
        controls.UI.Cancel.performed += OnCancel;
        controls.UI.Enable();
    }

    void OnDisable()
    {
        controls.Global.Pause.performed -= OnPause;

        controls.UI.Navigate.performed -= OnNavigate;
        controls.UI.Submit.performed -= OnSubmit;
        controls.UI.Cancel.performed -= OnCancel;
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
        inventoryToggleUI.ShowInventory();

        currentMode = PauseMenuMode.PanelSelect;

        controls.Player.Disable();
        controls.UI.Enable();

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        pauseMenuPanel.SetActive(false);
        inventoryToggleUI.HideAll();

        controls.UI.Disable();
        controls.Player.Enable();

        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 nav = ctx.ReadValue<Vector2>();

        // Only handle left/right page switching while in PanelSelect
        if (currentMode != PauseMenuMode.PanelSelect)
            return;

        if (nav.x > 0.7f)
        {
            inventoryToggleUI.ShowEvacuation();
            evacuationController.ShowInitialPrompt();
            //currentMode = PauseMenuMode.Evacuation;

            // Select the first button on the evacuation page (usually Yes)
            var firstEvacButton = inventoryToggleUI.evacuationPanel.GetComponentInChildren<UnityEngine.UI.Button>();
            if (firstEvacButton != null)
                EventSystem.current.SetSelectedGameObject(firstEvacButton.gameObject);

            return;
        }

        else if (nav.x < -0.7f)
        {
            inventoryToggleUI.ShowInventory();
            currentMode = PauseMenuMode.PanelSelect;

            EventSystem.current.SetSelectedGameObject(null);
        }
    }


    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        //Debug.Log($"First inventory button: {first}");

        var selected = EventSystem.current.currentSelectedGameObject;

    

        if (currentMode == PauseMenuMode.Evacuation)
        {
            evacuationController.OnSubmitFromPause();
            return;
        }

        // Evacuation buttons
        //if (currentMode == PauseMenuMode.Evacuation &&
            //selected != null &&
            //selected.GetComponent<UnityEngine.UI.Button>() != null)
        //{
            //selected.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            //return;
        //}

        // Enter Inventory mode from PanelSelect
        if (currentMode == PauseMenuMode.PanelSelect)
        {
            // If the evacuation page is currently visible, enter evacuation mode
            if (inventoryToggleUI.evacuationPanel.activeSelf)
            {
                currentMode = PauseMenuMode.Evacuation;
                evacuationController.HandleSubmit();
                return;
            }

            // Otherwise enter inventory mode
            currentMode = PauseMenuMode.Inventory;
            var first = inventoryToggleUI.inventoryPage.GetComponentInChildren<Button>();
            if (first != null)
                EventSystem.current.SetSelectedGameObject(first.gameObject);
            return;
        }

    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        // Inventory mode → exit to PanelSelect
        if (currentMode == PauseMenuMode.Inventory)
        {
            inventoryController.OnCancel();
            currentMode = PauseMenuMode.PanelSelect;
            //EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        // PanelSelect → resume game
        if (currentMode == PauseMenuMode.PanelSelect)
        {
            ResumeGame();
            return;
        }
        if (currentMode == PauseMenuMode.Evacuation)
        {
            evacuationController.HandleCancel();
            return;
        }


        // Evacuation mode → handled internally
    }

    public void ForceEvacuationOnTimer()
    {
        isPaused = true;
        Time.timeScale = 0f;

        pauseMenuPanel.SetActive(true);
        inventoryToggleUI.ShowEvacuation();

        currentMode = PauseMenuMode.Evacuation;

        evacuationController.ForceEvacuation();
    }
}
