using UnityEngine;
using UnityEngine.UI;

public enum SlotType { Backpack, Relic, Trash }

public class SlotRules : MonoBehaviour
{
    public SlotType slotType;

    [Header("Slot Colors")]
    public Color backpackColor = Color.white;
    public Color relicColor = Color.green;
    public Color trashColor = Color.black;

    private Image background;

    void Awake()
    {
        background = GetComponent<Image>();
    }

    void Start()
    {
        ApplyColor();
    }

    public void ApplyColor()
    {
        if (background == null) return;

        switch (slotType)
        {
            case SlotType.Backpack:
                background.color = backpackColor;
                break;

            case SlotType.Relic:
                background.color = relicColor;
                break;

            case SlotType.Trash:
                background.color = trashColor;
                break;
        }
    }

    public bool Accepts(ItemDefinition item)
    {
        if (slotType == SlotType.Trash)
            return true;

        if (slotType == SlotType.Relic)
            return item.isRelic;

        if (slotType == SlotType.Backpack)
            return true; // allow both relics and normal items

        return false;
    }

}
