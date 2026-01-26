using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class PlayerMiningController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputHandler input;
    public Transform mineOrigin;
    //public LayerMask mineableLayer;

    [Header("Mining Settings")]
    public float mineRange = 1.2f;
    public float mineCooldown = 0.25f;

    [Header("Tool Mode")]
    public ToolModeManager toolManager;


    [Header("Combo Mining")]
    public bool useComboMining = false;
    public ComboMiningModule comboModule;

    [Header("UI")]
    public ChargeBarUI chargeBarUI;

    [Header("Hold-to-Mine")]
    public bool holdToMineEnabled = true;
    public float maxChargeTime = 1.2f;
    public AnimationCurve chargeCurve;
    public float minChargeToMine = 0.1f;

    [Header("Wide Swing Settings")]
    public float swingRange = 2f;          // how far the arc reaches
    public float swingAngle = 60f;         // arc width in degrees
    public int swingDamage = 1;            // base damage per tile
    public int maxTilesHit = 5;            // optional cap

    [Header("Drill Settings")]
    public float drillTickRate = 0.15f;   // how often it deals damage
    public int drillDamage = 1;           // damage per tick
    public float drillRange = 1.2f;       // usually same as mineRange
    private float drillTimer = 0f;

    [Header("Hammer Settings")]
    public int hammerDamage = 3;      // base damage per tile
    public int hammerRadius = 1;      // AoE radius in tiles
    public float hammerRange = 1.2f;  // same as mineRange usually
    public float hammerCooldown = 0.5f;
    private float hammerTimer = 0f;

    [Header("Debug Gizmos")]
    public bool showMiningGizmos = true;


    public static PlayerMiningController Instance { get; private set; }

    [Header("Tilemap Reference (for Wide Swing)")]
    public Tilemap mineTilemap;

    private bool isCharging = false;
    private float currentCharge = 0f;

    private float mineTimer;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        mineTimer -= Time.deltaTime;
        hammerTimer -= Time.deltaTime;

        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            if (TryGetTargetCell(out Vector3Int cell, out Tilemap map))
                TileDurabilityManager.Instance.DebugWeakPoint(cell);
        }


        // DRILL continuous mining
        if (toolManager.CurrentTool.mode == ToolMode.Drill && input.mineHeld)
        {
            PerformDrill();
        }

        
        // Update combo state
        if (useComboMining && comboModule != null)
            comboModule.UpdateComboState(Time.deltaTime);

        // -----------------------------
        // HOLD-TO-MINE INPUT HANDLING
        // -----------------------------
        if (holdToMineEnabled)
        {
            // Start charging
            if (input.mineStarted)
            {
                isCharging = true;
                currentCharge = 0f;
            }

            // Continue charging
            if (isCharging && input.mineHeld)
            {
                float speed = toolManager.CurrentTool.chargeSpeedMultiplier;
                currentCharge += Time.deltaTime * speed;
                currentCharge = Mathf.Clamp(currentCharge, 0f, maxChargeTime);
            }

            if (chargeBarUI != null && holdToMineEnabled)
            {
                float normalized = maxChargeTime > 0f 
                    ? currentCharge / maxChargeTime 
                    : 0f;

                chargeBarUI.SetCharge(normalized);
            }
            //Debug.Log($"Started:{input.mineStarted} Held:{input.mineHeld} Released:{input.mineReleased}");


            // Release charge
            if (input.mineReleased)
            {
                ReleaseMiningCharge();
            }
        }
        
    }

    // ---------------------------------------------------------
    // HOLD-TO-MINE RELEASE LOGIC
    // ---------------------------------------------------------
    private void ReleaseMiningCharge()
    {
        if (!isCharging)
            return;

        isCharging = false;

        // Ignore micro-taps
        if (currentCharge < minChargeToMine)
            return;

        float chargePercent = currentCharge / maxChargeTime;
        float chargeMultiplier = chargeCurve.Evaluate(chargePercent);

        PerformChargedMine(chargeMultiplier);

        // Reset charge
        currentCharge = 0f;
    }

    // ---------------------------------------------------------
    // CHARGED MINING HIT
    // ---------------------------------------------------------
    private void PerformChargedMine(float chargeMultiplier)
    {
        if (!TryGetTargetCell(out Vector3Int cellPos, out Tilemap tilemap))
            return;
        Debug.Log("Using tool: " + toolManager.CurrentTool.name);

        toolManager.CurrentLevel.PerformMining(this, cellPos, tilemap, chargeMultiplier);
    }

    public int ComputeBaseDamage(float chargeMultiplier)
    {
        var tool = toolManager.CurrentTool;

        if (tool.overrideDamage)
            return tool.debugDamageValue;

        float scaled = tool.baseDamage * chargeMultiplier * tool.damageMultiplier;
        return Mathf.Max(1, Mathf.RoundToInt(scaled));
    }

    private void PerformWeakSpotChain(Tilemap tilemap, Vector3Int startCell, Vector2 direction, int initialDamage)
    {
        int chainLength = 2; // Level 2 ability: chain 2 tiles
        float falloff = 1f;

        int damage = initialDamage;

        Vector3Int currentCell = startCell;

        for (int i = 0; i < chainLength; i++)
        {
            // Move one tile forward in the mining direction
            currentCell += new Vector3Int(
                Mathf.RoundToInt(direction.x),
                Mathf.RoundToInt(direction.y),
                0
            );

            TileBase tile = tilemap.GetTile(currentCell);
            if (tile == null)
                break;

            int dmgToApply = Mathf.Max(1, Mathf.RoundToInt(damage * falloff));

            TileDurabilityManager.Instance.Damage(currentCell, tilemap, dmgToApply - 1);

            damage = dmgToApply;
        }
    }

    // ---------------------------------------------------------
    // WIDE SWING MINING
    // ---------------------------------------------------------
    private void PerformWideSwing()
    {
        Vector2 origin = mineOrigin.position;
        Vector2 direction = GetMiningDirection();
        Tilemap tilemap = mineTilemap;

        // Get all tiles in the arc
        var hitTiles = GetTilesInArc(origin, direction, swingRange, swingAngle);

        int hits = 0;

        foreach (var cell in hitTiles)
        {
            if (hits >= maxTilesHit)
                break;

            TileBase tile = tilemap.GetTile(cell);
            if (tile == null)
                continue;

            // Flash effect
            var flash = tilemap.GetComponent<TileFlashEffect>();
            if (flash != null)
                flash.FlashTile(cell);

            // Apply damage
            TileDurabilityManager.Instance.Damage(cell, tilemap, swingDamage - 1);
            hits++;
        }

        // TODO: Add swing animation, SFX, VFX
    }

    // ---------------------------------------------------------
    // ARC SELECTION FOR WIDE SWING
    // ---------------------------------------------------------
    private List<Vector3Int> GetTilesInArc(
    Vector2 origin, Vector2 direction, float range, float angle)
    {
        var results = new List<Vector3Int>();
        Tilemap tilemap = mineTilemap;

        int radius = Mathf.CeilToInt(range);

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3 worldPos = origin + new Vector2(x, y);
                Vector3Int cell = tilemap.WorldToCell(worldPos);

                // Avoid duplicates
                if (results.Contains(cell))
                    continue;

                Vector2 toTile = (Vector2)(tilemap.GetCellCenterWorld(cell) - (Vector3)origin);
                float dist = toTile.magnitude;

                if (dist > range)
                    continue;

                float dot = Vector2.Dot(direction.normalized, toTile.normalized);
                float degrees = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (degrees <= angle * 0.5f)
                    results.Add(cell);
            }
        }

        return results;
    }

    // ---------------------------------------------------------
    // DRILL MINING (continuous damage while held)
    // ---------------------------------------------------------
    private void PerformDrill()
    {
        // Tick timer
        drillTimer -= Time.deltaTime;
        if (drillTimer > 0f)
            return;

        drillTimer = drillTickRate;

        // ⭐ NEW: grid-based targeting
        if (!TryGetTargetCell(out Vector3Int cellPos, out Tilemap tilemap))
            return;

        TileBase tile = tilemap.GetTile(cellPos);
        if (tile == null)
            return;

        // Lookup tile definition
        TileDefinition def;
        if (!TileDurabilityManager.Instance.TryGetDefinition(tile, out def))
            return;

        // Drill damage ignores weak points (for now)
        int damage = drillDamage - 1;

        // Flash effect
        var flash = tilemap.GetComponent<TileFlashEffect>();
        if (flash != null)
            flash.FlashTile(cellPos);

        // Apply damage
        TileDurabilityManager.Instance.Damage(cellPos, tilemap, damage);
    }

    public void ResetDrillTimer()
    {
        drillTimer = 0f;
    }
    // ---------------------------------------------------------
    // HAMMER MINING (burst AoE impact)
    // ---------------------------------------------------------
    private void PerformHammer()
    {
        if (hammerTimer > 0f)
            return;

        hammerTimer = hammerCooldown;

        // ⭐ NEW: grid-based targeting
        if (!TryGetTargetCell(out Vector3Int centerCell, out Tilemap tilemap))
            return;

        // AoE damage around the center cell
        for (int x = -hammerRadius; x <= hammerRadius; x++)
        {
            for (int y = -hammerRadius; y <= hammerRadius; y++)
            {
                Vector3Int cell = new Vector3Int(centerCell.x + x, centerCell.y + y, centerCell.z);

                TileBase tile = tilemap.GetTile(cell);
                if (tile == null)
                    continue;

                // Flash effect
                var flash = tilemap.GetComponent<TileFlashEffect>();
                if (flash != null)
                    flash.FlashTile(cell);

                // Apply damage
                TileDurabilityManager.Instance.Damage(cell, tilemap, hammerDamage - 1);
            }
        }

        // TODO: Add hammer VFX, shockwave, screen shake, sound
    }

    private void MineAoE(Vector3Int center, Tilemap tilemap, int damage, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int pos = new Vector3Int(center.x + x, center.y + y, center.z);
                TileDurabilityManager.Instance.Damage(pos, tilemap, damage - 1);
            }
        }
    }

    // ---------------------------------------------------------
    // MINING DIRECTION
    // ---------------------------------------------------------
    public Vector2 GetMiningDirection()
    {
        if (input.moveInput.y > 0.5f) return Vector2.up;
        if (input.moveInput.y < -0.5f) return Vector2.down;

        float facing = transform.localScale.x > 0 ? 1 : -1;
        return new Vector2(facing, 0);
    }
    public bool HitWeakPoint(Vector2 direction, WeakPointDirection weakDir)
    {
        if (weakDir == WeakPointDirection.None)
            return false;

        switch (weakDir)
        {
            case WeakPointDirection.Left:  return direction.x < 0;
            case WeakPointDirection.Right: return direction.x > 0;
            case WeakPointDirection.Up:    return direction.y > 0;
            case WeakPointDirection.Down:  return direction.y < 0;
        }

        return false;
    }

    public bool TryGetTargetCell(out Vector3Int cellPos, out Tilemap tilemap)
    {
        tilemap = mineTilemap;
        cellPos = Vector3Int.zero;

        if (tilemap == null)
            return false;

        Vector2 direction = GetMiningDirection();
        Vector3 originWorld = mineOrigin.position;

        // Convert origin to starting cell
        Vector3Int startCell = tilemap.WorldToCell(originWorld);

        // How many tiles to step based on mineRange
        int maxSteps = Mathf.CeilToInt(mineRange);

        // Step direction in grid space
        Vector3Int step = new Vector3Int(
            Mathf.RoundToInt(direction.x),
            Mathf.RoundToInt(direction.y),
            0
        );

        Vector3Int current = startCell;

        for (int i = 0; i < maxSteps; i++)
        {
            current += step;

            TileBase tile = tilemap.GetTile(current);
            if (tile != null)
            {
                cellPos = current;
                return true;
            }
        }

        return false;
    }
    private void OnDrawGizmos()
    {
        if (!showMiningGizmos || mineTilemap == null || mineOrigin == null)
            return;

        Vector2 direction = GetMiningDirection();
        Vector3 origin = mineOrigin.position;

        // -----------------------------------------
        // 1. Mining direction line
        // -----------------------------------------
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + (Vector3)direction * mineRange);

        // -----------------------------------------
        // 2. Target cell highlight
        // -----------------------------------------
        if (Application.isPlaying)
        {
            if (TryGetTargetCell(out Vector3Int cellPos, out Tilemap tilemap))
            {
                Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(cellCenter, Vector3.one * 0.9f);
            }
        }

        // -----------------------------------------
        // 3. Hammer AoE radius
        // -----------------------------------------
        if (toolManager != null && toolManager.CurrentTool.mode == ToolMode.Hammer)
        {
            if (Application.isPlaying && TryGetTargetCell(out Vector3Int hammerCell, out Tilemap tilemap))
            {
                Vector3 center = tilemap.GetCellCenterWorld(hammerCell);
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
                Gizmos.DrawWireSphere(center, hammerRadius + 0.5f);
            }
        }

        // -----------------------------------------
        // 4. Wide Swing arc
        // -----------------------------------------
        if (toolManager != null && toolManager.CurrentTool.mode == ToolMode.WideSwing)
        {
            DrawSwingArc(origin, direction, swingRange, swingAngle);
        }
    }
    private void DrawSwingArc(Vector3 origin, Vector2 direction, float range, float angle)
    {
        Gizmos.color = new Color(0f, 0.7f, 1f, 0.4f);

        int segments = 30;
        float halfAngle = angle * 0.5f;
        float startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - halfAngle;

        Vector3 prevPoint = origin;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float currentAngle = startAngle + t * angle;
            float rad = currentAngle * Mathf.Deg2Rad;

            Vector3 nextPoint = origin + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * range;

            if (i > 0)
                Gizmos.DrawLine(prevPoint, nextPoint);

            prevPoint = nextPoint;
        }
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

