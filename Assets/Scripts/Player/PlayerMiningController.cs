using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMiningController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputHandler input;
    public Transform mineOrigin;          // Where the raycast starts (usually near player center)
    public LayerMask mineableLayer;

    [Header("Mining Settings")]
    public float mineRange = 1.2f;
    public float mineCooldown = 0.25f;

    private float mineTimer;

    void Update()
    {
        mineTimer -= Time.deltaTime;

        if (input.minePressed && mineTimer <= 0)
        {
            TryMine();
            mineTimer = mineCooldown;
        }
    }

    void TryMine()
    {
        Vector2 direction = GetMiningDirection();
        RaycastHit2D hit = Physics2D.Raycast(mineOrigin.position, direction, mineRange, mineableLayer);

        if (hit.collider != null)
        {
            Tilemap tilemap = hit.collider.GetComponentInParent<Tilemap>();
            if (tilemap != null)
            {
                Vector3Int cellPos = tilemap.WorldToCell(hit.point);
                TileDurabilityManager.Instance.Damage(cellPos, tilemap);
            }
        }
    }


    Vector2 GetMiningDirection()
    {
        // Vertical mining takes priority
        if (input.moveInput.y > 0.5f) return Vector2.up;
        if (input.moveInput.y < -0.5f) return Vector2.down;

        // Otherwise mine in facing direction
        float facing = transform.localScale.x > 0 ? 1 : -1;
        return new Vector2(facing, 0);
    }

    void OnDrawGizmosSelected()
    {
        if (mineOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(mineOrigin.position, mineOrigin.position + (Vector3)GetMiningDirection() * mineRange);
        }
    }
}
