using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isGrounded = false;

    private PlayerControls controls;
    private int facingDirection = 1; // 1 = right, -1 = left

    [Header("Layers")]
    public LayerMask groundAndWallLayer; // assign Ground + Wall in Inspector
    public LayerMask miningLayer;        // assign Ground + Wall + Ore in Inspector

    [Header("Mining Indicator")]
    public GameObject miningIndicatorPrefab;
    private GameObject miningIndicatorInstance;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Mine.performed += ctx => Mine();
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (miningIndicatorPrefab != null)
        {
            miningIndicatorInstance = Instantiate(miningIndicatorPrefab);
            miningIndicatorInstance.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
        movement = controls.Player.Move.ReadValue<Vector2>();

        if (movement.x > 0.1f) facingDirection = 1;
        else if (movement.x < -0.1f) facingDirection = -1;

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundAndWallLayer);
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void Mine()
    {
        if (movement.y > 0.5f)       MineUpward();
        else if (movement.y < -0.5f) MineDownward();
        else if (Mathf.Abs(movement.x) > 0.3f) MineForward();
    }

    void MineForward()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(facingDirection * 0.4f, 0);
        Vector2 direction = new Vector2(facingDirection, 0);

        Debug.DrawRay(origin, direction * 1.5f, Color.red, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 1.5f, miningLayer);

        if (hit.collider != null && hit.collider.gameObject != gameObject)
            HandleMineHit(hit);
    }

    void MineUpward()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.4f;
        Vector2 direction = Vector2.up;

        Debug.DrawRay(origin, direction * 1f, Color.green, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 1f, miningLayer);

        if (hit.collider != null && hit.collider.gameObject != gameObject)
            HandleMineHit(hit);
    }

    void MineDownward()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.4f;
        Vector2 direction = Vector2.down;

        Debug.DrawRay(origin, direction * 2f, Color.blue, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 2f, miningLayer);

        if (hit.collider != null && hit.collider.gameObject != gameObject)
            HandleMineHit(hit);
    }

    private void HandleMineHit(RaycastHit2D hit)
    {
        if (hit.collider == null) return;

        Tilemap tilemap = hit.collider.GetComponent<Tilemap>();
        if (tilemap == null) return;

        Vector3Int cellPos = tilemap.WorldToCell(hit.point);
        Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);
        TileBase tile = tilemap.GetTile(cellPos);

        Debug.Log($"Hit point: {hit.point}, Cell: {cellPos}, Center: {cellCenter}, Tile: {tile}");

        if (tile == null)
        {
            Debug.LogWarning($"No tile found at {cellPos} even though collider was hit!");
            return;
        }

        if (tile is MineableTile mineable)
        {
            TileDurabilityManager.Instance.Damage(cellPos, tilemap);
        }
    }

    void ShowMiningIndicator(Vector3 targetPos)
    {
        if (miningIndicatorInstance != null)
        {
            miningIndicatorInstance.SetActive(true);
            miningIndicatorInstance.transform.position = targetPos;
        }
    }

    void HideMiningIndicator()
    {
        if (miningIndicatorInstance != null)
        {
            miningIndicatorInstance.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("Spawn collision with: " + col.collider.name);
    }
}
