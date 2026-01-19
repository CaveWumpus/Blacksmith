using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("Current Input State")]
    public Vector2 moveInput;      // X = horizontal, Y = vertical (for ladders, aiming, etc.)
    public bool jumpPressed;
    public bool jumpHeld;
    public bool minePressed;
    public bool pausePressed;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction mineAction;
    private InputAction pauseAction;
    public bool lanternIncreasePressed;
    public bool lanternDecreasePressed;
    public bool lanternBoostPressed;
    public static PlayerInputHandler Instance { get; private set; }
    private PlayerControls controls;




    void Awake()
    {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();
        controls = new PlayerControls();
        controls.Enable();

        // Assumes you have an "Gameplay" or "Player" action map with these actions
        moveAction  = playerInput.actions["Move"];
        jumpAction  = playerInput.actions["Jump"];
        mineAction  = playerInput.actions["Mine"];
        pauseAction = playerInput.actions["Pause"];

        controls.Player.LanternIncrease.performed += _ => lanternIncreasePressed = true;
        controls.Player.LanternDecrease.performed += _ => lanternDecreasePressed = true;
        controls.Player.LanternBoost.performed += _ => lanternBoostPressed = true;

    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        mineAction.Enable();
        pauseAction.Enable();

        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled  += OnJumpCanceled;

        mineAction.performed += OnMinePerformed;

        pauseAction.performed += OnPausePerformed;
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        mineAction.Disable();
        pauseAction.Disable();

        jumpAction.performed -= OnJumpPerformed;
        jumpAction.canceled  -= OnJumpCanceled;

        mineAction.performed -= OnMinePerformed;

        pauseAction.performed -= OnPausePerformed;
    }

    void Update()
    {
        // Continuous inputs are read every frame
        moveInput = moveAction.ReadValue<Vector2>();
        jumpHeld  = jumpAction.IsPressed();

        // One-frame buttons (jumpPressed, minePressed, pausePressed) are latched here
        // and should be consumed by other systems, then reset.
    }

    //private void LateUpdate()
    //{
        // Reset one-frame flags after other systems have had a chance to read them
    //    jumpPressed  = false;
    //    minePressed  = false;
    //    pausePressed = false;
    //}
    private void FixedUpdate()
    {
        jumpPressed = false;
        minePressed = false;
        pausePressed = false;
        
        lanternIncreasePressed = false;
        lanternDecreasePressed = false;
        lanternBoostPressed = false;

    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        jumpPressed = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        // jumpHeld is handled in Update via IsPressed()
    }

    private void OnMinePerformed(InputAction.CallbackContext ctx)
    {
        minePressed = true;
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        pausePressed = true;
    }
}
