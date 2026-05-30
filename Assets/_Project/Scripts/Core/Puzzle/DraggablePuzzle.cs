using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class DraggablePuzzle : MonoBehaviour, IInteractable
{
    [Header("UI Panel Settings")]
    [SerializeField] string interactPrompt = "[E] Abrir Puzzle";
    [SerializeField] GameObject puzzlePanel;

    [Header("Puzzle Logic")]
    [Tooltip("Arrastra aquí las 6 casillas en orden (De izquierda a derecha o arriba a abajo)")]
    [SerializeField] FuseSlot[] slots = new FuseSlot[6];

    [Header("Events")]
    [SerializeField] UnityEvent OnPuzzleSolved;

    bool isSolved = false;
    bool isUIOpen = false;

    void Start()
    {
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        UserInterfaceManager.Instance.RegisterPanel(UserInterfaceManager.PanelType.Puzzle, () => TogglePanel(true));
    }

    public string GetTextInteract() => interactPrompt;
    public void Interact(Transform interactorTransform)
    {
        if (isSolved) return;
        TogglePanel(!isUIOpen);
    }

    public void TogglePanel(bool state)
    {
        isUIOpen = state;
        if (state)
        {
            if (!UserInterfaceManager.Instance.RequestOpenPanel(UserInterfaceManager.PanelType.Puzzle)) return;
        }
        else
        {
            UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Puzzle);
        }

        puzzlePanel.SetActive(state);
    }
    public void CheckWinCondition()
    {
        if (isSolved) return;

        bool slot1HasFuse = slots[0].transform.childCount > 0;
        bool slot3HasFuse = slots[2].transform.childCount > 0;
        bool slot5HasFuse = slots[4].transform.childCount > 0;
        bool slot6HasFuse = slots[5].transform.childCount > 0;

        if (slot1HasFuse && slot3HasFuse && slot5HasFuse && slot6HasFuse)
        {
            isSolved = true;
            interactPrompt = string.Empty;
            OnPuzzleSolved?.Invoke();
            TogglePanel(false);
        }
    }
}