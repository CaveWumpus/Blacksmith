using UnityEngine;
using TMPro; // for TextMeshProUGUI

public class MineTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float mineDuration = 600f; // 10 minutes default, adjustable in Inspector

    [Header("UI References")]
    public TextMeshProUGUI timerText; // drag a TMP text object in upper right corner

    private float remainingTime;
    private bool timerRunning = false;

    void Start()
    {
        remainingTime = mineDuration;
        timerRunning = true;
    }

    void Update()
    {
        if (!timerRunning) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime < 0)
        {
            remainingTime = 0;
            timerRunning = false;
            OnTimerExpired();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void OnTimerExpired()
    {
        // 🔹 Call into PauseManager to force evacuation
        PauseManager.Instance.ForceEvacuationOnTimer();
    }
}
