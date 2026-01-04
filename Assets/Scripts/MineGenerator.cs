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
    public Tile groundTile;     // unmineable
    public Tile softRockTile;   // mineable
    public Tile hardRockTile;   // mineable

    [Header("Player")]
    public Transform player;

    private bool[,] grid;
    private Vector2 spawnPoint;
    private int startX, startY;

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
                grid[x, y] = true; // solid by default
            }
        }

        // Step 2: Carve random interior caves
        //for (int x = 1; x < width - 1; x++)
        //{
            //for (int y = 1; y < height - 1; y++)
            //{
                //if (Random.value > 0.55f)
                    //grid[x, y] = false; // empty space
            //}
        //}

        // Step 3: Enforce perimeter walls
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

        // Step 4: Carve entrance room
        bool startOnLeft = Random.value < 0.5f;
        startX = startOnLeft ? 1 : width - 5;
        startY = Random.Range(height / 3, (height * 2) / 3);

        for (int x = startX; x < startX + 4; x++)
        {
            for (int y = startY; y < startY + 4; y++)
            {
                grid[x, y] = false; // force empty
            }
        }

        // Step 5: Keep perimeter open at entrance
        if (startOnLeft)
        {
            for (int y = startY; y < startY + 4; y++)
                grid[0, y] = false; // open left edge
        }
        else
        {
            for (int y = startY; y < startY + 4; y++)
                grid[width - 1, y] = false; // open right edge
        }

        // Step 6: Guaranteed tunnel carve into interior
        int tunnelLength = 6;
        if (startOnLeft)
        {
            for (int x = 0; x < tunnelLength; x++)
            {
                for (int y = startY + 1; y < startY + 3; y++)
                {
                    grid[startX + x, y] = false; // carve horizontal tunnel
                }
            }
        }
        else
        {
            for (int x = 0; x < tunnelLength; x++)
            {
                for (int y = startY + 1; y < startY + 3; y++)
                {
                    grid[startX - x, y] = false; // carve horizontal tunnel
                }
            }
        }

        // Save spawn point in center of room
        spawnPoint = new Vector2(startX + 2.5f, startY + 2.5f);

        // Step 7: Render grid to Tilemap
        RenderGridToTilemap();

        // Step 8: Refresh colliders
        tilemap.RefreshAllTiles();

        // Step 9: Spawn player safely
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
                        // Interior walls: mostly mineable, sprinkle ground occasionally
                        float roll = Random.value;
                        if (roll < 0.1f)
                            tilemap.SetTile(cellPos, groundTile); // rare unmineable cluster
                        else if (roll < 0.6f)
                            tilemap.SetTile(cellPos, softRockTile);
                        else
                            tilemap.SetTile(cellPos, hardRockTile);
                    }
                }
                else
                {
                    // Empty space (including entrance + tunnel)
                    tilemap.SetTile(cellPos, null);
                }
            }
        }
    }

    private IEnumerator SpawnNextFrame()
    {
        yield return null; // wait one frame for colliders to rebuild
        player.position = spawnPoint;
        player.GetComponent<Rigidbody2D>().freezeRotation = true; // prevent diagonal shooting
    }
}
