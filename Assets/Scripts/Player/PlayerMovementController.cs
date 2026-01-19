using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputHandler input;     // Drag your PlayerInputHandler here
    public Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float acceleration = 12f;
    public float deceleration = 14f;

    [Header("Advanced Movement Tuning")]
    public float maxAcceleration = 80f;     // how fast you reach full speed
    public float maxDeceleration = 90f;     // how fast you stop
    public float apexBonus = 1.2f;          // extra control at top of jump
    public float gravityScale = 4f;         // stronger gravity
    public float fallGravityScale = 6f;     // even stronger when falling


    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    // Internal state
    public bool isGrounded;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float targetSpeed;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleGroundCheck();
        HandleJumpTimers();
    }

    void FixedUpdate()
    {
        ApplyCustomGravity();
        HandleMovement();
        HandleJump();
        HandleFacingDirection();
    }

    void ApplyCustomGravity()
    {
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = fallGravityScale;   // fast fall
        else
            rb.gravityScale = gravityScale;       // normal jump
    }


    // -----------------------------
    // Ground Check
    // -----------------------------
    void HandleGroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;
    }

    // -----------------------------
    // Jump Buffer + Coyote Time
    // -----------------------------
    void HandleJumpTimers()
    {
        if (input.jumpPressed)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    // -----------------------------
    // Horizontal Movement
    // -----------------------------
    void HandleMovement()
    {
        float inputX = input.moveInput.x;

        // Target horizontal speed
        float targetSpeed = inputX * moveSpeed;

        // Determine acceleration or deceleration
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f)
            ? maxAcceleration
            : maxDeceleration;

        // Apex bonus (more control at top of jump)
        if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            accelRate *= apexBonus;

        // Move toward target speed
        float newSpeed = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            accelRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
    }


    // -----------------------------
    // Jump Logic
    // -----------------------------
    void HandleJump()
    {
        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            coyoteCounter = 0;
        }

        // Variable jump height
        if (!input.jumpHeld && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.55f);
        }
    }


    // -----------------------------
    // Facing Direction
    // -----------------------------
    void HandleFacingDirection()
    {
        if (input.moveInput.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (input.moveInput.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // -----------------------------
    // Gizmos (optional)
    // -----------------------------
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
