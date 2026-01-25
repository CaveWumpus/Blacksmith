using UnityEngine;
using System.Collections.Generic;

public class RelicLoadoutController : MonoBehaviour
{
    public static RelicLoadoutController Instance { get; private set; }

    [SerializeField] private List<RelicDefinition> equipped = new List<RelicDefinition>(4);

    public IReadOnlyList<RelicDefinition> Equipped => equipped;

    private void Awake()
    {
        Instance = this;
    }

    public void SetRelicInSlot(int slotIndex, RelicDefinition relic)
    {
        if (slotIndex < 0 || slotIndex >= 4)
            return;

        equipped[slotIndex] = relic;
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= 4)
            return;

        equipped[slotIndex] = null;
    }
}
