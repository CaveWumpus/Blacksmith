using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ExitTrigger : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leavePromptPanel;   // Assign your "Leave Mining?" panel
    public GameObject yesButton;          // Assign the Yes button
    public GameObject noButton;           // Assign the No button

    [Header("Blocker Settings")]
    public Vector2 blockerSize = new Vector2(0.5f, 4f); // adjustable in Inspector
    public float inwardOffset = 0.25f;                  // how far inside cave the wall sits

    private GameObject blocker;

    void Start()
    {
        // Spawn invisible wall just inside the cave relative to trigger
        blocker = new GameObject("ExitBlocker");
        var box = blocker.AddComponent<BoxCollider2D>();
        box.isTrigger = false; // solid wall
        box.size = blockerSize;

        // Place wall slightly deeper inside cave than the trigger
        if (transform.position.x < 1f) // entrance on left
            blocker.transform.position = transform.position + new Vector3(inwardOffset, 0, 0);
        else // entrance on right
            blocker.transform.position = transform.position + new Vector3(-inwardOffset, 0, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            leavePromptPanel.SetActive(true);
            Time.timeScale = 0f;

            // Default focus for controller input
            if (yesButton != null)
                EventSystem.current.SetSelectedGameObject(yesButton);
        }
    }

    public void OnYes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("ShopScene");
    }

    public void OnNo()
    {
        leavePromptPanel.SetActive(false);
        Time.timeScale = 1f;

        // Reset focus to No button so controller can still navigate
        if (noButton != null)
            EventSystem.current.SetSelectedGameObject(noButton);
    }
}
