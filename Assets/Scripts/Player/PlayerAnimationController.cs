using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementController movement;
    public PlayerMiningController mining;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovementAnimations();
        HandleMiningAnimations();
    }

    void HandleMovementAnimations()
    {
        float horizontalSpeed = Mathf.Abs(movement.rb.linearVelocity.x);
        float verticalSpeed = movement.rb.linearVelocity.y;

        anim.SetFloat("Speed", horizontalSpeed);
        anim.SetFloat("VerticalSpeed", verticalSpeed);
        anim.SetBool("IsGrounded", movement.isGrounded);
    }

    void HandleMiningAnimations()
    {
        // Trigger mining animation when mining cooldown resets
        if (mining.input.minePressed)
        {
            Vector2 dir = mining.input.moveInput;

            if (dir.y > 0.5f) anim.SetTrigger("MineUp");
            else if (dir.y < -0.5f) anim.SetTrigger("MineDown");
            else anim.SetTrigger("MineForward");
        }
    }
}
