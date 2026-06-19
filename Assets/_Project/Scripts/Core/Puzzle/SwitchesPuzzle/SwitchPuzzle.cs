using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwitchPuzzle : MonoBehaviour, IInteractable
{
    private const int MAX_SWITCHES = 6;

    [Header("Item Requirements")]
    [SerializeField] ItemData requiredItemData;
    [SerializeField] GameObject itemPlacedModel;

    [Header("Puzzle UI & Interaction")]
    [SerializeField] string interactPrompt = "[E] Inspect Panel";
    [SerializeField] GameObject uiPanel;

    [Tooltip("Utilizar Buttons normales para evitar el desajuste visual de Toggles.")]
    [SerializeField] Button[] fuseButtons = new Button[MAX_SWITCHES];
    [SerializeField] Scrollbar[] fuseVisualScrollbars = new Scrollbar[MAX_SWITCHES];
    [SerializeField] Image[] fuseFeedbackLights = new Image[MAX_SWITCHES];

    [Header("Color Settings")]
    [SerializeField] Color lightOnColor = Color.green;
    [SerializeField] Color lightOffColor = Color.red;

    [Header("Events")]
    [SerializeField] UnityEvent OnMissingItem;
    [SerializeField] UnityEvent OnPuzzleSolved;
    [SerializeField] UnityEvent OnNoPlacedItem;

    bool isFusePlaced = false;
    bool isSolved = false;
    bool isUIOpen = false;

    readonly bool[] currentSwitchStates = new bool[MAX_SWITCHES];

    ISwitchVariantLogic activePuzzleLogic;
    PuzzleVariant[] variantRegistry;

    public string PuzzlePrompt => interactPrompt;

    void Awake()
    {
        variantRegistry = new PuzzleVariant[]
        {
            new(0, new Variant1Logic()),
            new(1, new Variant2Logic())
        };
    }

    void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
        if (itemPlacedModel != null) itemPlacedModel.SetActive(false);

        for (int i = 0; i < fuseButtons.Length; i++)
        {
            int index = i;
            if (fuseButtons[index] != null)
            {
                fuseButtons[index].onClick.AddListener(() => OnFusePressed(index));
            }
        }
    }

    public void InitializeProceduralState(int proceduralSeed)
    {
        if (fuseButtons == null || fuseButtons.Length == 0) return;

        int variantIndex = proceduralSeed % variantRegistry.Length;
        activePuzzleLogic = variantRegistry[variantIndex].logicInstance;

        Debug.Log($"[Procedural] Switch Puzzle | Lógica Elegida: {variantRegistry[variantIndex].variantID}");
        for (int i = 0; i < currentSwitchStates.Length; i++)
        {
            currentSwitchStates[i] = false;
        }

        SyncAllVisuals();
    }

    public string GetTextInteract() => interactPrompt;

    public void Interact(Transform interactorTransform)
    {
        if (isSolved) return;

        if (!isFusePlaced)
        {
            TryPlaceFuse();
            return;
        }

        TogglePanel(!isUIOpen);
    }

    private void TryPlaceFuse()
    {
        if (EquipmentManager.Instance.CurrentEquippedItem == requiredItemData)
        {
            isFusePlaced = true;
            if (itemPlacedModel != null) itemPlacedModel.SetActive(true);

            InventoryManager.Instance.RemoveItem(requiredItemData);
            EquipmentManager.Instance.Unequip();
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

        if (uiPanel != null) uiPanel.SetActive(state);
    }

    private void OnFusePressed(int index)
    {
        if (activePuzzleLogic == null || isSolved) return;

        activePuzzleLogic.ProcessSwitch(index, currentSwitchStates);

        SyncAllVisuals();
        CheckWinCondition();
    }

    private void SyncAllVisuals()
    {
        for (int i = 0; i < MAX_SWITCHES; i++)
        {
            bool state = currentSwitchStates[i];

            if (fuseVisualScrollbars.Length > i && fuseVisualScrollbars[i] != null) fuseVisualScrollbars[i].value = state ? 1f : 0f;

            if (fuseFeedbackLights.Length > i && fuseFeedbackLights[i] != null) fuseFeedbackLights[i].color = state ? lightOnColor : lightOffColor;
        }
    }
    private void CheckWinCondition()
    {
        for (int i = 0; i < MAX_SWITCHES; i++)
        {
            if (!currentSwitchStates[i]) return;
        }

        isSolved = true;
        interactPrompt = string.Empty;
        OnPuzzleSolved?.Invoke();
        TogglePanel(false);
    }
}   