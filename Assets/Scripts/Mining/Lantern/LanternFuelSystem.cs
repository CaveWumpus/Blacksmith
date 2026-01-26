using UnityEngine;

public class LanternFuelSystem : MonoBehaviour
{
    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float startingFuel = 50f;

    [Header("Consumption")]
    public bool autoConsume = true; // Option C compatible
    public float minimumFuelToMaintainLevel = 0.1f;

    private float currentFuel;

    public float CurrentFuel => currentFuel;
    public float FuelPercent => currentFuel / maxFuel;

    public System.Action OnFuelEmpty; // LanternManager will subscribe

    void Awake()
    {
        currentFuel = Mathf.Clamp(startingFuel, 0, maxFuel);
    }

    void Update()
    {
        if (!autoConsume)
            return;
    }

    // Called by LanternManager every frame
    public void Consume(float amountPerSecond)
    {
        if (amountPerSecond <= 0)
            return;

        currentFuel -= amountPerSecond * Time.deltaTime;

        if (currentFuel <= 0)
        {
            currentFuel = 0;
            OnFuelEmpty?.Invoke();
        }
    }

    public bool HasFuelForLevel(float costPerSecond)
    {
        return currentFuel >= minimumFuelToMaintainLevel &&
               currentFuel >= costPerSecond * Time.deltaTime;
    }

    public void AddFuel(float amount)
    {
        currentFuel = Mathf.Clamp(currentFuel + amount, 0, maxFuel);
    }

    public void SetFuel(float amount)
    {
        currentFuel = Mathf.Clamp(amount, 0, maxFuel);
    }

    public bool TryConsumeManualRefuel(float amount)
    {
        if (currentFuel + amount > maxFuel)
            return false;

        currentFuel += amount;
        return true;
    }
}
