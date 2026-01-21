using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ComboCounterUI : MonoBehaviour
{
    [Header("References")]
    public ComboMiningModule comboModule;
    public Image comboIcon;
    public TMP_Text comboText;

    [Header("Visual Settings")]
    public Color baseColor = Color.white;
    public Color maxColor = Color.red;
    public float maxScale = 1.4f;
    public float scaleLerpSpeed = 8f;
    public float colorLerpSpeed = 6f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = comboIcon.transform.localScale;
    }

    void Update()
    {
        if (comboModule == null || !comboModule.comboEnabled)
        {
            comboText.text = "";
            comboIcon.transform.localScale = originalScale;
            comboIcon.color = baseColor;
            return;
        }

        if (!comboModule.inCombo)
        {
            comboText.text = "";
            comboIcon.transform.localScale = originalScale;
            comboIcon.color = baseColor;
            return;
        }

        // Update text
        comboText.text = $"Combo x{comboModule.currentComboCount}";

        // Scale based on combo progress
        float t = (float)comboModule.currentComboCount / comboModule.maxComboCount;
        float targetScale = Mathf.Lerp(1f, maxScale, t);

        comboIcon.transform.localScale = Vector3.Lerp(
            comboIcon.transform.localScale,
            originalScale * targetScale,
            Time.deltaTime * scaleLerpSpeed
        );

        // Color shift
        comboIcon.color = Color.Lerp(baseColor, maxColor, t);
    }

    // Called by ComboMiningModule on each successful hit
    public void Pulse()
    {
        StopAllCoroutines();
        StartCoroutine(PulseRoutine());
    }

    private System.Collections.IEnumerator PulseRoutine()
    {
        comboIcon.transform.localScale = originalScale * maxScale;
        yield return new WaitForSeconds(0.08f);
    }
}
