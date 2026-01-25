using UnityEngine;
using UnityEngine.UI;

public class RelicSlotUI_Mining : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    // Optional: faded color for empty slots
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.25f);

    public void SetRelic(RelicDefinition relic)
    {
        if (relic == null)
        {
            iconImage.sprite = null;
            iconImage.color = emptyColor;
        }
        else
        {
            iconImage.sprite = relic.icon;
            iconImage.color = Color.white;
        }
    }
}
