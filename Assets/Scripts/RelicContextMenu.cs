using UnityEngine;

public class RelicContextMenu : MonoBehaviour
{
    public static RelicContextMenu Instance;
    private RelicSlotUI currentSlot;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Open(RelicSlotUI slot)
    {
        currentSlot = slot;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        currentSlot = null;
    }

    public void OnDelete()
    {
        RelicInventory.Instance.RemoveRelic(currentSlot.nameText.text);
        Close();
    }
}
