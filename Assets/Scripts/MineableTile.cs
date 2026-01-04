using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewMineableTile", menuName = "Tiles/MineableTile")]
public class MineableTile : Tile   // <-- inherits from Tile, not MonoBehaviour
{
    public int durability = 3;        // hits required to break
    public GameObject dropPrefab;     // optional: item to spawn when broken
}
