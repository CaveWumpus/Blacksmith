using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMiningController : MonoBehaviour
{
    [Header("References")]
    public PlayerInputHandler input;
    public Transform mineOrigin;
    public LayerMask mineableLayer;

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
        else
        {
            // -----------------------------
            // TAP-TO-MINE (legacy mode)
            // -----------------------------
            if (input.minePressed && mineTimer <= 0)
            {
                TryMine();
                float cooldown = mineCooldown;

                if (useComboMining && comboModule != null)
                    cooldown = comboModule.GetModifiedCooldown(mineCooldown);
                // ⭐ Apply relic mining speed multiplier 
                float speedMult = RelicEffectResolver.GetMiningSpeedMultiplier(); 
                cooldown /= speedMult;

                mineTimer = cooldown;
            }
        }

        // Debug rays
        Debug.DrawRay(mineOrigin.position, GetMiningDirection() * mineRange, Color.red);
        Debug.DrawRay(mineOrigin.position, Vector2.down * mineRange, Color.blue);
        Debug.DrawRay(mineOrigin.position, Vector2.up * mineRange, Color.blue);
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
        
        if (toolManager.CurrentTool.mode == ToolMode.Hammer)
        {
            PerformHammer();
            return;
        }

        // WIDE SWING OVERRIDE
        if (toolManager.CurrentTool.mode == ToolMode.WideSwing)
        {
            PerformWideSwing();
            return;
        }

        var tool = toolManager.CurrentTool;

        Vector2 direction = GetMiningDirection();
        RaycastHit2D hit = Physics2D.Raycast(mineOrigin.position, direction, mineRange, mineableLayer);
        Debug.Log("Raycast hit: " + (hit.collider ? hit.collider.gameObject.name : "NULL"));
        if (hit.collider == null)
            return;

        Tilemap tilemap = hit.collider.GetComponentInParent<Tilemap>();
        TileData tileData = hit.collider.GetComponentInParent<TileData>();

        if (tilemap == null)
            return;

        Vector3 hitPos = hit.point + direction * 0.05f;
        Vector3Int cellPos = tilemap.WorldToCell(hitPos);

        int baseDamage = 1;
        float scaled = baseDamage * chargeMultiplier * tool.damageMultiplier;
        int finalDamage = Mathf.RoundToInt(scaled);
        //Debug.Log($"TileData at hit: {tileData?.weakPointDirection}");
        //Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
        

        // Weak point logic
        bool hitWeakSpot = tileData != null && HitWeakPoint(direction, tileData);

        if (hitWeakSpot)
        {
            finalDamage += Mathf.RoundToInt(tileData.tileDefinition.weakPointMultiplier);

            if (tileData.tileDefinition.weakPointVFX != null)
                Instantiate(tileData.tileDefinition.weakPointVFX, hit.point, Quaternion.identity);

            // ⭐ Precision Pick Level 2 ability: Weak Spot Chaining
            if (ToolXPManager.Instance.IsAbilityUnlocked(ToolMode.PrecisionPick, 1))
            {
                PerformWeakSpotChain(tilemap, cellPos, direction, finalDamage);
            }
        }

        var flash = tilemap.GetComponent<TileFlashEffect>();
        if (flash != null)
            flash.FlashTile(cellPos);

        TileDurabilityManager.Instance.Damage(cellPos, tilemap, finalDamage - 1);
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

        var tilemap = mineTilemap;

        var hitTiles = GetTilesInArc(origin, direction, swingRange, swingAngle);

        int hits = 0;

        foreach (var cell in hitTiles)
        {
            if (hits >= maxTilesHit)
                break;

            TileBase tile = tilemap.GetTile(cell);
            if (tile == null)
                continue;

            TileDurabilityManager.Instance.Damage(cell, tilemap, swingDamage - 1);
            hits++;
        }

        // TODO: Add swing animation, SFX, VFX
    }
    // ---------------------------------------------------------
    // ARC SELECTION FOR WIDE SWING
    // ---------------------------------------------------------
    private System.Collections.Generic.List<Vector3Int> GetTilesInArc(
        Vector2 origin, Vector2 direction, float range, float angle)
    {
        var results = new System.Collections.Generic.List<Vector3Int>();
        var tilemap = mineTilemap;

        int radius = Mathf.CeilToInt(range);

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3 worldPos = origin + new Vector2(x, y);
                Vector3Int cell = tilemap.WorldToCell(worldPos);

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
        drillTimer -= Time.deltaTime;
        if (drillTimer > 0f)
            return;

        drillTimer = drillTickRate;
        Debug.Log("Drill tick attempt");
        Debug.Log("drillTimer = " + drillTimer);


        Vector2 direction = GetMiningDirection();
        RaycastHit2D hit = Physics2D.Raycast(mineOrigin.position, direction, drillRange, mineableLayer);

        if (hit.collider == null)
            return;

        Tilemap tilemap = hit.collider.GetComponentInParent<Tilemap>();
        if (tilemap == null)
            return;

        Vector3 hitPos = hit.point + direction * 0.05f;
        Vector3Int cellPos = tilemap.WorldToCell(hitPos);

        // Drill damage ignores weak points for now
        TileDurabilityManager.Instance.Damage(cellPos, tilemap, drillDamage - 1);
        Debug.DrawRay(mineOrigin.position, direction * drillRange, Color.green);
        if (hit.collider == null)
        {
            Debug.Log("Drill raycast hit NOTHING");
            return;
        }
        if (tilemap == null)
        {
            Debug.Log("Drill found collider but no tilemap");
            return;
        }


        // TODO: Add drill VFX, sparks, sound loop
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

        Vector2 direction = GetMiningDirection();
        RaycastHit2D hit = Physics2D.Raycast(mineOrigin.position, direction, hammerRange, mineableLayer);

        if (hit.collider == null)
            return;

        Tilemap tilemap = hit.collider.GetComponentInParent<Tilemap>();
        if (tilemap == null)
            return;

        Vector3 hitPos = hit.point + direction * 0.05f;
        Vector3Int centerCell = tilemap.WorldToCell(hitPos);

        // AoE damage
        for (int x = -hammerRadius; x <= hammerRadius; x++)
        {
            for (int y = -hammerRadius; y <= hammerRadius; y++)
            {
                Vector3Int cell = new Vector3Int(centerCell.x + x, centerCell.y + y, centerCell.z);
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
    // TAP-TO-MINE (legacy)
    // ---------------------------------------------------------
    void TryMine()
    {
        Vector2 direction = GetMiningDirection();
        RaycastHit2D hit = Physics2D.Raycast(mineOrigin.position, direction, mineRange, mineableLayer);

        if (hit.collider != null)
        {
            Tilemap tilemap = hit.collider.GetComponentInParent<Tilemap>();
            if (tilemap != null)
            {
                Vector3 hitPos = hit.point + direction * 0.05f;
                Vector3Int cellPos = tilemap.WorldToCell(hitPos);

                int bonus = 0;
                if (useComboMining && comboModule != null)
                    bonus = comboModule.GetBonusDamage();

                TileDurabilityManager.Instance.Damage(cellPos, tilemap, bonus);

                if (useComboMining && comboModule != null)
                    comboModule.RegisterSuccessfulHit();
            }
        }
    }


    // ---------------------------------------------------------
    // MINING DIRECTION
    // ---------------------------------------------------------
    Vector2 GetMiningDirection()
    {
        if (input.moveInput.y > 0.5f) return Vector2.up;
        if (input.moveInput.y < -0.5f) return Vector2.down;

        float facing = transform.localScale.x > 0 ? 1 : -1;
        return new Vector2(facing, 0);
    }
    private bool HitWeakPoint(Vector2 direction, TileData tileData)
    {
        if (tileData == null || !tileData.hasWeakPoint)
            return false;

        switch (tileData.weakPointDirection)
        {
            case WeakPointDirection.Left:  return direction.x < 0;
            case WeakPointDirection.Right: return direction.x > 0;
            case WeakPointDirection.Up:    return direction.y > 0;
            case WeakPointDirection.Down:  return direction.y < 0;
        }

        return false;
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

