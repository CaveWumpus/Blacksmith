using UnityEngine;

[CreateAssetMenu(fileName = "DropPool", menuName = "Mine/DropPool")]
public class DropPool : ScriptableObject
{
    public MineableDrop[] ores;
    public MineableDrop[] gems;
    public MineableDrop[] relics;
}
