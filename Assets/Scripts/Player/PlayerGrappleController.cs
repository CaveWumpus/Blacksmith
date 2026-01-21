using UnityEngine;

public class PlayerGrappleController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public PlayerInputHandler input;

    [Header("Grapple Settings")]
    public bool allowWallGrab = true;
    public bool allowCeilingGrab = true;
    public bool hazardsBreakGrip = true;

    [Header("Detection Settings")]
    public float detectDistance = 0.2f;
    public LayerMask tileLayer;

    [Header("Movement While Gripping")]
    public float shimmySpeed = 3f;
    public float climbSpeed = 3f;

    private bool isGripping = false;
    private bool touchingWallLeft = false;
    private bool touchingWallRight = false;
    private bool touchingCeiling = false;

    void Awake()
    {
        // Auto‑assign references if not set in Inspector
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (input == null)
            input = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        DetectSurfaces();

        if (!isGripping)
        {
            TryEnterGrip();
        }
        else
        {
            MaintainGrip();
        }
    }

    // -----------------------------
    // SURFACE DETECTION
    // -----------------------------
    private void DetectSurfaces()
    {
        touchingWallLeft = Physics2D.Raycast(transform.position, Vector2.left, detectDistance, tileLayer);
        touchingWallRight = Physics2D.Raycast(transform.position, Vector2.right, detectDistance, tileLayer);
        touchingCeiling = Physics2D.Raycast(transform.position, Vector2.up, detectDistance, tileLayer);
    }

    // -----------------------------
    // ENTER GRIP
    // -----------------------------
    private void TryEnterGrip()
    {
        if (!input.gripHeld)
            return;

        bool canGrabWall = allowWallGrab && (touchingWallLeft || touchingWallRight);
        bool canGrabCeiling = allowCeilingGrab && touchingCeiling;

        if (canGrabWall || canGrabCeiling)
        {
            EnterGrip();
        }
    }

    private void EnterGrip()
    {
        isGripping = true;
        playerController.SetGrappleState(true);
    }

    // -----------------------------
    // MAINTAIN GRIP
    // -----------------------------
    private void MaintainGrip()
    {
        // Release grip
        if (!input.gripHeld)
        {
            ExitGrip();
            return;
        }

        // Allow turning while gripping
        if (input.moveInput.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (input.moveInput.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);

         // Jump out of wall grip
        if (input.jumpPressed)
        {
            ExitGrip();

            // Push player away from the wall
            float pushDir = touchingWallLeft ? 1f : -1f;
            playerController.SetVelocity(new Vector2(pushDir * 5f, 10f)); // tweak values

            return;
        }
   

        // Ceiling shimmy
        if (touchingCeiling && allowCeilingGrab)
        {
            float horizontal = input.moveInput.x;
            playerController.SetVelocity(new Vector2(horizontal * shimmySpeed, 0));
        }
        // Wall climb
        else if ((touchingWallLeft || touchingWallRight) && allowWallGrab)
        {
            float vertical = input.moveInput.y;
            playerController.SetVelocity(new Vector2(0, vertical * climbSpeed));
        }
        else
        {
            // Lost the surface
            ExitGrip();
        }
    }

    // -----------------------------
    // HAZARD BREAK
    // -----------------------------
    public void BreakGripFromHazard()
    {
        if (!hazardsBreakGrip)
            return;

        ExitGrip();
    }

    // -----------------------------
    // EXIT GRIP
    // -----------------------------
    private void ExitGrip()
    {
        isGripping = false;
        playerController.SetGrappleState(false);
    }
}
