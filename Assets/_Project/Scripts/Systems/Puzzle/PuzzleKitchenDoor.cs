using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PuzzleKitchenDoor : MonoBehaviour
{
    [Header("Nota con el código")]
    [SerializeField] GameObject[] notePossiblePositions;

    [Header("Código")]
    [SerializeField] int correctCode = 1234;
    [SerializeField] int codeLength = 4;

    [Header("UI Panel de código")]
    [SerializeField] GameObject codeInputPanel;
    [SerializeField] TextMeshProUGUI codeDisplayText;

    [Header("Puerta")]
    [Tooltip("Arrastrar el GameObject 'Hinge' (hijo vacío en la bisagra)")]
    [SerializeField] Transform doorTransform;
    [Tooltip("90 = abre hacia un lado, -90 = hacia el otro. Probá cual va en tu escena.")]
    [SerializeField] float openAngle = 90f;
    [SerializeField] float openDuration = 0.8f;

    [Header("Hint (pensamiento del personaje)")]
    [SerializeField] TextMeshProUGUI hintText;
    [SerializeField] float hintDuration = 3f;

    [Header("Eventos")]
    public UnityEvent OnPuzzleSolved;

    string currentInput = "";
    bool isSolved = false;
    bool uiOpen = false;
    bool isOpening = false;
    float openProgress = 0f;
    Quaternion closedRot;
    Quaternion openRot;

    void Start()
    {
        ActivateRandomNote();

        if (codeInputPanel != null) codeInputPanel.SetActive(false);
        UpdateDisplay();

        if (doorTransform != null)
        {
            closedRot = doorTransform.localRotation;
            openRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
        }
        else
        {
            Debug.LogWarning("[PuzzleA] doorTransform no asignado. Asignar el GameObject 'Hinge'.");
        }
    }

    void Update()
    {
        if (uiOpen && Input.GetKeyDown(KeyCode.Escape))
            ToggleCodeUI();

        if (!isOpening) return;

        openProgress += Time.deltaTime / openDuration;
        float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(openProgress));
        doorTransform.localRotation = Quaternion.Slerp(closedRot, openRot, t);

        if (openProgress >= 1f)
        {
            doorTransform.localRotation = openRot;
            isOpening = false;
        }
    }

    void ActivateRandomNote()
    {
        if (notePossiblePositions == null || notePossiblePositions.Length == 0)
        {
            Debug.LogWarning("[PuzzleA] No hay notas asignadas en notePossiblePositions.");
            return;
        }

        foreach (var go in notePossiblePositions)
            if (go != null) go.SetActive(false);

        int chosen = Random.Range(0, notePossiblePositions.Length);
        if (notePossiblePositions[chosen] != null)
        {
            notePossiblePositions[chosen].SetActive(true);
            Debug.Log($"[PuzzleA] Nota activa → '{notePossiblePositions[chosen].name}'");
        }
    }

    public void OnPlayerInteract()
    {
        if (isSolved) return;
        ToggleCodeUI();
    }

    void ToggleCodeUI()
    {
        uiOpen = !uiOpen;
        if (codeInputPanel != null) codeInputPanel.SetActive(uiOpen);

        var cam = FindFirstObjectByType<Player_Camera>();
        if (cam != null) cam.CameraMovement(uiOpen);

        Cursor.lockState = uiOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = uiOpen;

        if (uiOpen) ResetInput();
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
        if (!uiOpen || currentInput.Length == 0) return;
        currentInput = currentInput[..^1];
        UpdateDisplay();
    }

    void CheckCode()
    {
        if (int.Parse(currentInput) == correctCode)
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
        if (uiOpen) ToggleCodeUI();

        if (doorTransform != null)
        {
            openProgress = 0f;
            isOpening = true;
        }

        ShowHint("*clic* La puerta se abrió.");
        OnPuzzleSolved?.Invoke();
        Debug.Log("[PuzzleA] ¡Puzzle resuelto!");
    }

    void ShowHint(string msg)
    {
        if (hintText == null) { Debug.LogWarning("[PuzzleA] hintText no asignado."); return; }
        hintText.text = msg;
        hintText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideHint));
        Invoke(nameof(HideHint), hintDuration);
    }

    void HideHint()
    {
        if (hintText != null) hintText.gameObject.SetActive(false);
    }
}
