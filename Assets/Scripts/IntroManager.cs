using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class IntroManager : MonoBehaviour
{
    [Header("Next Scene")]
    public string nextSceneName = "MineScene"; // set in Inspector

    [Header("Skip Settings")]
    public InputActionReference skipAction; // assign UI/Submit or Global/Skip

    void OnEnable()
    {
        skipAction.action.performed += OnSkip;
        skipAction.action.Enable();
    }

    void OnDisable()
    {
        skipAction.action.performed -= OnSkip;
        skipAction.action.Disable();
    }

    void OnSkip(InputAction.CallbackContext ctx)
    {
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        Debug.Log("[IntroManager] Skipping intro, loading MineScene");
        SceneManager.LoadScene(nextSceneName);
    }
}
