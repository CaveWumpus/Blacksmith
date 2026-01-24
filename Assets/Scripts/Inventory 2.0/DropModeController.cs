using UnityEngine;
using UnityEngine.InputSystem;

public class DropModeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MiningInventoryController inventory;
    [SerializeField] private MiningInventoryOverlayUI overlayUI;

    [Header("Settings")]
    [SerializeField] private float slowTimeScale = 0.25f;
    [SerializeField] private float normalTimeScale = 1f;

    private bool isInDropMode = false;
    private int selectedIndex = 0;
    private float navCooldown = 0.05f;
    private float navTimer = 0f;


    // ---------------------------------------------------------
    // Input (Unity Input System)
    // ---------------------------------------------------------
    private PlayerControls input;

    private void Awake()
    {
        input = new PlayerControls();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.DropMode.performed += OnDropModePressed; // LB+RB
        input.Player.Move.performed += OnNavigate;            // left stick
        input.Player.Jump.performed += OnConfirm;             // A button
        input.Player.Cancel.performed += OnCancel;            // B button (new action)
    }


    private void OnDisable()
    {
        input.Player.DropMode.performed -= OnDropModePressed;
        input.Player.Move.performed -= OnNavigate;
        input.Player.Jump.performed -= OnConfirm;
        input.Player.Cancel.performed -= OnCancel;

        input.Disable();
    }


    // ---------------------------------------------------------
    // Enter / Exit Drop Mode
    // ---------------------------------------------------------
    private void OnDropModePressed(InputAction.CallbackContext ctx)
    {
        if (!isInDropMode)
            EnterDropMode();
        else
            ExitDropMode();
    }

    private void EnterDropMode()
    {
        isInDropMode = true;
        Time.timeScale = slowTimeScale;

        overlayUI.SetDropMode();
        overlayUI.BuildList(inventory.GetItemsSorted());
        HighlightSmartDropCandidates();

        selectedIndex = 0;
        overlayUI.HighlightSlot(selectedIndex);
    }



    private void ExitDropMode()
    {
        isInDropMode = false;
        Time.timeScale = normalTimeScale;

        overlayUI.SetHUDMode();
    }


    // ---------------------------------------------------------
    // Navigation
    // ---------------------------------------------------------
    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (!isInDropMode) return;

        navTimer -= Time.unscaledDeltaTime;
        if (navTimer > 0f) return;

        Vector2 inputDir = ctx.ReadValue<Vector2>();

        if (inputDir.y > 0.5f)
        {
            MoveSelection(-1);
            navTimer = navCooldown;
        }
        else if (inputDir.y < -0.5f)
        {
            MoveSelection(1);
            navTimer = navCooldown;
        }
    }


    private void MoveSelection(int delta)
    {
        int newIndex = selectedIndex + delta;

        if (newIndex < 0 || newIndex >= inventory.Count)
            return;

        selectedIndex = newIndex;
        overlayUI.HighlightSlot(selectedIndex);
    }


    // ---------------------------------------------------------
    // Confirm Drop
    // ---------------------------------------------------------
    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (!isInDropMode) return;

        InventoryItemData dropped = inventory.DropItem(selectedIndex);
        overlayUI.PlayDropAnimation(selectedIndex);

        // Optional: spawn physical drop
        // Optional: destroy item immediately

        ExitDropMode();
    }

    // ---------------------------------------------------------
    // Cancel
    // ---------------------------------------------------------
    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (isInDropMode)
            ExitDropMode();
    }

    // ---------------------------------------------------------
    // Smart Drop Highlighting
    // ---------------------------------------------------------
    private void HighlightSmartDropCandidates()
    {
        var candidates = inventory.GetSmartDropCandidates();
        overlayUI.HighlightSmartDropCandidates(candidates);
    }
}
