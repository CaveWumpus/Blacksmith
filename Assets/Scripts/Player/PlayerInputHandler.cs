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
    public bool gripHeld;
    public bool mineStarted { get; private set; }
    public bool mineHeld { get; private set; }
    public bool mineReleased { get; private set; }
    public bool toolNextPressed;
    public bool toolPrevPressed;
    private InputAction toolNextAction;
    private InputAction toolPrevAction;







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
        toolNextAction = playerInput.actions["ToolNext"];
        toolPrevAction = playerInput.actions["ToolPrev"];


        controls.Player.LanternIncrease.performed += _ => lanternIncreasePressed = true;
        controls.Player.LanternDecrease.performed += _ => lanternDecreasePressed = true;
        controls.Player.LanternBoost.performed += _ => lanternBoostPressed = true;
        controls.Player.Grapple.performed += _ => gripHeld = true;
        controls.Player.Grapple.canceled += _ => gripHeld = false;


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
        toolNextAction.Enable();
        toolPrevAction.Enable();

        toolNextAction.performed += ctx => toolNextPressed = true;
        toolPrevAction.performed += ctx => toolPrevPressed = true;

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
        toolNextAction.Disable();
        toolPrevAction.Disable();
    }

    void Update()
    {
        // Continuous inputs are read every frame
        moveInput = moveAction.ReadValue<Vector2>();
        jumpHeld  = jumpAction.IsPressed();

        // One-frame buttons (jumpPressed, minePressed, pausePressed) are latched here
        // and should be consumed by other systems, then reset.
    }

    void LateUpdate()
    {
        mineStarted = false;
        mineReleased = false;
        toolNextPressed = false;
        toolPrevPressed = false;
    }

    private void FixedUpdate()
    {
        jumpPressed = false;
        minePressed = false;
        pausePressed = false;
        
        lanternIncreasePressed = false;
        lanternDecreasePressed = false;
        lanternBoostPressed = false;
        
    }
    public void OnMine(InputAction.CallbackContext context)
    {
        //Debug.Log("OnMine: " + context.phase);

        if (context.started)
            mineStarted = true;

        if (context.performed)
            mineHeld = true;

        if (context.canceled)
        {
            mineHeld = false;
            mineReleased = true;
        }
        Debug.Log("OnMine: " + context.phase);

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
    public void OnToolNext(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            toolNextPressed = true;
    }


    public void OnToolPrev(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            toolPrevPressed = true;
    }

}
