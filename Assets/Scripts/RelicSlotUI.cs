using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicSlotUI : MonoBehaviour
{
    public Image icon;        // drag the Icon child here
    public TMP_Text nameText; // drag the Name child here

    public void SetRelic(Sprite relicIcon, string relicName)
    {
        if (relicIcon != null)
        {
            icon.sprite = relicIcon;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }

        nameText.text = relicName;
    }

    public void OnSlotClicked()
    {
        RelicContextMenu.Instance.Open(this);
    }


    // ✅ Clear slot completely
    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
        nameText.text = "";
    }
}
