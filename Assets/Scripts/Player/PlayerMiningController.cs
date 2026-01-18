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
        Debug.DrawRay(mineOrigin.position, GetMiningDirection() * mineRange, Color.red);
        Debug.DrawRay(mineOrigin.position, Vector2.down * mineRange, Color.blue);
        Debug.DrawRay(mineOrigin.position, Vector2.up * mineRange, Color.blue);

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
            Debug.DrawRay(hit.point, Vector3.up * 0.1f, Color.green, 0.2f);
            Tilemap tilemap = hit.collider.GetComponentInParent<Tilemap>();
            if (tilemap != null)
            {
                //Vector3 hitPos = hit.point + direction * 0.05f; // push slightly into the tile
                //Vector3Int cellPos = tilemap.WorldToCell(hit.point);
                //Vector2 direction = GetMiningDirection();
                Vector3 hitPos = hit.point + direction * 0.05f;   // push slightly inside the tile
                Vector3Int cellPos = tilemap.WorldToCell(hitPos);

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
