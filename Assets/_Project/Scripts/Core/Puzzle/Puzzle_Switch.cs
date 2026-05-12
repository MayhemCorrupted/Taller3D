using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Puzzle_Switch : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;
    [Header("Item Requirements")]
    [SerializeField] ItemData requiredItem;
    [SerializeField] GameObject fuseVisual;
    [Header("Puzzle Configs")]
    [SerializeField] GameObject puzzlePanel;
    [SerializeField] Scrollbar[] fuseScrollBars;
    [SerializeField] Image[] feedbackLights;
    [SerializeField] Color lightColorOn = Color.green;
    [SerializeField] Color lightColorOff = Color.red;
    [Header("Events")]
    [SerializeField] UnityEvent OnPuzzleSolved;

    private readonly int[][] solutions = new int[][]
    {
        new int[] { 1, 0, 1, 0 },
        new int[] { 0, 1, 0, 1 }
    };
    int[] currentSolution;
    bool isPlaced = false;
    bool isSolved = false;
    Player_Movement playerMove;
    Player_Camera playerCam;
    void Awake()
    {
        playerMove = player.GetComponent<Player_Movement>();
        playerCam = player.GetComponent<Player_Camera>();
    }
    void Start()
    {
        currentSolution = solutions[Random.Range(0, solutions.Length)];
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        UpdateLights();
    }
    public void Interact()
    {
        if (isSolved) return;
        if (!isPlaced)
        {
            if (EquipmentManager.Instance.CurrentEquippedItem == requiredItem) PlaceFuse();
            else Debug.Log("Falta fusible");
        }
        else
        {
            puzzlePanel.SetActive(!puzzlePanel.activeSelf);
            playerCam.CameraMovement(puzzlePanel.activeSelf);
            playerMove.SetMovement(puzzlePanel.activeSelf);
            Cursor.lockState = puzzlePanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = puzzlePanel.activeSelf;
        }
    }
    void PlaceFuse()
    {
        isPlaced = true;
        if (fuseVisual != null) fuseVisual.SetActive(true);
        InventoryManager.Instance.RemoveItem(requiredItem);
        EquipmentManager.Instance.Unequip();
        Debug.Log("Fusible colocado");
    }
    public void OnFuseClicked(int index)
    {
        float targetValue = fuseScrollBars[index].value < 0.5f ? 0 : 1;
        fuseScrollBars[index].value = targetValue;
        if (targetValue == 1) SetRules(index);
        UpdateLights();
        CheckWin();
    }
    void SetRules(int index)
    {
        switch (index)
        {
            case 0:
                fuseScrollBars[2].value = 0;
                break;
            case 1:
                fuseScrollBars[0].value = 0;
                break;
            case 2:
                break;
            case 3:
                fuseScrollBars[1].value = 0;
                break;
        }
    }
    void UpdateLights()
    {
        for (int i = 0; i < feedbackLights.Length; i++)
        {
            int currentValue = fuseScrollBars[i].value < 0.5f ? 0 : 1;
            feedbackLights[i].color = (currentValue == currentSolution[i]) ? lightColorOn : lightColorOff;
        }
    }
    void CheckWin()
    {
        for (int i = 0; i < fuseScrollBars.Length; i++)
        {
            int currentValue = fuseScrollBars[i].value < 0.5f ? 0 : 1;
            if (currentValue != currentSolution[i]) return;
        }
        isSolved = true;
        puzzlePanel.SetActive(false);
        OnPuzzleSolved?.Invoke();
        Debug.Log("Puzzle completado");
    }
}   