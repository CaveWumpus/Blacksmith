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
        HandleMovement();
        HandleJump();
        HandleFacingDirection();
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
        targetSpeed = input.moveInput.x * moveSpeed;

        // Smooth acceleration/deceleration
        if (Mathf.Abs(targetSpeed) > 0.1f)
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
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

        // Optional: variable jump height
        if (!input.jumpHeld && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.6f);
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
