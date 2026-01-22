using UnityEngine;
using UnityEngine.UI;

public class PortraitRenderer : MonoBehaviour
{
    public Image headLayer;
    public Image eyesLayer;
    public Image noseLayer;
    public Image mouthLayer;
    public Image hairLayer;
    public Image accessoryLayer;
    public Image shirtLayer;

    public void Render(CustomerProfile profile)
    {
        headLayer.sprite = profile.headShape.sprite;
        eyesLayer.sprite = profile.eyes.sprite;
        noseLayer.sprite = profile.nose.sprite;
        mouthLayer.sprite = profile.mouth.sprite;
        hairLayer.sprite = profile.hair.sprite;
        accessoryLayer.sprite = profile.accessory.sprite;

        //headLayer.color = profile.skinColor;
        //shirtLayer.color = profile.shirtColor;
    }
}
