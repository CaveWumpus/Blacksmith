using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class StartMenuManager : MonoBehaviour
{
    public GameObject bestRunsPanel;
    public GameObject optionsPanel;
    public GameObject newGameButton;
    public GameObject mainMenuGroup; // parent of NewGame, Continue, etc.
    
    public void NewGame()
    {
        Debug.Log("[StartMenu] New Game selected");
        SceneManager.LoadScene("IntroScene"); // or directly "MineScene" if skipping intro
    }

    public void ContinueGame()
    {
        Debug.Log("[StartMenu] Continue selected");
        // For now, just load MineScene. Later, load saved data.
        SceneManager.LoadScene("MineScene");
    }

    public void BestRuns()
    {
        bestRunsPanel.SetActive(true);
        mainMenuGroup.SetActive(false);
        EventSystem.current.SetSelectedGameObject(bestRunsPanel.transform.Find("CloseButton").gameObject);
    }

    public void Options()
    {
        Debug.Log("[StartMenu] Options selected");
        optionsPanel.SetActive(true);
        mainMenuGroup.SetActive(false);
        EventSystem.current.SetSelectedGameObject(optionsPanel.transform.Find("CloseButton").gameObject);
    }

    public void QuitGame()
    {
        Debug.Log("[StartMenu] Quit selected");
        Application.Quit();
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        mainMenuGroup.SetActive(true);
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }

}
