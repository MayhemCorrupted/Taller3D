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

    [Header("Hint (pensamiento del personaje)")]
    [SerializeField] TextMeshProUGUI hintText;
    [SerializeField] float hintDuration = 3f;

    [Header("Eventos")]
    public UnityEvent OnPuzzleSolved;

    string currentInput = "";
    bool isSolved = false;

    void Start()
    {
        ActivateRandomNote();

        if (codeInputPanel != null) codeInputPanel.SetActive(false);
        UpdateDisplay();

    }

    void Update()
    {

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
    }

    public void OnNumberPressed(int number)
    {
    }

    public void OnDeletePressed()
    {
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
