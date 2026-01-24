using UnityEngine;
using UnityEngine.UI;

public enum SlotType { General, Relic, Trash }

public class SlotRules : MonoBehaviour
{
    public SlotType slotType;

    [Header("Slot Colors")]
    public Color generalColor = Color.white;
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
            case SlotType.General:
                background.color = generalColor;
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
            return true; // anything can be trashed

        if (slotType == SlotType.Relic)
            return item.isRelic; // only relics allowed

        if (slotType == SlotType.General)
            return true; // ore, gems, relics, recipes, patterns, everything

        return false;
    }
}
