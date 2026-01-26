using UnityEngine;
using UnityEngine.Tilemaps;

public class DualTileDebugHelper : MonoBehaviour
{
    [Header("References")]
    public PlayerMiningController controller;
    public Tilemap tilemap;

    // Look tile (from TryGetTargetCell)
    private Vector3Int lookCell;
    private WeakPointDirection lookWeakDir;
    private bool lookHitWeak;

    // Hit tile (from PerformMining)
    public static Vector3Int lastHitCell;
    public static WeakPointDirection lastHitWeakDir;
    public static bool lastHitWeak;

    private Vector2 mineDir;

    void Update()
    {
        if (controller == null || tilemap == null)
            return;

        mineDir = controller.GetMiningDirection();

        // Tile the player is LOOKING at
        if (controller.TryGetTargetCell(out Vector3Int cell, out Tilemap map))
        {
            lookCell = cell;
            lookWeakDir = TileDurabilityManager.Instance.GetWeakPoint(cell);
            lookHitWeak = controller.HitWeakPoint(mineDir, lookWeakDir);
        }
    }

    void OnGUI()
    {
        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(10, 10, 350, 200), GUI.skin.box);

        GUILayout.Label("<b><size=14>Dual Tile Debug</size></b>");

        GUILayout.Space(5);
        GUILayout.Label("<b>Mining Direction:</b> " + mineDir);

        GUILayout.Space(10);
        GUILayout.Label("<b>LOOK Tile (TryGetTargetCell)</b>");
        GUILayout.Label("Cell: " + lookCell);
        GUILayout.Label("WeakDir: " + lookWeakDir);
        GUILayout.Label("HitWeakPoint: " + lookHitWeak);

        GUILayout.Space(10);
        GUILayout.Label("<b>HIT Tile (PerformMining)</b>");
        GUILayout.Label("Cell: " + DualTileDebugHelper.lastHitCell);
        GUILayout.Label("WeakDir: " + DualTileDebugHelper.lastHitWeakDir);
        GUILayout.Label("HitWeakPoint: " + DualTileDebugHelper.lastHitWeak);

        GUILayout.Space(10);
        bool match = (lookCell == DualTileDebugHelper.lastHitCell);
        GUILayout.Label("<b>Tiles Match:</b> " + match);

        GUILayout.EndArea();
    }


    // Called by tool levels to record the actual hit tile
    public static void RecordHitTile(Vector3Int cell, WeakPointDirection weakDir, bool hitWeak)
    {
        lastHitCell = cell;
        lastHitWeakDir = weakDir;
        lastHitWeak = hitWeak;
    }
}
