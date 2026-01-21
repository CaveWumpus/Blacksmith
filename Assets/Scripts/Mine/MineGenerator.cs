using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MineGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 30;
    public Tilemap tilemap;

    [Header("Tiles")]
    public TileBase perimeterTile;

    [Header("Definitions (Manual Mode)")]
    public bool useAutoLoad = true;

    public List<RockDefinition> rockDefinitions = new List<RockDefinition>();
    public List<VeinDefinition> oreDefinitions = new List<VeinDefinition>();
    public List<VeinDefinition> gemDefinitions = new List<VeinDefinition>();
    public List<RelicDefinition> relicDefinitions = new List<RelicDefinition>();
    public List<RecipeDefinition> recipeDefinitions = new List<RecipeDefinition>();
    public List<PatternDefinition> patternDefinitions = new List<PatternDefinition>();

    [Header("Biome")]
    public BiomeDefinition biome;

    [Header("Player")]
    public Transform player;
    public int playerLevel = 1;

    [Header("Exit Settings")]
    public GameObject exitTriggerPrefab;

    [Header("Debug")]
    public bool showWeakPointIndicators = false;


    private bool[,] grid;
    private Vector2 spawnPoint;
    private int startX, startY;
    private bool startOnLeft;

    private void Start()
    {
#if UNITY_EDITOR
        if (useAutoLoad)
            AutoLoadDefinitions();
#endif
        GenerateMine();
    }

#if UNITY_EDITOR
    // ---------------------------------------------------------
    // AUTO‑LOAD ALL SCRIPTABLEOBJECT DEFINITIONS (Option C)
    // ---------------------------------------------------------
    private void AutoLoadDefinitions()
    {
        rockDefinitions = LoadAll<RockDefinition>();

        var allVeins = LoadAll<VeinDefinition>();
        oreDefinitions = allVeins.Where(v => v.tileType == TileType.Ore).ToList();
        gemDefinitions = allVeins.Where(v => v.tileType == TileType.Gem).ToList();

        relicDefinitions = LoadAll<RelicDefinition>();
        recipeDefinitions = LoadAll<RecipeDefinition>();
        patternDefinitions = LoadAll<PatternDefinition>();

        // Auto‑assign a biome if none set
        if (biome == null)
        {
            var biomes = LoadAll<BiomeDefinition>();
            if (biomes.Count > 0)
                biome = biomes[0];
        }

        Debug.Log("[MineGenerator] Auto‑loaded all definitions via AssetDatabase.");
    }

    private List<T> LoadAll<T>() where T : ScriptableObject
    {
        var list = new List<T>();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                list.Add(asset);
        }

        return list;
    }
