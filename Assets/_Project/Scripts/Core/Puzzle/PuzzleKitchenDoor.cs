using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class PuzzleKitchenDoor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject keynoteItem;
    Player_Camera playerCamera;
    Player_Movement playerMovement;

    [Header("Code Config")]
    [SerializeField] private int codeLength = 4;
    [SerializeField] private string saveKey = "KitchenPuzzleCode";

    [Header("UI Panel")]
    [SerializeField] private GameObject keypadPanel;
    [SerializeField] private TextMeshProUGUI displayField;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private float hintDuration = 2.5f;

    [Header("Events")]
    public UnityEvent OnCorrectCode;

    private string correctCodeString;
    private string currentInput = "";
    private bool isSolved = false;
    private bool isUIOpen = false;

    private void Awake()
    {
        if (player != null)
        {
            playerCamera = player.GetComponent<Player_Camera>();
            playerMovement = player.GetComponent<Player_Movement>();
        }
    }
    void Start()
    {
        SetupPuzzle();
        if (keypadPanel != null) keypadPanel.SetActive(false);
        UpdateDisplay();
    }

    void SetupPuzzle()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            correctCodeString = PlayerPrefs.GetString(saveKey);
        }
        else
        {
            correctCodeString = "";
            for (int i = 0; i < codeLength; i++)
                correctCodeString += Random.Range(0, 10).ToString();
        }
    }
    public void OnPlayerInteract()
    {
        if (isSolved) return;
        ToggleKeypad(!isUIOpen);
    }
    void Update()
    {
        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
            ToggleKeypad(false);
    }
    public void ToggleKeypad(bool state)
    {
        isUIOpen = state;
        keypadPanel.SetActive(state);

        if (playerCamera != null) playerCamera.CameraMovement(state);
        if (playerMovement != null) playerMovement.SetMovement(!state);

        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;

        if (state) ResetInput();
    }

    #region OnClick_UI
    public void InputNumber(int number)
    {
        if (!isUIOpen || currentInput.Length >= codeLength) return;

        currentInput += number.ToString();
        UpdateDisplay();

        if (currentInput.Length == codeLength)
        {
            CheckCode();
        }
    }

    public void DeleteLastDigit()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput[..^1];
            UpdateDisplay();
        }
    }

    #endregion
    private void CheckCode()
    {
        if (currentInput == correctCodeString)
        {
            isSolved = true;
            OnCorrectCode?.Invoke();
            ShowMessage("*CLIC* La puerta se ha desbloqueado.");
            ToggleKeypad(false);
        }
        else
        {
            ShowMessage("Código Incorrecto...");
            StartCoroutine(WaitAndReset());
        }
    }

    private IEnumerator WaitAndReset()
    {
        WaitForSeconds resetDelay = new(0.5f);
        yield return resetDelay;
        ResetInput();
    }
    private void ResetInput()
    {
        currentInput = "";
        UpdateDisplay();
    }
    private void UpdateDisplay()
    {
        if (displayField != null)
            displayField.text = currentInput.PadRight(codeLength, '_');
    }
    private void ShowMessage(string msg)
    {
        if (hintText == null) return;
        StopAllCoroutines();
        StartCoroutine(DisplayHintRoutine(msg));
    }
    private IEnumerator DisplayHintRoutine(string msg)
    {
        hintText.text = msg;
        hintText.gameObject.SetActive(true);
        yield return new WaitForSeconds(hintDuration);
        hintText.gameObject.SetActive(false);
    }
}
