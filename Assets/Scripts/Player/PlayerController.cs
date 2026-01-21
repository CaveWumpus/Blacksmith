using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Subsystems")]
    public PlayerInputHandler input;
    public PlayerMovementController movement;
    public PlayerMiningController mining;
    public PlayerAnimationController animationController;
    [Header("Grapple State")]
    public bool isGripping = false;

    private float defaultGravityScale;
    private Rigidbody2D rb;


    void Reset()
    {
        input = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovementController>();
        mining = GetComponent<PlayerMiningController>();
        animationController = GetComponent<PlayerAnimationController>();
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }
    public void SetGrappleState(bool gripping)
    {
        isGripping = gripping;

        if (gripping)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero; // stop drifting
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    public void SetVelocity(Vector2 vel)
    {
        rb.linearVelocity = vel;
    }


}