#endif

    // ---------------------------------------------------------
    // MAIN GENERATION
    // ---------------------------------------------------------
    public void GenerateMine()
    {
        grid = new bool[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = true;

        for (int x = 0; x < width; x++)
        {
            grid[x, 0] = true;
            grid[x, height - 1] = true;
        }
        for (int y = 0; y < height; y++)
        {
            grid[0, y] = true;
            grid[width - 1, y] = true;
        }

        startOnLeft = Random.value < 0.5f;
        startX = startOnLeft ? 1 : width - 5;
        startY = Random.Range(height / 3, (height * 2) / 3);

        for (int x = startX; x < startX + 4; x++)
            for (int y = startY; y < startY + 4; y++)
                grid[x, y] = false;

        if (startOnLeft)
            for (int y = startY; y < startY + 4; y++)
                grid[0, y] = false;
        else
            for (int y = startY; y < startY + 4; y++)
                grid[width - 1, y] = false;

        int tunnelLength = 6;
        if (startOnLeft)
        {
            for (int x = 0; x < tunnelLength; x++)
                for (int y = startY + 1; y < startY + 3; y++)
                    grid[startX + x, y] = false;
        }
        else
        {
            for (int x = 0; x < tunnelLength; x++)
                for (int y = startY + 1; y < startY + 3; y++)
                    grid[startX - x, y] = false;
        }

        PlaceExitTrigger();

        spawnPoint = new Vector2(startX + 2.5f, startY + 2.5f);

        RenderGridToTilemap();

        tilemap.RefreshAllTiles();

        if (player != null)
            StartCoroutine(SpawnNextFrame());
    }

    // ---------------------------------------------------------
    // EXIT TRIGGER
    // ---------------------------------------------------------
    private void PlaceExitTrigger()
    {
        if (exitTriggerPrefab == null || tilemap == null)
            return;

        Vector3Int cellPos = startOnLeft
            ? new Vector3Int(0, startY + 2, 0)
            : new Vector3Int(width - 1, startY + 2, 0);

        Vector3 exitPos = tilemap.GetCellCenterWorld(cellPos);
        GameObject trigger = Instantiate(exitTriggerPrefab, exitPos, Quaternion.identity);

        BoxCollider2D box = trigger.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.isTrigger = true;
            box.size = new Vector2(1f, 4f);
            box.offset = Vector2.zero;
        }
    }

    // ---------------------------------------------------------
    // RENDER GRID USING NEW SYSTEM
    // ---------------------------------------------------------
    private void RenderGridToTilemap()
    {
        tilemap.ClearAllTiles();

        MineGenerationContext ctx = new MineGenerationContext
        {
            playerLevel = playerLevel,
            biome = biome
        };

        var validRocks = rockDefinitions
            .Where(r => playerLevel >= r.levelStart && playerLevel <= r.levelEnd)
            .Where(r => biome.allowedRocks.Contains(r))
            .ToList();

        var validOre = oreDefinitions
            .Where(v => playerLevel >= v.levelStart && playerLevel <= v.levelEnd)
            .ToList();

        var validGem = gemDefinitions
            .Where(v => playerLevel >= v.levelStart && playerLevel <= v.levelEnd)
            .ToList();

        var validRelic = relicDefinitions
            .Where(r => playerLevel >= r.levelStart && playerLevel <= r.levelEnd)
            .ToList();

        var validRecipe = recipeDefinitions
            .Where(r => playerLevel >= r.levelStart && playerLevel <= r.levelEnd)
            .ToList();

        var validPattern = patternDefinitions
            .Where(p => playerLevel >= p.levelStart && playerLevel <= p.levelEnd)
            .ToList();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                if (!grid[x, y])
                {
                    tilemap.SetTile(cellPos, null);
                    continue;
                }

                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    tilemap.SetTile(cellPos, perimeterTile);
                    continue;
                }

                RockDefinition rock = GetWeightedRock(validRocks, biome);

                tilemap.SetTile(cellPos, rock.tileAsset);
                if (showWeakPointIndicators &&
                    rock.weakPointDirection != WeakPointDirection.None &&
                    rock.weakPointIndicatorPrefab != null)
                {
                    Vector3 worldPos = tilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0);
                    GameObject indicator = Instantiate(
                        rock.weakPointIndicatorPrefab,
                        worldPos,
                        Quaternion.identity,
                        tilemap.transform
                    );

                    // Rotate based on weak point direction
                    switch (rock.weakPointDirection)
                    {
                        case WeakPointDirection.Left:
                            indicator.transform.rotation = Quaternion.Euler(0, 0, 180);
                            break;
                        case WeakPointDirection.Right:
                            indicator.transform.rotation = Quaternion.Euler(0, 0, 0);
                            break;
                        case WeakPointDirection.Up:
                            indicator.transform.rotation = Quaternion.Euler(0, 0, 90);
                            break;
                        case WeakPointDirection.Down:
                            indicator.transform.rotation = Quaternion.Euler(0, 0, 270);
                            break;
                    }

                    TileDurabilityManager.Instance.RegisterIndicator(cellPos, indicator);
                }



                DropResult drop = DropRoller.Roll(
                    rock, ctx,
                    validOre, validGem,
                    validRelic, validRecipe, validPattern
                );

                TileDurabilityManager.Instance.AssignTile(cellPos, rock, drop);
            }
        }
    }

    // ---------------------------------------------------------
    // WEIGHTED ROCK SELECTION
    // ---------------------------------------------------------
    private RockDefinition GetWeightedRock(List<RockDefinition> rocks, BiomeDefinition biome)
    {
        float total = 0f;

        foreach (var r in rocks)
        {
            float weight = r.spawnWeight;

            foreach (var mod in biome.rockModifiers)
            {
                if (mod.rock == r)
                    weight *= mod.multiplier;
            }

            total += weight;
        }

        float roll = Random.value * total;

        foreach (var r in rocks)
        {
            float weight = r.spawnWeight;

            foreach (var mod in biome.rockModifiers)
            {
                if (mod.rock == r)
                    weight *= mod.multiplier;
            }

            if (roll < weight)
                return r;

            roll -= weight;
        }

        return rocks[0];
    }

    // ---------------------------------------------------------
    // PLAYER SPAWN
    // ---------------------------------------------------------
    private IEnumerator SpawnNextFrame()
    {
        yield return null;
        player.position = spawnPoint;
        player.GetComponent<Rigidbody2D>().freezeRotation = true;
    }
}
