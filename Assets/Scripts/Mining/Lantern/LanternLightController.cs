using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanternLightController : MonoBehaviour
{
    [Header("References")]
    public Light2D lanternLight;

    [Header("Transition Settings")]
    public float transitionSpeed = 5f; // how fast radius/intensity lerp

    [Header("Optional Flicker")]
    public bool enableFlicker = false;
    public float flickerAmount = 0.1f;
    public float flickerSpeed = 20f;

    private float targetRadius;
    private float targetIntensity;

    private float baseRadius;
    private float baseIntensity;

    void Awake()
    {
        if (lanternLight == null)
            lanternLight = GetComponentInChildren<Light2D>();

        baseRadius = lanternLight.pointLightOuterRadius;
        baseIntensity = lanternLight.intensity;

        targetRadius = baseRadius;
        targetIntensity = baseIntensity;
    }

    void Update()
    {
        // Smoothly interpolate toward target values
        lanternLight.pointLightOuterRadius =
            Mathf.Lerp(lanternLight.pointLightOuterRadius, targetRadius, Time.deltaTime * transitionSpeed);

        lanternLight.intensity =
            Mathf.Lerp(lanternLight.intensity, targetIntensity, Time.deltaTime * transitionSpeed);

        // Optional flicker
        if (enableFlicker)
        {
            float flicker = Mathf.Sin(Time.time * flickerSpeed) * flickerAmount;
            lanternLight.intensity += flicker;
        }
    }

    // Called by LanternManager
    public void ApplyLevel(float radius, float intensity)
    {
        targetRadius = radius;
        targetIntensity = intensity;
    }

    // Hazards can call this
    public void DimLight(float amount)
    {
        targetRadius = Mathf.Max(0.5f, targetRadius - amount);
        targetIntensity = Mathf.Max(0.1f, targetIntensity - amount * 0.5f);
    }

    public void ForceLevel(float radius, float intensity)
    {
        lanternLight.pointLightOuterRadius = radius;
        lanternLight.intensity = intensity;

        targetRadius = radius;
        targetIntensity = intensity;
    }

    public void Flicker(float duration)
    {
        // Manager will toggle enableFlicker for a short time
    }
}
