using UnityEngine;

public class RelicOverlayUI_Mining : MonoBehaviour
{
    [SerializeField] private RelicSlotUI_Mining[] slots; // size = 4

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        var equipped = RelicLoadoutController.Instance.Equipped;

        for (int i = 0; i < slots.Length; i++)
        {
            RelicDefinition relic = (i < equipped.Count) ? equipped[i] : null;
            slots[i].SetRelic(relic);
        }
    }
}

