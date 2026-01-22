using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class EvacuationController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject evacuationPanel;
    public TMPro.TextMeshProUGUI headerText;
    public TMPro.TextMeshProUGUI breakdownText;

    [Header("Buttons")]
    public GameObject yesButton;
    public GameObject noButton; // Only used on confirmation screen

    [Header("Settings")]
    [Range(0, 100)] public int evacuationLossPercent = 10;

    private enum EvacuationStep { InitialPrompt, ConfirmPrompt, Forced }
    private EvacuationStep currentStep = EvacuationStep.InitialPrompt;
    

    //private PlayerControls controls;

    /*void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.UI.Submit.performed += OnSubmit;
        controls.UI.Cancel.performed += OnCancel;
        controls.UI.Enable();
    }

    void OnDisable()
    {
        controls.UI.Submit.performed -= OnSubmit;
        controls.UI.Cancel.performed -= OnCancel;
        controls.UI.Disable();
    }*/

    // Called by PauseManager when navigating right
    public void ShowInitialPrompt()
    {
        evacuationPanel.SetActive(true);
        currentStep = EvacuationStep.InitialPrompt;

        headerText.text = "Would you like to evacuate to Shop?";
        breakdownText.text = "";

        yesButton.SetActive(true);
        noButton.SetActive(false); // No button removed on first screen

        EventSystem.current.SetSelectedGameObject(yesButton);
        //PauseManager.Instance.currentMode = PauseManager.PauseMenuMode.Evacuation;

        //StartCoroutine(SetFocusNextFrame(yesButton));
    }

    // Called by MineTimer
    public void ForceEvacuation()
    {
        evacuationPanel.SetActive(true);
        currentStep = EvacuationStep.Forced;

        headerText.text = "Time expired! You must evacuate.";
        breakdownText.text = BuildEvacuationBreakdown();

        yesButton.SetActive(true);
        noButton.SetActive(false); // Cannot decline forced evacuation

        StartCoroutine(SetFocusNextFrame(yesButton));
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        if (selected == yesButton)
            OnYes();
        else if (selected == noButton)
            OnNo();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (currentStep == EvacuationStep.InitialPrompt)
        {
            Hide();
            //StartCoroutine(ResetNextFrame());
        }
        else if (currentStep == EvacuationStep.ConfirmPrompt)
        {
            // Return to initial prompt
            ShowInitialPrompt();
        }
    }


    public void OnYes()
    {
        if (currentStep == EvacuationStep.InitialPrompt)
        {
            // Move to confirmation screen
            currentStep = EvacuationStep.ConfirmPrompt;

            headerText.text = "You will lose the following resources:";
            breakdownText.text = BuildEvacuationBreakdown();

            yesButton.SetActive(true);
            noButton.SetActive(true);

            StartCoroutine(SetFocusNextFrame(yesButton));
        }
        else if (currentStep == EvacuationStep.ConfirmPrompt || currentStep == EvacuationStep.Forced)
        {
            ApplyLosses();
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("ShopScene");
        }
    }

    public void OnNo()
    {
        if (currentStep == EvacuationStep.ConfirmPrompt)
        {
            Hide();
            //StartCoroutine(ResetNextFrame());
        }
    }

    public void Hide()
    {
        evacuationPanel.SetActive(false);
        PauseManager.Instance.currentMode = PauseManager.PauseMenuMode.PanelSelect;
    }
    public void OnSubmitFromPause()
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == yesButton) OnYes();
        else if (selected == noButton) OnNo();
    }


    //private IEnumerator ResetNextFrame()
    //{
       // yield return null; // wait one frame
       // ResetToFirstPageForPanelSelect();
    //}

    private IEnumerator SetFocusNextFrame(GameObject go)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(go);
    }

    private string BuildEvacuationBreakdown()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (PlayerInventory.Instance == null)
        {
            sb.AppendLine("No inventory data available.");
            return sb.ToString();
        }

        if (PlayerInventory.Instance.oreStacks != null)
        {
            foreach (var stack in PlayerInventory.Instance.oreStacks)
            {
                int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
                sb.AppendLine($"{stack.itemName}: Lose {lost} (from {stack.count})");
            }
        }

        if (PlayerInventory.Instance.gemStacks != null)
        {
            foreach (var stack in PlayerInventory.Instance.gemStacks)
            {
                int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
                sb.AppendLine($"{stack.itemName}: Lose {lost} (from {stack.count})");
            }
        }

        return sb.ToString();
    }


    public void HandleSubmit()
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == yesButton) OnYes();
        else if (selected == noButton) OnNo();
    }

    public void HandleCancel()
    {
        Hide();

    }

    private void ApplyLosses()
    {
        foreach (var stack in PlayerInventory.Instance.oreStacks)
        {
            int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
            stack.count -= lost;
        }

        foreach (var stack in PlayerInventory.Instance.gemStacks)
        {
            int lost = Mathf.FloorToInt(stack.count * evacuationLossPercent / 100f);
            stack.count -= lost;
        }
    }
}
