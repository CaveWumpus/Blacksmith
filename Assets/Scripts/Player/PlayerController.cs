using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Subsystems")]
    public PlayerInputHandler input;
    public PlayerMovementController movement;
    public PlayerMiningController mining;
    public PlayerAnimationController animationController;

    void Reset()
    {
        input = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovementController>();
        mining = GetComponent<PlayerMiningController>();
        animationController = GetComponent<PlayerAnimationController>();
    }
}
