using UnityEngine;
using System.Collections;

public class ChargeAuraController : MonoBehaviour
{
    [Header("Outline Renderer")]
    public SpriteRenderer outlineRenderer;

    [Header("Scales")]
    public float baseScale = 1.05f;
    public float sweetScale = 1.12f;
    public float perfectScale = 1.15f;
    public float missScale = 1.10f;

    [Header("Timings")]
    public float scaleLerpSpeed = 12f;     // how fast it smooths
    public float lingerTime = 0.15f;       // how long sweet spot stays after release

    private AuraProfile profile;

    private float currentValue;
    private bool inSweet;
    private bool inPerfect;

    private Coroutine resetRoutine;

    public void SetProfile(AuraProfile newProfile)
    {
        profile = newProfile;
        currentValue = 0f;
        inSweet = false;
        inPerfect = false;

        if (resetRoutine != null)
            StopCoroutine(resetRoutine);

        SetInstant(baseScale, profile.idleColor);
    }

    public void OnChargeStart()
    {
        currentValue = 0f;
        inSweet = false;
        inPerfect = false;

        if (resetRoutine != null)
            StopCoroutine(resetRoutine);

        SetInstant(baseScale, profile.idleColor);
    }

    public void UpdateCharge(float normalized)
    {
        currentValue = Mathf.Clamp01(normalized);
        UpdateVisuals();
    }

    public void OnChargeEnd(out bool wasSweet, out bool wasPerfect)
    {
        wasSweet = inSweet;
        wasPerfect = inPerfect;

        if (resetRoutine != null)
            StopCoroutine(resetRoutine);

        resetRoutine = StartCoroutine(LingerAndReset(wasSweet, wasPerfect));
    }

    private IEnumerator LingerAndReset(bool wasSweet, bool wasPerfect)
    {
        float targetScale = baseScale;
        Color targetColor = profile.idleColor;

        if (wasPerfect)
        {
            targetScale = perfectScale;
            targetColor = profile.perfectColor;
        }
        else if (wasSweet)
        {
            targetScale = sweetScale;
            targetColor = profile.sweetColor;
        }
        else
        {
            targetScale = missScale;
            targetColor = profile.idleColor;
        }

        // Hold the effect briefly
        SetInstant(targetScale, targetColor);
        yield return new WaitForSeconds(lingerTime);

        // Smoothly return to base
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            outlineRenderer.transform.localScale = Vector3.Lerp(
                outlineRenderer.transform.localScale,
                Vector3.one * baseScale,
                t
            );
            outlineRenderer.color = Color.Lerp(outlineRenderer.color, profile.idleColor, t);
            yield return null;
        }

        SetInstant(baseScale, profile.idleColor);
    }

    private void UpdateVisuals()
    {
        if (profile == null || outlineRenderer == null)
            return;

        float targetScale = baseScale;
        Color targetColor = profile.idleColor;

        bool nowSweet = currentValue >= profile.sweetStart &&
                        currentValue <= profile.sweetEnd;

        bool nowPerfect = profile.usePerfectWindow &&
                          currentValue >= profile.perfectStart &&
                          currentValue <= profile.perfectEnd;

        inSweet = nowSweet;
        inPerfect = nowPerfect;

        if (nowPerfect)
        {
            targetScale = perfectScale;
            targetColor = profile.perfectColor;
        }
        else if (nowSweet)
        {
            targetScale = sweetScale;
            targetColor = profile.sweetColor;
        }

        // Smooth transitions
        outlineRenderer.transform.localScale = Vector3.Lerp(
            outlineRenderer.transform.localScale,
            Vector3.one * targetScale,
            Time.deltaTime * scaleLerpSpeed
        );

        outlineRenderer.color = Color.Lerp(
            outlineRenderer.color,
            targetColor,
            Time.deltaTime * scaleLerpSpeed
        );
    }

    private void SetInstant(float scale, Color color)
    {
        outlineRenderer.transform.localScale = Vector3.one * scale;
        outlineRenderer.color = color;
    }
}
