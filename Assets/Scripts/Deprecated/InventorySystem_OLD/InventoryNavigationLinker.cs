using UnityEngine;
using UnityEngine.UI;

public class InventoryNavigationLinker : MonoBehaviour
{
    public InventoryUI backpackUI;
    public InventoryUI relicUI;
    public InventoryUI trashUI;

    void Start()
    {
        LinkBackpackToRelics();
        LinkBackpackToTrash();
    }

    void LinkBackpackToRelics()
    {
        // Last row of backpack → first row of relics
        int cols = backpackUI.columns;

        for (int col = 0; col < cols; col++)
        {
            int backpackIndex = backpackUI.UISlots.Count - cols + col;
            if (backpackIndex < 0 || backpackIndex >= backpackUI.UISlots.Count) continue;

            var backpackButton = backpackUI.UISlots[backpackIndex].GetComponent<Button>();
            var relicButton = relicUI.UISlots[col].GetComponent<Button>();

            var nav = backpackButton.navigation;
            nav.selectOnDown = relicButton;
            backpackButton.navigation = nav;

            // Optional reverse link
            var nav2 = relicButton.navigation;
            nav2.selectOnUp = backpackButton;
            relicButton.navigation = nav2;
        }
    }

    void LinkBackpackToTrash()
    {
        // Example: leftmost backpack slot → trash slot
        var firstBackpackButton = backpackUI.UISlots[0].GetComponent<Button>();
        var trashButton = trashUI.UISlots[0].GetComponent<Button>();

        var nav = firstBackpackButton.navigation;
        nav.selectOnLeft = trashButton;
        firstBackpackButton.navigation = nav;

        // Optional reverse link
        var nav2 = trashButton.navigation;
        nav2.selectOnRight = firstBackpackButton;
        trashButton.navigation = nav2;
    }
}
