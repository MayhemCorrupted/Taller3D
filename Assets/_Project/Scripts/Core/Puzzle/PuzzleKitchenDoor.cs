using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class PuzzleKitchenDoor : MonoBehaviour
{
    [SerializeField] Player_Camera playerCamera;
    [Header("Nota con el código")]
    [SerializeField] GameObject[] notePossiblePositions;
    [SerializeField] TextMeshPro[] noteTextUI;

    [Header("Configuración del Código")]
    [SerializeField] int codeLength = 4;
    [SerializeField] string saveKey = "KitchenPuzzleCode";

    [Header("UI Panel de código")]
    [SerializeField] GameObject codeInputPanel;
    [SerializeField] TextMeshProUGUI codeDisplayText;

    [Header("Hint (pensamiento del personaje)")]
    [SerializeField] TextMeshProUGUI hintText;
    [SerializeField] float hintDuration = 3f;

    [Header("Eventos")]
    public UnityEvent OnPuzzleSolved;

    private string correctCode = "";
    private string currentInput = "";
    private bool isSolved = false;
    private bool uiOpen = false;

    void Start()
    {
        InitializePuzzle();
        ActivateRandomNote();
        if (codeInputPanel != null) codeInputPanel.SetActive(false);
        UpdateDisplay();
    }
    void InitializePuzzle()
    {
        if (PlayerPrefs.HasKey(saveKey)) correctCode = PlayerPrefs.GetString(saveKey);
        else
        {
            correctCode = GenerateRandomCode();
            PlayerPrefs.SetString(saveKey, correctCode);
            PlayerPrefs.Save();
        }
        foreach (var txt in noteTextUI) if (txt != null) txt.text = correctCode;
    }
    string GenerateRandomCode()
    {
        string newCode = "";
        for (int i = 0; i < codeLength; i++) newCode += Random.Range(0, 10).ToString();
        return newCode;
    }
    void ActivateRandomNote()
    {
        if (notePossiblePositions == null || notePossiblePositions.Length == 0) return;

        foreach (var go in notePossiblePositions)
            if (go != null) go.SetActive(false);

        int chosen = Random.Range(0, notePossiblePositions.Length);
        if (notePossiblePositions[chosen] != null)
            notePossiblePositions[chosen].SetActive(true);
    }
    public void OnPlayerInteract()
    {
        if (isSolved) return;
        ToggleCodeUI(!uiOpen);
    }
    void Update()
    {
        if (uiOpen && Input.GetKeyDown(KeyCode.Escape))
            ToggleCodeUI(false);
    }
    public void ToggleCodeUI(bool state)
    {
        uiOpen = state;
        if (codeInputPanel != null) codeInputPanel.SetActive(state);

        if (playerCamera != null)
            playerCamera.CameraMovement(state);

        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;

        if (state) ResetInput();
    }
    public void OnNumberPressed(int number)
    {
        if (!uiOpen || currentInput.Length >= codeLength) return;
        currentInput += number.ToString();
        UpdateDisplay();

        if (currentInput.Length == codeLength) CheckCode();
    }
    public void OnDeletePressed()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput[..^1];
            UpdateDisplay();
        }
    }
    void CheckCode()
    {
        if (currentInput == correctCode)
            SolvePuzzle();
        else
        {
            ShowHint("Ese no es el código...");
            ResetInput();
        }
    }
    void ResetInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (codeDisplayText != null)
            codeDisplayText.text = currentInput.PadRight(codeLength, '_');
    }

    void SolvePuzzle()
    {
        isSolved = true;
        ToggleCodeUI(false);

        ShowHint("*clic* La puerta se abrió.");
        OnPuzzleSolved?.Invoke();
    }

    void ShowHint(string msg)
    {
        if (hintText == null) return;
        StopAllCoroutines();
        StartCoroutine(DisplayHint(msg));
    }

    IEnumerator DisplayHint(string msg)
    {
        hintText.text = msg;
        hintText.gameObject.SetActive(true);
        yield return new WaitForSeconds(hintDuration);
        hintText.gameObject.SetActive(false);
    }
}
