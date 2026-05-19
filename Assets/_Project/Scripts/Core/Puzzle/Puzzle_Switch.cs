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
    [SerializeField] Toggle[] fuseToggles;
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

        for (int i = 0; i < fuseToggles.Length; i++)
        {
            int index = i;
            fuseToggles[i].onValueChanged.AddListener((val) => OnToggleChanged(index, val));
        }

        GenerateProceduralStart();
    }

    void GenerateProceduralStart()
    {
        bool allOn = true;

        for (int i = 0; i < fuseToggles.Length; i++)
        {
            bool randomState = Random.value > 0.5f;
            fuseToggles[i].SetIsOnWithoutNotify(randomState);

            if (!randomState) allOn = false;
        }

        if (allOn)
        {
            int randomIndex = Random.Range(0, fuseToggles.Length);
            fuseToggles[randomIndex].SetIsOnWithoutNotify(false);
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

    void PlaceFuse()
    {
        isPlaced = true;
        if (itemGameObject != null) itemGameObject.SetActive(true);
        InventoryManager.Instance.RemoveItem(requiredItemData);
        EquipmentManager.Instance.Unequip();
    }

    public void TogglePanel(bool state)
    {
        puzzlePanel.SetActive(state);
        playerCam.LockCamera(state);
        playerMove.CanMove(!state);

        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }

    void OnToggleChanged(int index, bool value)
    {
        if (value) ApplyRules(index);

        SyncAllVisuals();
        CheckWin();
    }

    void ApplyRules(int index)
    {
        switch (index)
        {
            case 0: if (fuseToggles[2].isOn) fuseToggles[2].SetIsOnWithoutNotify(false); break;
            case 1: if (fuseToggles[0].isOn) fuseToggles[0].SetIsOnWithoutNotify(false); break;
            case 3: if (fuseToggles[1].isOn) fuseToggles[1].SetIsOnWithoutNotify(false); break;
        }
    }

    void SyncAllVisuals()
    {
        for (int i = 0; i < fuseToggles.Length; i++)
        {
            bool state = fuseToggles[i].isOn;

            if (visualFuses.Length > i && visualFuses[i] != null)
                visualFuses[i].value = state ? 1f : 0f;

            if (feedbackLights.Length > i && feedbackLights[i] != null)
                feedbackLights[i].color = state ? lightColorOn : lightColorOff;
        }
    }

    void CheckWin()
    {
        foreach (Toggle t in fuseToggles) if (!t.isOn) return;
        isSolved = true;
        playerCam.LockCamera(false);
        OnPuzzleSolved?.Invoke();
        puzzlePrompt = string.Empty;
        TogglePanel(false);
    }
}   