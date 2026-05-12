using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class PuzzleFuseBox : MonoBehaviour
{
    [Header("Referencias del jugador")]
    [SerializeField] GameObject player;
    Player_Camera playerCamera;
    Player_Movement playerMovement;

    [Header("Panel UI")]
    [SerializeField] GameObject fuseBoxPanel;

    [Header("Puerta que se abre al resolver")]
    [Tooltip("Arrastrar el GameObject que tiene DoorController (el padre 'Door (Hinge)')")]
    [SerializeField] DoorController targetDoor;
    [Tooltip("Posición desde donde se calcula la dirección de apertura")]
    [SerializeField] Transform doorOpenFromPoint;

    [Header("Slots (6 en orden)")]
    [SerializeField] FuseSlot[] slots = new FuseSlot[6];

    [Header("Fusibles (F1, F2, F3, F4 en ese orden)")]
    [SerializeField] FuseDraggable[] fuses = new FuseDraggable[4];

    [Header("Feedback")]
    [SerializeField] TextMeshProUGUI feedbackText;
    [SerializeField] float feedbackDuration = 2.5f;

    [Header("Eventos")]
    public UnityEvent OnPuzzleSolved;

    bool isSolved = false;
    bool isPanelOpen = false;

    void Awake()
    {
        if (player != null)
        {
            playerCamera = player.GetComponent<Player_Camera>();
            playerMovement = player.GetComponent<Player_Movement>();
        }

        foreach (var slot in slots) if (slot != null) slot.Init(this);
        foreach (var fuse in fuses) if (fuse != null) fuse.Init(this);
    }

    void Start()
    {
        if (fuseBoxPanel != null) fuseBoxPanel.SetActive(false);
        HideFeedback();
    }

    void Update()
    {
        if (isPanelOpen && Input.GetKeyDown(KeyCode.Escape))
            TogglePanel(false);
    }

    public void OnPlayerInteract()
    {
        if (isSolved) return;
        TogglePanel(!isPanelOpen);
    }

    public void TogglePanel(bool state)
    {
        isPanelOpen = state;
        if (fuseBoxPanel != null) fuseBoxPanel.SetActive(state);

        if (playerCamera != null) playerCamera.CameraMovement(state);
        if (playerMovement != null) playerMovement.SetMovement(!state);

        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }

    public void CheckSolution()
    {
        for (int f = 0; f < fuses.Length; f++)
            if (fuses[f].CurrentSlotIndex < 0) return;

        int slotF1 = fuses[0].CurrentSlotIndex;
        int slotF2 = fuses[1].CurrentSlotIndex;
        int slotF3 = fuses[2].CurrentSlotIndex;
        int slotF4 = fuses[3].CurrentSlotIndex;

        bool rule1 = slotF1 >= 2;
        bool rule2 = slotF2 == slotF3 + 2;
        bool rule3 = slotF4 == slotF1 + 1;
        bool rule4 = Mathf.Abs(slotF1 - slotF2) > 1;

        if (rule1 && rule2 && rule3 && rule4)
            SolvePuzzle();
        else
            ShowFeedback("Algo no encaja...");
    }

    void SolvePuzzle()
    {
        isSolved = true;
        ShowFeedback("¡Las luces volvieron!");
        OnPuzzleSolved?.Invoke();
        StartCoroutine(SolveSequence());
        Debug.Log("[PuzzleB] ¡Resuelto!");
    }

    IEnumerator SolveSequence()
    {
        yield return new WaitForSeconds(1.5f);
        TogglePanel(false);

        if (targetDoor != null)
        {
            targetDoor.UnlockDoor();

            Vector3 openFrom = doorOpenFromPoint != null
                ? doorOpenFromPoint.position
                : (player != null ? player.transform.position : targetDoor.transform.position);

            targetDoor.Interact(openFrom);
        }

    }

    public int SlotCount => slots.Length;
    public FuseSlot GetSlot(int i) => (i >= 0 && i < slots.Length) ? slots[i] : null;

    void ShowFeedback(string msg)
    {
        if (feedbackText == null) return;
        StopAllCoroutines();
        StartCoroutine(FeedbackRoutine(msg));
    }

    IEnumerator FeedbackRoutine(string msg)
    {
        feedbackText.text = msg;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(feedbackDuration);
        HideFeedback();
    }

    void HideFeedback()
    {
        if (feedbackText != null) feedbackText.gameObject.SetActive(false);
    }
}