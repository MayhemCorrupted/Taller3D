using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PanelCodePuzzle : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class PuzzleSaveData
    {
        public string savedCode;
        public bool isSolved;
    }

    const int NUMPAD = 10;

    [Header("References")]
    [SerializeField] GameObject player;
    PlayerCamera playerCamera;
    PlayerMovement playerMovement;

    [Header("Code Config")]
    [SerializeField] int codeLength = 4;
    [SerializeField] string customCode = "";
    [SerializeField] TMP_Text panelCode;
    [SerializeField] NoteData linkedNote;

    [Header("UI Panel & Buttons")]
    [SerializeField] string interactPrompt = "[E] Abrir panel";
    [SerializeField] GameObject keypadPanel;
    [SerializeField] Button[] numberButtons = new Button[NUMPAD];
    [SerializeField] Button deleteButton;
    [SerializeField] Button exitButton;

    [Header("Feedback Visual (Lights)")]
    [SerializeField] Image[] indicatorLights;
    [SerializeField] Color defaultColor = Color.grey;
    [SerializeField] Color pressedColor = Color.white;
    [SerializeField] Color correctColor = Color.green;
    [SerializeField] Color incorrectColor = Color.red;

    [Header("Events")]
    [SerializeField] UnityEvent OnCorrectCode;
    bool canUsePanel = true;
    string currentInput = "";
    bool isSolved = false;
    bool isUIOpen = false;
    WaitForSeconds resetDelay;

    public bool CanUsePanel { set { canUsePanel = value; } }
    public string TextPrompt => interactPrompt;
    public bool IsUIOpen => isUIOpen;
    public string CorrectCodeString { get; private set; } = "";

    void Awake()
    {
        resetDelay = new WaitForSeconds(0.6f);

        if (player != null)
        {
            playerCamera = player.GetComponent<PlayerCamera>();
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    void Start()
    {
        SetupPuzzle();
        if (keypadPanel != null) keypadPanel.SetActive(false);
        UserInterfaceManager.Instance.RegisterPanel(UserInterfaceManager.PanelType.Puzzle, () => ToggleKeypad(true));
        ResetLights();
    }

    public string GetTextInteract() => interactPrompt;

    public void Interact(Transform interactorTransform)
    {
        UsePanel();
    }
    void AssignButtonListeners()
    {
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int index = i;
            if (numberButtons[i] != null)
            {
                numberButtons[i].onClick.RemoveAllListeners();
                numberButtons[i].onClick.AddListener(() => InputNumber(index));
            }
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(DeleteLastDigit);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() => ToggleKeypad(false));
        }
    }

    void RemoveButtonListeners()
    {
        for (int i = 0; i < numberButtons.Length; i++)
        {
            if (numberButtons[i] != null) numberButtons[i].onClick.RemoveAllListeners();
        }

        if (deleteButton != null) deleteButton.onClick.RemoveAllListeners();
        if (exitButton != null) exitButton.onClick.RemoveAllListeners();
    }


    void SetupPuzzle()
    {
        if (string.IsNullOrEmpty(CorrectCodeString) || CorrectCodeString.Length != codeLength)
        {
            if (!string.IsNullOrEmpty(customCode) && customCode.Length == codeLength) CorrectCodeString = customCode;
            else
            {
                CorrectCodeString = "";
                for (int i = 0; i < codeLength; i++) CorrectCodeString += Random.Range(0, 10).ToString();
            }
        }

        if (linkedNote != null && linkedNote.isPuzzleNote) linkedNote.generatedCode = CorrectCodeString;
    }

    void UsePanel()
    {
        if (isSolved || !canUsePanel) return;
        ToggleKeypad(!isUIOpen);
    }

    void ToggleKeypad(bool state)
    {
        isUIOpen = state;

        if (state)
        {
            if (!UserInterfaceManager.Instance.RequestOpenPanel(UserInterfaceManager.PanelType.Puzzle)) return;
            AssignButtonListeners();
            ResetInput();
        }
        else
        {
            UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Puzzle);
            RemoveButtonListeners();
        }

        if (keypadPanel != null) keypadPanel.SetActive(state);
    }
    void InputNumber(int number)
    {
        if (!isUIOpen || isSolved || currentInput.Length >= codeLength) return;

        currentInput += number.ToString();
        UpdateDisplay();
        UpdateLights();

        if (currentInput.Length == codeLength)
        {
            StartCoroutine(CheckCodeRoutine());
        }
    }
    void DeleteLastDigit()
    {
        if (isSolved || currentInput.Length == 0) return;

        currentInput = currentInput[..^1];
        UpdateDisplay();
        UpdateLights();
    }
    void UpdateDisplay()
    {
        if (panelCode != null)
        {
            panelCode.text = currentInput.PadRight(codeLength, 'o');
        }
    }
    void UpdateLights()
    {
        for (int i = 0; i < indicatorLights.Length; i++)
        {
            if (indicatorLights[i] == null) continue;

            if (i < currentInput.Length) indicatorLights[i].color = pressedColor;
            else indicatorLights[i].color = defaultColor;
        }
    }
    IEnumerator CheckCodeRoutine()
    {
        bool isCodeCorrect = (currentInput == CorrectCodeString);
        bool hasRequiredNote = true;

        if (linkedNote != null && NotesManager.Instance != null)
        {
            hasRequiredNote = NotesManager.Instance.GetCollectedNotes().Contains(linkedNote);
        }

        bool codeMatch = isCodeCorrect && hasRequiredNote;

        Color targetColor = codeMatch ? correctColor : incorrectColor;
        SetAllLightsColor(targetColor);

        yield return resetDelay;

        if (codeMatch)
        {
            isSolved = true;
            OnCorrectCode?.Invoke();
            interactPrompt = string.Empty;
            ToggleKeypad(false);
        }
        else
        {
            ResetInput();
        }
    }

    void SetAllLightsColor(Color color)
    {
        for (int i = 0; i < indicatorLights.Length; i++)
        {
            if (indicatorLights[i] != null) indicatorLights[i].color = color;
        }
    }
    void ResetInput()
    {
        currentInput = "";
        UpdateDisplay();
        ResetLights();
    }
    void ResetLights()
    {
        SetAllLightsColor(defaultColor);
    }
    public PuzzleSaveData SavePuzzleState()
    {
        PuzzleSaveData data = new()
        {
            savedCode = this.CorrectCodeString,
            isSolved = this.isSolved
        };
        return data;
    }
    public void LoadPuzzleState(PuzzleSaveData loadedData)
    {
        if (loadedData == null) return;

        this.CorrectCodeString = loadedData.savedCode;
        this.isSolved = loadedData.isSolved;

        SetupPuzzle();

        if (isSolved)
        {
            ResetLights();
            if (keypadPanel != null) keypadPanel.SetActive(false);
        }
    }
}