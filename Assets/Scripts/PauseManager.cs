using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;   // drag your PauseMenuPanel here
    public InventoryToggleUI inventoryToggleUI; // drag your toggle script here
    public PlayerController playerController; // drag your PlayerController here

    private PlayerControls controls;
    private bool isPaused = false;

    void Awake()
    {
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
        controls.UI.Disable();

        // Gameplay enabled by default
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Global.Pause.performed -= OnPause;
        controls.Global.Disable();

        controls.UI.Navigate.performed -= OnNavigate;
        controls.UI.Disable();

        controls.Player.Disable();
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 nav = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(nav.x) > 0.7f)
        {
            inventoryToggleUI.ToggleLeftRight(nav);
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
        var eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem != null && PlayerInventory.Instance.UISlots.Count > 0)
        {
            eventSystem.SetSelectedGameObject(PlayerInventory.Instance.UISlots[0].gameObject);
        }
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
