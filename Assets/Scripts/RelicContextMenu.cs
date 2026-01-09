using UnityEngine;

public class RelicContextMenu : MonoBehaviour
{
    public static RelicContextMenu Instance;
    private RelicSlotUI currentSlot;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // hidden by default
    }

    public void Open(RelicSlotUI slot)
    {
        currentSlot = slot;
        gameObject.SetActive(true);

        // Switch PauseManager into ContextMenu mode
        PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.ContextMenu;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        currentSlot = null;
    }

    // Called when player chooses "Move"
    public void OnMove()
    {
        Debug.Log($"Move relic {currentSlot.relicName}");
        PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.MovePending;
        PauseManager.Instance.moveSourceRelicSlot = currentSlot;
        Close();
    }

    // Called when player chooses "Delete"
    public void OnDelete()
    {
        Debug.Log($"Delete relic {currentSlot.relicName}");
        RelicInventory.Instance.RemoveRelic(currentSlot.relicName);
        Close();
        PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.Navigation;
    }
}
