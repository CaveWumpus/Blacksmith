using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MineGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 30;
    public Tilemap tilemap;

    [Header("Tiles")]
    public TileBase perimeterTile;

    [Header("Definitions")]
    public List<RockDefinition> rockDefinitions;
    public List<VeinDefinition> oreDefinitions;
    public List<VeinDefinition> gemDefinitions;
    public List<RelicDefinition> relicDefinitions;
    public List<RecipeDefinition> recipeDefinitions;
    public List<PatternDefinition> patternDefinitions;

    [Header("Biome")]
    public BiomeDefinition biome;

    [Header("Player")]
    public Transform player;
    public int playerLevel = 1;

    [Header("Exit Settings")]
    public GameObject exitTriggerPrefab;

    private bool[,] grid;
    private Vector2 spawnPoint;
    private int startX, startY;
    private bool startOnLeft;

    void Start()
    {
        GenerateMine();
    }

    // ---------------------------------------------------------
    // MAIN GENERATION
    // ---------------------------------------------------------
    public void GenerateMine()
    {
        grid = new bool[width, height];

        // Fill with solid
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = true;

        // Perimeter walls
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

        // Entrance room
        startOnLeft = Random.value < 0.5f;
        startX = startOnLeft ? 1 : width - 5;
        startY = Random.Range(height / 3, (height * 2) / 3);

        for (int x = startX; x < startX + 4; x++)
            for (int y = startY; y < startY + 4; y++)
                grid[x, y] = false;

        // Open perimeter at entrance
        if (startOnLeft)
            for (int y = startY; y < startY + 4; y++)
                grid[0, y] = false;
        else
            for (int y = startY; y < startY + 4; y++)
                grid[width - 1, y] = false;

        // Tunnel
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

        // Filter pools by level
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

        // Loop through grid
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

                // Perimeter = unmineable
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    tilemap.SetTile(cellPos, perimeterTile);
                    continue;
                }

                // Pick a weighted rock
                RockDefinition rock = GetWeightedRock(validRocks, biome);

                tilemap.SetTile(cellPos, rock.tileAsset);

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
