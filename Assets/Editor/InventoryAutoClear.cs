using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class InventoryAutoClear
{
    static InventoryAutoClear()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            var inv = Object.FindFirstObjectByType<UnifiedInventoryController>();
            if (inv != null && inv.inventoryData != null)
            {
                inv.inventoryData.ClearAllSlots();

                if (inv.inventoryUI != null)
                    inv.inventoryUI.RefreshAllSlots();

                Debug.Log("Inventory auto‑cleared on exiting Play Mode.");
            }
        }
    }
}
