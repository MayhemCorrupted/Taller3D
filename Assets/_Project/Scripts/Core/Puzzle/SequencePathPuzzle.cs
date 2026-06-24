using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class SequencePathPuzzle : MonoBehaviour, IInteractable
{
    [Header("UI Panel & Interaction")]
    [SerializeField] GameObject uiPanel;
    [SerializeField] Button exitButton;
    [SerializeField] string interactPrompt = "[{key}] Use panel";

    [Header("Puzzle Components")]
    [SerializeField] Button[] numpadButtons;
    [SerializeField] Image[] sequenceLights;
    [SerializeField] Sprite[] sequenceIcons;

    [Header("Feedback Colors")]
    [SerializeField] Color lightOffColor = Color.black;
    [SerializeField] Color lightCorrectColor = Color.green;
    [SerializeField] Color lightErrorColor = Color.red;

    [Header("Events")]
    [SerializeField] UnityEvent OnSolved;

    int[] proceduralButtonValues;
    int expectedNextValue = 0;
    int currentSequenceStep = 0;
    bool isLockedOut = false;
    bool isSolved = false;
    public string PuzzlePrompt => interactPrompt;
   void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
        if (exitButton != null) exitButton.onClick.AddListener(() => UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Sequence));

        UserInterfaceManager.Instance.RegisterPanel(
            UserInterfaceManager.PanelType.Sequence,
            () =>
            {
                if (uiPanel != null) uiPanel.SetActive(true);
            },
            () =>
            {
                if (uiPanel != null) uiPanel.SetActive(false);
            }
        );

        InitializeProceduralGrid();
        AssignButtonListeners();
        ResetLights();
    }

    private void InitializeProceduralGrid()
    {
        Random.InitState(ProceduralSeedGenerator.Instance.OfficeSeed + gameObject.name.GetHashCode());

        int totalButtons = numpadButtons.Length;
        proceduralButtonValues = new int[totalButtons];

        for (int i = 0; i < totalButtons; i++)
        {
            proceduralButtonValues[i] = i + 1;
        }

        for (int i = 0; i < totalButtons; i++)
        {
            int randomIndex = Random.Range(i, totalButtons);
            (proceduralButtonValues[randomIndex], proceduralButtonValues[i]) = (proceduralButtonValues[i], proceduralButtonValues[randomIndex]);
        }

        for (int i = 0; i < totalButtons; i++)
        {
            if (numpadButtons[i] != null)
            {
                if (numpadButtons[i].transform.childCount > 0)
                {
                    if (numpadButtons[i].transform.GetChild(0).TryGetComponent<Image>(out var iconImage))
                    {
                        int spriteIndex = proceduralButtonValues[i] - 1; 
                        
                        if (spriteIndex >= 0 && spriteIndex < sequenceIcons.Length)
                        {
                            iconImage.sprite = sequenceIcons[spriteIndex];
                        }
                    }
                }
            }
        }
    }

    private void AssignButtonListeners()
    {
        for (int i = 0; i < numpadButtons.Length; i++)
        {
            int buttonIndex = i;
            if (numpadButtons[i] != null)
            {
                numpadButtons[i].onClick.AddListener(() => OnButtonPressed(buttonIndex));
            }
        }
    }

    public string GetTextInteract() => interactPrompt;

    public void Interact(Transform interactorTransform)
    {
        if (isSolved) return;
        UserInterfaceManager.Instance.TogglePanel(UserInterfaceManager.PanelType.Sequence);
    }

    private void OnButtonPressed(int buttonIndex)
    {
        if (isLockedOut || isSolved) return;

        int pressedValue = proceduralButtonValues[buttonIndex];

        if (currentSequenceStep == 0 || pressedValue == expectedNextValue)
        {
            ProcessCorrectPress(buttonIndex, pressedValue);
        }
        else
        {
            ProcessError();
        }
    }
    private void ProcessCorrectPress(int buttonIndex, int pressedValue)
    {
        numpadButtons[buttonIndex].interactable = false;

        if (currentSequenceStep < sequenceLights.Length && sequenceLights[currentSequenceStep] != null)
        {
            sequenceLights[currentSequenceStep].color = lightCorrectColor;
        }

        expectedNextValue = (pressedValue % numpadButtons.Length) + 1;
        currentSequenceStep++;

        if (currentSequenceStep >= numpadButtons.Length)
        {
            CompletePuzzle();
        }
    }

    private void ProcessError()
    {
        StartCoroutine(ErrorResetRoutine());
    }

    IEnumerator ErrorResetRoutine()
    {
        isLockedOut = true;

        if (currentSequenceStep < sequenceLights.Length && sequenceLights[currentSequenceStep] != null)
        {
            sequenceLights[currentSequenceStep].color = lightErrorColor;
        }

        float waitForSeconds = 0.5f;
        yield return new WaitForSeconds(waitForSeconds);

        currentSequenceStep = 0;
        expectedNextValue = 0;
        
        ResetLights();

        foreach (Button btn in numpadButtons)
        {
            if (btn != null) btn.interactable = true;
        }

        isLockedOut = false;
    }

    private void ResetLights()
    {
        foreach (Image light in sequenceLights)
        {
            if (light != null) light.color = lightOffColor;
        }
    }

    private void CompletePuzzle()
    {
        isSolved = true;
        interactPrompt = string.Empty;
        Debug.Log("[Puzzle Caja Fuerte] Secuencia completada con éxito.");
        OnSolved?.Invoke();
     UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Sequence);
    }
}
