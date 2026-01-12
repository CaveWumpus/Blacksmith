using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class MineGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 30;
    public Tilemap tilemap;

    [Header("Tiles")]
    public Tile groundTile; // perimeter walls
    public TilePool rockPool; // pool of rock tiles (SoftRock, HardRock, etc.)

    [Header("Player")]
    public Transform player;
    public int playerLevel = 1; // progression level

    [Header("Exit Settings")]
    public GameObject exitTriggerPrefab; // assign ExitTrigger prefab here

    private bool[,] grid;
    private Vector2 spawnPoint;
    private int startX, startY;
    private bool startOnLeft;

    void Start()
    {
        GenerateMine();
    }

    public void GenerateMine()
    {
        grid = new bool[width, height];

        // Step 1: Fill grid with solid walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = true;
            }
        }

        // Step 2: Enforce perimeter walls
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

        // Step 3: Carve entrance room
        startOnLeft = Random.value < 0.5f;
        startX = startOnLeft ? 1 : width - 5;
        startY = Random.Range(height / 3, (height * 2) / 3);

        for (int x = startX; x < startX + 4; x++)
        {
            for (int y = startY; y < startY + 4; y++)
            {
                grid[x, y] = false;
            }
        }

        // Step 4: Keep perimeter open at entrance
        if (startOnLeft)
        {
            for (int y = startY; y < startY + 4; y++)
                grid[0, y] = false;
        }
        else
        {
            for (int y = startY; y < startY + 4; y++)
                grid[width - 1, y] = false;
        }

        // Step 5: Guaranteed tunnel carve into interior
        int tunnelLength = 6;
        if (startOnLeft)
        {
            for (int x = 0; x < tunnelLength; x++)
            {
                for (int y = startY + 1; y < startY + 3; y++)
                {
                    grid[startX + x, y] = false;
                }
            }
        }
        else
        {
            for (int x = 0; x < tunnelLength; x++)
            {
                for (int y = startY + 1; y < startY + 3; y++)
                {
                    grid[startX - x, y] = false;
                }
            }
        }

        // Step 5b: Place exit trigger at the carved opening
        if (exitTriggerPrefab != null && tilemap != null)
        {
            // Pick the cell in the middle of the opening
            Vector3Int cellPos;
            if (startOnLeft)
                cellPos = new Vector3Int(0, startY + 2, 0); // left wall opening
            else
                cellPos = new Vector3Int(width - 1, startY + 2, 0); // right wall opening

            // Convert cell coordinates to world position
            Vector3 exitPos = tilemap.GetCellCenterWorld(cellPos);

            // Spawn the trigger prefab at the correct world position
            GameObject trigger = Instantiate(exitTriggerPrefab, exitPos, Quaternion.identity);

            // Adjust collider size to cover the 4‑tile tall opening
            BoxCollider2D box = trigger.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                box.isTrigger = true;
                box.size = new Vector2(1f, 4f);   // 1 tile wide, 4 tiles tall
                box.offset = Vector2.zero;        // center on prefab origin
            }
        }



        
        // Save spawn point in center of room
        spawnPoint = new Vector2(startX + 2.5f, startY + 2.5f);

        // Step 6: Render grid to Tilemap
        RenderGridToTilemap();

        // Step 7: Refresh colliders
        tilemap.RefreshAllTiles();

        // Step 8: Spawn player safely
        if (player != null)
        {
            StartCoroutine(SpawnNextFrame());
        }
    }

    private void RenderGridToTilemap()
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                if (grid[x, y])
                {
                    // Perimeter walls stay unmineable
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        tilemap.SetTile(cellPos, groundTile);
                    }
                    else
                    {
                        // Pick a rock tile from the pool
                        TileDefinition chosen = GetRandomTileDefinition();
                        tilemap.SetTile(cellPos, chosen.tileAsset);

                        // Map TileCategory → DropType
                        DropType dropType = CategoryToDropType(chosen.category);

                        // Ask DropManager for a drop of that type
                        MineableDrop drop = DropManager.Instance.GetDropForType(dropType, playerLevel);

                        // Assign durability + drop
                        TileDurabilityManager.Instance.AssignDrop(cellPos, drop, chosen);
                    }
                }
                else
                {
                    // Empty space (entrance + tunnel)
                    tilemap.SetTile(cellPos, null);
                }
            }
        }
    }

    private TileDefinition GetRandomTileDefinition()
    {
        float totalWeight = 0f;
        foreach (var def in rockPool.tileDefinitions)
            totalWeight += def.spawnChance;

        float roll = Random.value * totalWeight;

        foreach (var def in rockPool.tileDefinitions)
        {
            if (roll < def.spawnChance)
                return def;
            roll -= def.spawnChance;
        }

        return rockPool.tileDefinitions[0]; // fallback
    }

    private DropType CategoryToDropType(TileCategory category)
    {
        switch (category)
        {
            case TileCategory.OreVein: return DropType.Ore;
            case TileCategory.GemVein: return DropType.Gem;
            case TileCategory.RelicVein: return DropType.Relic;
            default: return DropType.None; // or whatever you use for "no drop"
        }
    }

    private IEnumerator SpawnNextFrame()
    {
        yield return null;
        player.position = spawnPoint;
        player.GetComponent<Rigidbody2D>().freezeRotation = true;
    }
}
