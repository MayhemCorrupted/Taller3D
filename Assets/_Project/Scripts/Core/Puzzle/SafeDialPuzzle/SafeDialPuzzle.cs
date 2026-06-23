using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SafeDialPuzzle : MonoBehaviour, IInteractable
{
    [Header("Interaction & UI")]
    [SerializeField] string interactPrompt = "[E] Inspeccionar Caja Fuerte";
    [SerializeField] GameObject uiPanel;
    [SerializeField] Button exitButton;
    [SerializeField] SafeDialUI dialController;
    [Header("Combination Settings")]
    [SerializeField] private int[] currentCombination = new int[3];
    [Header("Dial State (Read Only)")]
    [SerializeField] int currentNumber = 0;
    [SerializeField] List<int> playerInputSequence = new();
    [Header("Events")]
    [Tooltip("Para mostrar el tutorial")]
    public UnityEvent OnPuzzleOpened;
    [Tooltip("Para ocultar el tutorial")]
    public UnityEvent OnFirstInteraction;
    [Tooltip("Cuando girar el dial")]
    public UnityEvent OnDialTick;
    [Tooltip("Cuando la secuencia es correcta")]
    public UnityEvent OnStepCorrect;
    [Tooltip("Cuando se falla la secuencia")]
    public UnityEvent OnFailSequence;
    [Tooltip("Cuando se gana el puzzle")]
    public UnityEvent OnSafeUnlocked;    

    readonly int rightDirection = -1;
    readonly int leftDirection = 1;
    bool isLocked = false;
    bool isSolved = false;
    bool hasInteracted = false;
    void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);

        if (exitButton != null) exitButton.onClick.AddListener(() => UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Dial));

        if (dialController != null)
        {
            dialController.OnDialReleased.AddListener(EvaluatePlayerInput);
            dialController.OnNumberChanged.AddListener(HandleDialMovement); ;
        }
        UserInterfaceManager.Instance.RegisterPanel(
            UserInterfaceManager.PanelType.Dial,
            () => { if (uiPanel != null) uiPanel.SetActive(true); if (!hasInteracted) OnPuzzleOpened?.Invoke(); },
            () => { if (uiPanel != null) uiPanel.SetActive(false); }
        );
    }
    public string GetTextInteract() => interactPrompt;
    public void Interact(Transform interactorTransform)
    {
        if (isSolved) return;
        UserInterfaceManager.Instance.TogglePanel(UserInterfaceManager.PanelType.Dial);
    }
    void HandleDialMovement(int num)
    {
        currentNumber = num;

        if (!hasInteracted)
        {
            hasInteracted = true;
            OnFirstInteraction?.Invoke();
        }

        OnDialTick?.Invoke();
    }
    void EvaluatePlayerInput(int finalNumber)
    {
        if (isSolved || isLocked) return;

        int actualDir = dialController.GetNetDragDirection();
        int expectedDir = GetExpectedDirectionForCurrentStep();

        if (actualDir != expectedDir)
        {
            FailAndReset();
            return;
        }
        int currentIndex = playerInputSequence.Count;

        if (finalNumber != currentCombination[currentIndex])
        {
            FailAndReset();
            return;
        }

        playerInputSequence.Add(finalNumber);

        if (playerInputSequence.Count == currentCombination.Length) UnlockSafe();
        else OnStepCorrect?.Invoke();
    }
    int GetExpectedDirectionForCurrentStep()
    {
        if (playerInputSequence.Count == 0) return rightDirection;
        if (playerInputSequence.Count == 1) return leftDirection;
        return rightDirection;
    }
    void UnlockSafe()
    {
        isLocked = true;
        isSolved = true;
        interactPrompt = string.Empty;

        if (dialController != null) dialController.SetInteractable(false);

        OnSafeUnlocked?.Invoke();
        UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Dial);
    }
    public void FailAndReset()
    {
        OnFailSequence?.Invoke(); 

        playerInputSequence.Clear();
        currentNumber = 0;

        if (dialController != null) dialController.ResetVisualDial();
    }
}
