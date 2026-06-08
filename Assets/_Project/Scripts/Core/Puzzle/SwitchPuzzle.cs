using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwitchPuzzle : MonoBehaviour, IInteractable
{
    [Header("Item Requirements")]
    [SerializeField] ItemData requiredItemData;
    [SerializeField] GameObject itemPlacedModel;

    [Header("Puzzle UI & Interaction")]
    [SerializeField] string interactPrompt = "[E] Inspect Panel";
    [SerializeField] GameObject uiPanel;
    [SerializeField] Toggle[] fuseButtons;
    [SerializeField] Scrollbar[] fuseVisualScrollbars;
    [SerializeField] Image[] fuseFeedbackLights;

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

    public string PuzzlePrompt => interactPrompt;

    void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
        if (itemPlacedModel != null) itemPlacedModel.SetActive(false);

        for (int i = 0; i < fuseButtons.Length; i++)
        {
            int index = i;
            fuseButtons[index].onValueChanged.AddListener((val) => OnFuseToggled(index, val));
        }
    }

    public void InitializeProceduralState(bool startAllOff)
    {
        if (fuseButtons == null || fuseButtons.Length == 0) return;

        if (startAllOff)
        {
            for (int i = 0; i < fuseButtons.Length; i++)
                fuseButtons[i].SetIsOnWithoutNotify(false);
        }
        else
        {
            for (int i = 0; i < fuseButtons.Length; i++)
                fuseButtons[i].SetIsOnWithoutNotify(true);

            if (fuseButtons.Length >= 2)
            {
                int turnedOffCount = 0;
                while (turnedOffCount < 2)
                {
                    int randomIndex = Random.Range(0, fuseButtons.Length);
                    if (fuseButtons[randomIndex].isOn)
                    {
                        fuseButtons[randomIndex].SetIsOnWithoutNotify(false);
                        turnedOffCount++;
                    }
                }
            }
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

    private void OnFuseToggled(int index, bool value)
    {
        if (!value)
        {
            fuseButtons[index].SetIsOnWithoutNotify(true);
            return;
        }

        ApplySwitchRules(index);
        SyncAllVisuals();
        CheckWinCondition();
    }

    private void ApplySwitchRules(int pressedIndex)
    {
        if (fuseButtons.Length < 4) return;

        switch (pressedIndex)
        {
            case 0:
                if (fuseButtons[2].isOn) fuseButtons[2].SetIsOnWithoutNotify(false);
                break;
            case 1:
                if (fuseButtons[0].isOn) fuseButtons[0].SetIsOnWithoutNotify(false);
                break;
            case 2:
                break;
            case 3:
                if (fuseButtons[1].isOn) fuseButtons[1].SetIsOnWithoutNotify(false);
                break;
        }
    }

    private void SyncAllVisuals()
    {
        for (int i = 0; i < fuseButtons.Length; i++)
        {
            bool state = fuseButtons[i].isOn;

            if (fuseVisualScrollbars.Length > i && fuseVisualScrollbars[i] != null)
                fuseVisualScrollbars[i].value = state ? 1f : 0f;

            if (fuseFeedbackLights.Length > i && fuseFeedbackLights[i] != null)
                fuseFeedbackLights[i].color = state ? lightOnColor : lightOffColor;
        }
    }

    private void CheckWinCondition()
    {
        foreach (Toggle t in fuseButtons)
        {
            if (!t.isOn) return;
        }

        isSolved = true;
        interactPrompt = string.Empty;
        OnPuzzleSolved?.Invoke();
        TogglePanel(false);
    }
}   