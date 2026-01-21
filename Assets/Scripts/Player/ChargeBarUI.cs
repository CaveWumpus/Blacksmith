using UnityEngine;
using UnityEngine.UI;

public class ChargeBarUI : MonoBehaviour
{
    [Header("References")]
    public Image fillImage;

    [Header("Visual")]
    public Color emptyColor = Color.gray;
    public Color fullColor = Color.yellow;
    public float lerpSpeed = 12f;

    private float currentFill = 0f;

    /// <summary>
    /// normalizedCharge should be 0–1
    /// </summary>
    public void SetCharge(float normalizedCharge)
    {
        normalizedCharge = Mathf.Clamp01(normalizedCharge);
        currentFill = normalizedCharge;

        if (fillImage == null)
            return;

        // If using Filled Image:
        fillImage.fillAmount = currentFill;

        // Color lerp
        fillImage.color = Color.Lerp(emptyColor, fullColor, currentFill);
    }

    void Update()
    {
        // Optional smoothing if you want a lerped feel instead of instant:
        // If you prefer instant, you can remove Update() entirely.
        if (fillImage == null)
            return;

        float displayed = fillImage.fillAmount;
        float target = currentFill;

        float newFill = Mathf.Lerp(displayed, target, Time.deltaTime * lerpSpeed);
        fillImage.fillAmount = newFill;
        fillImage.color = Color.Lerp(emptyColor, fullColor, newFill);
    }
}
