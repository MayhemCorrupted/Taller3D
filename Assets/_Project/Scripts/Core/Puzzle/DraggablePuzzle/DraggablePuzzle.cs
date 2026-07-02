using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public struct SlotRequirement
{
    [Tooltip("Índice de la casilla dentro del arreglo de Grid Slots (ej. de 0 a 5).")]
    public int slotIndex;
    [Tooltip("ID numérico que debe poseer el FuseDraggable insertado.")]
    public int expectedFuseID;
}
public class DraggablePuzzle : MonoBehaviour, IInteractable
{
    [Header("Item Requirements")]
    [SerializeField] ItemData requiredItemData;
    [SerializeField] GameObject itemPlacedModel;

    [Header("UI Panel Settings")]
    [SerializeField] string interactPrompt = "[{key}] Open Box";
    [SerializeField] string placeFusePrompt = "[{key}] Place Fuse";
    [SerializeField] string missingFusePrompt = "Requires Fuse";
    [SerializeField] GameObject uiPanel;
    [SerializeField] Button restartButton;
    [SerializeField] Button exitButton;

    [Header("Puzzle Logic")]
    [Tooltip("Orden de casillas (0 a 5)")]
    [SerializeField] FuseSlot[] gridSlots = new FuseSlot[6];
    [Header("Solution Configuration")]
    [Tooltip("Añade aquí las casillas que tienen un requisito de fusible obligatorio para ganar.")]
    [SerializeField] SlotRequirement[] solutionRequirements;
    [Header("Variants")]
    [Tooltip("0 = Nota de pista | 1 = Visuales de amperaje")]
    [SerializeField] GameObject[] puzzleVariants;

    [Header("Events")]
    [SerializeField] UnityEvent OnMissingItem;
    [SerializeField] UnityEvent OnNoPlacedItem;
    [SerializeField] UnityEvent OnPlacedItem;
    [SerializeField] UnityEvent OnPuzzleSolved;

    bool isSolved = false;
    bool isFusePlaced = false;
    void Start()
    {
        if (restartButton != null) restartButton.onClick.AddListener(ResetAllFusesToHome);
        if (exitButton != null) exitButton.onClick.AddListener(() => UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Draggable));
        if (uiPanel != null) uiPanel.SetActive(false);
        if (itemPlacedModel != null) itemPlacedModel.SetActive(false);
        UserInterfaceManager.Instance.RegisterPanel(
            UserInterfaceManager.PanelType.Draggable,
            () =>
            {
                if (uiPanel != null) uiPanel.SetActive(true);
            },
            () =>
            {
                if (uiPanel != null) uiPanel.SetActive(false);
            }
        );
    }
    public string GetTextInteract()
    {
        if (isSolved) return "";

        if (!isFusePlaced)
        {
            if (PlayerHasFuse())
            {
                return placeFusePrompt;
            }
            return missingFusePrompt;
        }

        return interactPrompt;

    }
    public void Interact(Transform interactorTransform)
    {
        if (isSolved) return;

        if (!isFusePlaced && requiredItemData != null)
        {
            TryPlaceFuse();
            return;
        }

        UserInterfaceManager.Instance.TogglePanel(UserInterfaceManager.PanelType.Draggable);
    }
    void TryPlaceFuse()
    {
        if (EquipmentManager.Instance.CurrentEquippedItem == requiredItemData)
        {
            isFusePlaced = true;
            if (itemPlacedModel != null) itemPlacedModel.SetActive(true);

            InventoryManager.Instance.RemoveItem(requiredItemData);
            EquipmentManager.Instance.Unequip();
            OnPlacedItem?.Invoke(); 
        }
        else
        {
            bool hasItemInInventory = false;
            ItemData[] currentItems = InventoryManager.Instance.GetAllItems();

            for (int i = 0; i < currentItems.Length; i++)
            {
                if (currentItems[i] == requiredItemData)
                {
                    hasItemInInventory = true;
                    break;
                }
            }

            if (hasItemInInventory) OnNoPlacedItem?.Invoke();
            else OnMissingItem?.Invoke(); 
        }
    }
    private bool PlayerHasFuse()
    {
        if (requiredItemData == null) return false;

        if (EquipmentManager.Instance != null && EquipmentManager.Instance.CurrentEquippedItem == requiredItemData)
        {
            return true;
        }

        if (InventoryManager.Instance != null)
        {
            ItemData[] currentItems = InventoryManager.Instance.GetAllItems();
            for (int i = 0; i < currentItems.Length; i++)
            {
                if (currentItems[i] == requiredItemData) return true;
            }
        }

        return false;
    }
    public void CheckWinCondition()
    {
        if (isSolved) return;

        if (solutionRequirements == null || solutionRequirements.Length == 0) return;

        bool isCorrect = true;

        foreach (SlotRequirement req in solutionRequirements)
        {
            if (!CheckSlotSequence(req.slotIndex, req.expectedFuseID))
            {
                isCorrect = false;
                break; 
            }
        }

        if (isCorrect)
        {
            isSolved = true;
            interactPrompt = string.Empty;
            OnPuzzleSolved?.Invoke();
            UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Draggable);
        }
    }
    bool CheckSlotSequence(int slotIndex, int expectedFuseID)
    {
        if (slotIndex < 0 || slotIndex >= gridSlots.Length) return false;

        FuseDraggable fuse = gridSlots[slotIndex].GetComponentInChildren<FuseDraggable>();

        return fuse != null && fuse.FuseID == expectedFuseID;
    }

    public void ResetAllFusesToHome()
    {
        if (uiPanel == null) return;

        FuseDraggable[] allFuses = uiPanel.GetComponentsInChildren<FuseDraggable>(true);
        foreach (FuseDraggable fuse in allFuses)
        {
            fuse.ResetToInitialPosition();
        }
    }
    public void SetActiveVariant(int variantIndex)
    {
        if (puzzleVariants == null || puzzleVariants.Length == 0) return;

        for (int i = 0; i < puzzleVariants.Length; i++)
        {
            if (puzzleVariants[i] != null)
            {
                puzzleVariants[i].SetActive(i == variantIndex);
            }
        }
    }
}