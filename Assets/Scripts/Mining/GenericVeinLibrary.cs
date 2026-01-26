using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GenericVeinLibrary : MonoBehaviour
{
    public static GenericVeinLibrary Instance { get; private set; }

    [Header("Definitions (Manual Mode)")]
    public bool useAutoLoad = true;

    // We now store GenericVeinDefinition, not GenericVeinTile
    public List<GenericVeinDefinition> genericVeins = new List<GenericVeinDefinition>();

    private Dictionary<(TileType, RarityTier), GenericVeinDefinition> lookup =
        new Dictionary<(TileType, RarityTier), GenericVeinDefinition>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

#if UNITY_EDITOR
        if (useAutoLoad)
            AutoLoadGenericVeins();
#endif

        BuildLookup();
    }

#if UNITY_EDITOR
    private void AutoLoadGenericVeins()
    {
        genericVeins = LoadAllAssetsOfType<GenericVeinDefinition>();
        Debug.Log($"[GenericVeinLibrary] Auto‑loaded {genericVeins.Count} GenericVeinDefinitions.");
    }

    private List<T> LoadAllAssetsOfType<T>() where T : ScriptableObject
    {
        var assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                assets.Add(asset);
        }

        return assets;
    }
#endif

    private void BuildLookup()
    {
        lookup.Clear();

        foreach (var gv in genericVeins)
        {
            if (gv == null || gv.tileAsset == null)
                continue;

            var key = (gv.tileType, gv.rarity);
            lookup[key] = gv;
        }

        Debug.Log($"[GenericVeinLibrary] Built lookup for {lookup.Count} generic vein entries.");
    }

    public GenericVeinDefinition GetGenericDefinition(TileType type, RarityTier rarity)
    {
        var key = (type, rarity);
        if (lookup.TryGetValue(key, out var gv) && gv != null)
            return gv;

        Debug.LogWarning($"[GenericVeinLibrary] No generic vein definition for {type} / {rarity}.");
        return null;
    }
}
