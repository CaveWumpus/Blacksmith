using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicSlotUI : MonoBehaviour
{
    public Image icon;        // drag the Icon child here
    public TMP_Text nameText; // drag the Name/Count child here

    public string relicName;
    public int count;

    // Update slot with relic data
    public void SetSlot(Sprite relicIcon, int newCount, string newName = "")
    {
        relicName = newName;
        count = newCount;

        icon.sprite = relicIcon;
        icon.enabled = relicIcon != null;
        nameText.text = count > 1 ? relicName + " x" + count : relicName;
    }

    public void OnSlotClicked()
    {
        // No longer used in controller-based navigation
    }


    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(relicName) || count <= 0;
    }

    public void ClearSlot()
    {
        Debug.Log($"[ClearSlot] {name} cleared. RelicName={relicName}, Count={count}");
        relicName = "";
        count = 0;
        icon.sprite = null;
        icon.enabled = false;
        nameText.text = "";
    }


    public void Refresh()
    {
        nameText.text = count > 1 ? relicName + " x" + count : relicName;
        icon.enabled = !string.IsNullOrEmpty(relicName);
    }
}
