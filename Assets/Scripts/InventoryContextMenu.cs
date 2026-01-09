using UnityEngine;

public class InventoryContextMenu : MonoBehaviour
{
    public static InventoryContextMenu Instance;
    private InventorySlotUI currentSlot;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // hidden by default
    }

    public void Open(InventorySlotUI slot)
    {
        currentSlot = slot;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        currentSlot = null;
    }

    public void OnMove()
    {
        Debug.Log($"Move {currentSlot.itemName}");
        PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.MovePending;
        PauseManager.Instance.moveSourceSlot = currentSlot;
        Close();
    }

    public void OnSplit()
    {
        int half = currentSlot.count / 2;
        if (half > 0 && PlayerInventory.Instance.GetEmptySlotIndex() != -1)
        {
            PlayerInventory.Instance.AddItem(new MineableDrop { dropName = currentSlot.itemName, icon = currentSlot.icon.sprite }, half);
            PlayerInventory.Instance.RemoveItem(currentSlot.itemName, half);
        }
        else
        {
            Debug.Log("No empty slot available to split.");
        }
        Close();
        PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.Navigation;
    }

    public void OnDelete()
    {
        PlayerInventory.Instance.RemoveItem(currentSlot.itemName, currentSlot.count);
        Close();
        PauseManager.Instance.inventoryMode = PauseManager.InventoryMode.Navigation;
    }
}
