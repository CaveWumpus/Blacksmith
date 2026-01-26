using UnityEngine;

public class MiningSceneController : MonoBehaviour
{
    private void Start()
    {
        if (RelicManager.Instance != null)
            RelicManager.Instance.LockRelics();
        else
            Debug.LogWarning("RelicManager.Instance is null in MiningSceneController.Start");
    }

    private void Update()
    {
        if (RelicManager.Instance != null)
            RelicManager.Instance.TickRelicTimer(Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (RelicManager.Instance != null)
            RelicManager.Instance.UnlockRelics();
    }
}

