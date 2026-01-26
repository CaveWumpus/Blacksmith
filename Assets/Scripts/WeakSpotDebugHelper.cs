using UnityEngine;
using UnityEngine.Tilemaps;

public class WeakSpotDebugHelper : MonoBehaviour
{
    public PlayerMiningController controller;
    public Tilemap tilemap;

    private Vector3Int lastCell;
    private WeakPointDirection lastWeakDir;
    private Vector2 lastMineDir;
    private bool lastHit;

    void Update()
    {
        if (controller == null || tilemap == null)
            return;

        // Get current mining direction
        lastMineDir = controller.GetMiningDirection();

        // Get the tile you're currently targeting
        if (controller.TryGetTargetCell(out Vector3Int cell, out Tilemap map))
        {
            lastCell = cell;
            lastWeakDir = TileDurabilityManager.Instance.GetWeakPoint(cell);
            lastHit = controller.HitWeakPoint(lastMineDir, lastWeakDir);
        }
    }

    void OnGUI()
    {
        // Draw a small debug window in the corner
        GUI.color = Color.white;
        GUILayout.BeginArea(new Rect(10, 10, 300, 150), GUI.skin.box);

        GUILayout.Label("<b><size=14>Weak Spot Debug</size></b>");

        GUILayout.Label($"Cell: {lastCell}");
        GUILayout.Label($"WeakDir: {lastWeakDir}");
        GUILayout.Label($"MineDir: {lastMineDir}");
        GUILayout.Label($"HitWeakPoint: {lastHit}");

        GUILayout.EndArea();
    }
}
