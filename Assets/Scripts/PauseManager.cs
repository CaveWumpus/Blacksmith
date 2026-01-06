using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;   // drag your PauseMenuPanel here
    public InventoryToggleUI inventoryToggleUI; // drag your toggle script here

    private PlayerControls controls;
    private bool isPaused = false;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.UI.Pause.performed += OnPause;
        controls.UI.Enable();
    }

    void OnDisable()
    {
        controls.UI.Pause.performed -= OnPause;
        controls.UI.Disable();
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
        isPaused = true;
        Time.timeScale = 0f; // freeze gameplay
        pauseMenuPanel.SetActive(true);
        inventoryToggleUI.ShowBackpack(); // default view
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // resume gameplay
        pauseMenuPanel.SetActive(false);
    }
}
