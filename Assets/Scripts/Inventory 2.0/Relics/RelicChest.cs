using UnityEngine;

public class RelicChest : MonoBehaviour
{
    public void AddRelicToChest(RelicDefinition relic)
    {
        RelicManager.Instance.AddOwnedRelic(relic);
        // UI update event here
    }
}
