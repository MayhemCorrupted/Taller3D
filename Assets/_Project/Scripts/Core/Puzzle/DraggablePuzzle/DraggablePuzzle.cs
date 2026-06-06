using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DraggablePuzzle : MonoBehaviour, IInteractable
{
    [Header("UI Panel Settings")]
    [SerializeField] string interactPrompt = "[E] Abrir Puzzle";
    [SerializeField] GameObject puzzlePanel;

    [Header("Item Requirements")]
    [SerializeField] ItemData requiredItemData;
    [SerializeField] GameObject itemModel;

    [Header("Puzzle Logic")]
    [Tooltip("Arrastra aquí las 6 casillas en orden (De izquierda a derecha o arriba a abajo)")]
    [SerializeField] FuseSlot[] slots = new FuseSlot[6];
    [SerializeField] Button restartButton;

    [Header("Events")]
    [SerializeField] UnityEvent OnCantInteract;
    [SerializeField] UnityEvent OnNeedItem;
    [SerializeField] UnityEvent OnPuzzleSolved;

    bool isSolved = false;
    bool isPlaced = false;
    bool isUIOpen = false;

    void Start()
    {
        if (restartButton != null) restartButton.onClick.AddListener(ResetAllFusesToHome);
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        if (itemModel != null) itemModel.SetActive(false);
        UserInterfaceManager.Instance.RegisterPanel(UserInterfaceManager.PanelType.Puzzle, () => TogglePanel(true));
    }

    public string GetTextInteract() => interactPrompt;
    public void Interact(Transform interactorTransform)
    {
        if (isSolved) return;
        if (!isPlaced && requiredItemData != null)
        {
            ItemData currentEquipped = EquipmentManager.Instance.CurrentEquippedItem;

            if (currentEquipped == requiredItemData)
            {
                isPlaced = true;

                InventoryManager.Instance.RemoveItem(requiredItemData);
                EquipmentManager.Instance.Unequip();
                //if (itemModel != null) itemModel.SetActive(true);
                return;
            }
            else
            {
                OnNeedItem?.Invoke();
                return;
            }
        }
        TogglePanel(!isUIOpen);
    }

    public void TogglePanel(bool state)
    {
        isUIOpen = state;
        if (state)
        {
            if (!UserInterfaceManager.Instance.RequestOpenPanel(UserInterfaceManager.PanelType.Puzzle)) return;
        }
        else UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Puzzle);

        puzzlePanel.SetActive(state);
    }
    public void CheckWinCondition()
    {
        if (isSolved) return;

        bool correctSequence = CheckSlotSequence(0, 3) && CheckSlotSequence(2, 2) && CheckSlotSequence(4, 1) && CheckSlotSequence(5, 4);

        if (correctSequence)
        {
            isSolved = true;
            interactPrompt = string.Empty;
            OnPuzzleSolved?.Invoke();
            TogglePanel(false);
        }
    }
    bool CheckSlotSequence(int slotIndex, int expectedFuseID)
    {
        if (slots[slotIndex].transform.childCount == 0) return false;
        FuseDraggable fuse = slots[slotIndex].transform.GetChild(0).GetComponent<FuseDraggable>();
        return fuse != null && fuse.FuseID == expectedFuseID;
    }
    public void ResetAllFusesToHome()
    {
        FuseDraggable[] allFuses = puzzlePanel.GetComponentsInChildren<FuseDraggable>(true);
        foreach (FuseDraggable fuse in allFuses) fuse.ResetToInitialPosition();
    }
}