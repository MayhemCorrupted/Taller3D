using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InventoryUI))]
public class NotesUI : MonoBehaviour
{
    [Header("Note List")]
    [SerializeField] Transform container;
    [SerializeField] GameObject notePrefab;

    [Header("Fullscreen Note")]
    [SerializeField] GameObject notePanelFS;
    [SerializeField] Image noteImageFS;
    [SerializeField] Button nextNoteButton;
    [SerializeField] Button prevNoteButton;
    [SerializeField] Button toggleTranscriptButton;
    [SerializeField] Button closeNoteButton;

    [Header("Transcript Panel")]
    [SerializeField] GameObject transcriptPanel;
    [SerializeField] TextMeshProUGUI noteDescriptionText;
    [SerializeField] Button closeTranscriptButton;

    InventoryUI inventoryUI;
    bool openedFromInventory = false;
    int currentNoteIndex = 0;
    bool isTranscriptVisible = false;
    readonly List<GameObject> uiNoteItems = new();

    public bool IsNoteOpen => notePanelFS != null && notePanelFS.activeSelf;

    void Awake()
    {
        inventoryUI = GetComponent<InventoryUI>();

        if (nextNoteButton != null) nextNoteButton.onClick.AddListener(NextNote);
        if (prevNoteButton != null) prevNoteButton.onClick.AddListener(PrevNote);
        if (toggleTranscriptButton != null) toggleTranscriptButton.onClick.AddListener(ToggleTranscript);
        if (closeNoteButton != null) closeNoteButton.onClick.AddListener(CloseNoteFS);
        if (closeTranscriptButton != null) closeTranscriptButton.onClick.AddListener(HideTranscript);

        if (notePanelFS != null) notePanelFS.SetActive(false);
        if (transcriptPanel != null) transcriptPanel.SetActive(false);
    }

    void Start()
    {
        if (NotesManager.Instance != null)
            NotesManager.Instance.OnNoteCollected += RefreshNotesUI;

        UserInterfaceManager.Instance.RegisterPanel(
            UserInterfaceManager.PanelType.Notes,
            () =>
            {
                var notes = NotesManager.Instance.GetCollectedNotes();
                if (notes != null && notes.Count > 0) OpenNoteFS(currentNoteIndex);
            });
    }

    void OnEnable() => RefreshNotesUI();

    public void RefreshNotesUI()
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();
        ClearList();
        if (notes == null || notes.Count == 0) return;

        for (int i = 0; i < notes.Count; i++)
        {
            int index = i;
            GameObject noteGO = Instantiate(notePrefab, container);
            uiNoteItems.Add(noteGO);

            TextMeshProUGUI label = noteGO.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = string.IsNullOrEmpty(notes[i].itemName) ? "- ???" : $"- {notes[i].itemName}";

            if (noteGO.TryGetComponent<Button>(out var btn))
                btn.onClick.AddListener(() => OpenNoteFS(index));
        }
    }

    void ClearList()
    {
        foreach (var go in uiNoteItems)
            if (go != null) Destroy(go);

        uiNoteItems.Clear();
    }

    void OpenNoteFS(int index)
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();
        if (notes == null || notes.Count == 0 || index < 0) return;

        currentNoteIndex = Mathf.Clamp(index, 0, notes.Count - 1);

        if (inventoryUI != null && inventoryUI.IsInventoryOpen)
        {
            openedFromInventory = true;
            inventoryUI.TogglePanel(false);
        }
        else openedFromInventory = false;

        if (UserInterfaceManager.Instance.RequestOpenPanel(UserInterfaceManager.PanelType.Notes))
        {
            if (notePanelFS != null) notePanelFS.SetActive(true);
            UpdateNoteDisplay(notes);
        }
    }

    void UpdateNoteDisplay(List<NoteData> notes)
    {
        if (notes == null || currentNoteIndex < 0 || currentNoteIndex >= notes.Count) return;
        NoteData current = notes[currentNoteIndex];
        if (current == null) return;

        if (noteImageFS != null) noteImageFS.sprite = current.image;
        if (noteDescriptionText != null) noteDescriptionText.text = current.GetParsedDescription();

        HideTranscript();

        if (prevNoteButton != null) prevNoteButton.interactable = currentNoteIndex > 0;
        if (nextNoteButton != null) nextNoteButton.interactable = currentNoteIndex < notes.Count - 1;
    }

    public void CloseNoteFS()
    {
        if (notePanelFS != null) notePanelFS.SetActive(false);
        HideTranscript();
        UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Notes);

        if (openedFromInventory && inventoryUI != null)
        {
            openedFromInventory = false;
            inventoryUI.TogglePanel(true);
        }
    }

    public void ForceCloseAll()
    {
        openedFromInventory = false;
        CloseNoteFS();
    }


    public void ToggleTranscript()
    {
        isTranscriptVisible = !isTranscriptVisible;
        if (transcriptPanel != null) transcriptPanel.SetActive(isTranscriptVisible);
    }

    void HideTranscript()
    {
        isTranscriptVisible = false;
        if (transcriptPanel != null) transcriptPanel.SetActive(false);
    }


    void NextNote()
    {
        var notes = NotesManager.Instance?.GetCollectedNotes();
        if (notes != null && currentNoteIndex < notes.Count - 1)
        {
            currentNoteIndex++;
            UpdateNoteDisplay(notes);
        }
    }

    void PrevNote()
    {
        var notes = NotesManager.Instance?.GetCollectedNotes();
        if (notes != null && currentNoteIndex > 0)
        {
            currentNoteIndex--;
            UpdateNoteDisplay(notes);
        }
    }

    void OnDestroy()
    {
        if (NotesManager.Instance != null)
            NotesManager.Instance.OnNoteCollected -= RefreshNotesUI;
    }
}