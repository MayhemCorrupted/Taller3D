using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Puzzle_Switch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject itemInWorld;
    Item itemScript;
    Player_Movement playerMove;
    Player_Camera playerCam;

    [Header("Item Requirements")]
    [SerializeField] ItemData requiredItemData;
    [SerializeField] GameObject itemGameObject;

    [Header("Puzzle Configs")]
    [SerializeField] string puzzlePrompt = "[E] Empezar Puzzle";
    [SerializeField] GameObject puzzlePanel;
    [SerializeField] Toggle[] fuseButtons;
    [SerializeField] Scrollbar[] visualFuses;
    [SerializeField] Image[] feedbackLights;
    [SerializeField] Color lightColorOn = Color.green;
    [SerializeField] Color lightColorOff = Color.red;

    [Header("Events")]
    [SerializeField] UnityEvent OnCantInteract;
    [SerializeField] UnityEvent OnNeedItem;
    [SerializeField] UnityEvent OnPuzzleSolved;
    public string PuzzlePrompt => puzzlePrompt;
    bool isPlaced = false;
    bool isSolved = false;
    void Awake()
    {
        if (itemInWorld != null) itemScript = itemInWorld.GetComponent<Item>();
        if (player != null)
        {
            playerMove = player.GetComponent<Player_Movement>();
            playerCam = player.GetComponent<Player_Camera>();
        }
    }
    void Start()
    {
        puzzlePanel.SetActive(false);

        for (int i = 0; i < fuseButtons.Length; i++)
        {
            int index = i;
            fuseButtons[i].onValueChanged.AddListener((val) => OnToggleChanged(index, val));
        }
        UImanager.Instance.RegisterPanel(UImanager.UIPanelType.Puzzle, () => TogglePanel(true));
        GenerateProceduralStart();
    }

    private void GenerateProceduralStart()
    {
        if (fuseButtons == null || fuseButtons.Length == 0) return;

        float randomChance = Random.value;

        if (randomChance < 0.5f)
        {
            for (int i = 0; i < fuseButtons.Length; i++)
            {
                fuseButtons[i].SetIsOnWithoutNotify(true);
            }

            if (fuseButtons.Length >= 2)
            {
                int off = 0;
                while (off < 2)
                {
                    int randomIndex = Random.Range(0, fuseButtons.Length);
                    if (fuseButtons[randomIndex].isOn)
                    {
                        fuseButtons[randomIndex].SetIsOnWithoutNotify(false);
                        off++;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < fuseButtons.Length; i++)
            {
                fuseButtons[i].SetIsOnWithoutNotify(false);
            }
        }

        SyncAllVisuals();
    }

    public void Interact()
    {
        if (isSolved) return;

        if (!isPlaced)
        {
            if (EquipmentManager.Instance.CurrentEquippedItem != requiredItemData && itemInWorld.activeSelf) OnCantInteract?.Invoke();
            if (!itemInWorld.activeSelf) OnNeedItem?.Invoke();
            if (EquipmentManager.Instance.CurrentEquippedItem == requiredItemData) PlaceFuse();
            return;
        }

        TogglePanel(true);
    }

    private void PlaceFuse()
    {
        isPlaced = true;
        if (itemGameObject != null) itemGameObject.SetActive(true);
        InventoryManager.Instance.RemoveItem(requiredItemData);
        EquipmentManager.Instance.Unequip();
    }

    public void TogglePanel(bool state)
    {
        if (state)
        {
            if (!UImanager.Instance.RequestOpen(UImanager.UIPanelType.Puzzle)) return;
        }
        else UImanager.Instance.ReportClose(UImanager.UIPanelType.Puzzle);

        puzzlePanel.SetActive(state);
    }

    private void OnToggleChanged(int index, bool value)
    {
        if (!value)
        {
            fuseButtons[index].SetIsOnWithoutNotify(true);
            return;
        }

        ApplySwitchRules(index);

        SyncAllVisuals();
        CheckWin();
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

            if (visualFuses.Length > i && visualFuses[i] != null)
                visualFuses[i].value = state ? 1f : 0f;

            if (feedbackLights.Length > i && feedbackLights[i] != null)
                feedbackLights[i].color = state ? lightColorOn : lightColorOff;
        }
    }
    private void CheckWin()
    {
        foreach (Toggle t in fuseButtons) if (!t.isOn) return;
        isSolved = true;
        playerCam.LockCamera(false);
        OnPuzzleSolved?.Invoke();
        puzzlePrompt = string.Empty;
        TogglePanel(false);
    }
}   