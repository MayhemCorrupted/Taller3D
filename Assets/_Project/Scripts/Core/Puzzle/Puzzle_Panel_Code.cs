using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Puzzle_PanelCode : MonoBehaviour
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
    Player_Camera playerCamera;
    Player_Movement playerMovement;

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

    string currentInput = "";
    bool isSolved = false;
    bool isUIOpen = false;
    WaitForSeconds resetDelay;

    public string TextPrompt => interactPrompt;
    public bool IsUIOpen => isUIOpen;
    public string CorrectCodeString { get; private set; } = "";

    void Awake()
    {
        resetDelay = new WaitForSeconds(0.6f);

        if (player != null)
        {
            playerCamera = player.GetComponent<Player_Camera>();
            playerMovement = player.GetComponent<Player_Movement>();
        }

        SetupButtonListeners();
    }

    void Start()
    {
        SetupPuzzle();
        if (keypadPanel != null) keypadPanel.SetActive(false);
        ResetLights();
    }

    void SetupButtonListeners()
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

    void SetupPuzzle()
    {
        if (string.IsNullOrEmpty(CorrectCodeString) || CorrectCodeString.Length != codeLength)
        {
            if (!string.IsNullOrEmpty(customCode) && customCode.Length == codeLength)
            {
                CorrectCodeString = customCode;
            }
            else
            {
                CorrectCodeString = "";
                for (int i = 0; i < codeLength; i++)
                {
                    CorrectCodeString += Random.Range(0, 10).ToString();
                }
            }
        }

        if (linkedNote != null && linkedNote.isPuzzleNote)
        {
            linkedNote.generatedCode = CorrectCodeString;
        }
    }

    public void Interact()
    {
        if (isSolved) return;
        ToggleKeypad(!isUIOpen);
    }

    public void ToggleKeypad(bool state)
    {
        isUIOpen = state;
        keypadPanel.SetActive(state);

        if (playerCamera != null) playerCamera.LockCamera(state);
        if (playerMovement != null) playerMovement.CanMove(!state);

        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;

        if (state) ResetInput();
    }

    public void InputNumber(int number)
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

    public void DeleteLastDigit()
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
            panelCode.text = currentInput.PadRight(codeLength, '0');
        }
    }
    void UpdateLights()
    {
        for (int i = 0; i < indicatorLights.Length; i++)
        {
            if (indicatorLights[i] == null) continue;

            if (i < currentInput.Length)
            {
                indicatorLights[i].color = pressedColor;
            }
            else
            {
                indicatorLights[i].color = defaultColor;
            }
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