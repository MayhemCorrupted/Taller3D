using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotesUI : MonoBehaviour
{
    [Header("Note Settings")]
    [SerializeField] Transform container;
    [SerializeField] GameObject notePrefab;

    [Header("Fullscreen Note Settings")]
    [SerializeField] GameObject notePanelFS;
    [SerializeField] Image noteImageFS;
    [SerializeField] Button nextNoteButton;
    [SerializeField] Button prevNoteButton;
    [SerializeField] Button toggleTranscriptButtonFS;
    [SerializeField] Button closeNoteButtonFS;

    [Header("Transcript Panel Settings")]
    [SerializeField] GameObject transcriptPanelFS;
    [SerializeField] TextMeshProUGUI noteDescriptionFS;
    [SerializeField] Button closeTranscriptButtonFS;    

    int currentNoteIndex = 0;
    bool isTranscriptVisible = false;
    readonly List<GameObject> UInotes = new();

    void Awake()
    {
        if (nextNoteButton != null) nextNoteButton.onClick.AddListener(NextNote);
        if (prevNoteButton != null) prevNoteButton.onClick.AddListener(PrevNote);
        if (toggleTranscriptButtonFS != null) toggleTranscriptButtonFS.onClick.AddListener(ToggleTranscriptFS);
        if (closeNoteButtonFS != null) closeNoteButtonFS.onClick.AddListener(CloseNoteFS);

        if (closeTranscriptButtonFS != null) closeTranscriptButtonFS.onClick.AddListener(HideTranscript);

        if (notePanelFS != null) notePanelFS.SetActive(false);
        if (transcriptPanelFS != null) transcriptPanelFS.SetActive(false);
    }
    void Start()
    {
        if (NotesManager.Instance != null) NotesManager.Instance.OnNoteCollected += RefreshNotesUI;
    }
    void OnEnable() => RefreshNotesUI();
    void OnDisable() => CloseNoteFS();
    public void RefreshNotesUI()
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();

        ClearList();

        if (notes == null || notes.Count == 0) return;

        for (int i = 0; i < notes.Count; i++)
        {
            int index = i;
            GameObject newNote = Instantiate(notePrefab, container);
            UInotes.Add(newNote);

            TextMeshProUGUI itemName = newNote.GetComponentInChildren<TextMeshProUGUI>();
            if (itemName != null) itemName.text = string.IsNullOrEmpty(notes[i].itemName) ? $"- ???" : $"- {notes[i].itemName}";

            if (newNote.TryGetComponent<Button>(out var itemButton)) itemButton.onClick.AddListener(() => OpenNoteFS(index));
        }
    }
    void ClearList()
    {
        foreach (var note in UInotes)
        {
            if (note != null) Destroy(note);
        }
        UInotes.Clear();
    }
    void OpenNoteFS(int index)
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();
        if (notes == null || index < 0 || notes.Count == 0) return;

        currentNoteIndex = Mathf.Clamp(index, 0, notes.Count - 1);
        if (notePanelFS != null) notePanelFS.SetActive(true);

        UpdateNoteDisplay(notes);
    }
    void UpdateNoteDisplay(List<NoteData> notes)
    {
        if (notes == null || currentNoteIndex < 0 || currentNoteIndex >= notes.Count) return;
        NoteData current = notes[currentNoteIndex];
        if (current == null) return;

        if (noteImageFS != null) noteImageFS.sprite = current.image;

        if (noteDescriptionFS != null) noteDescriptionFS.text = current.GetParsedDescription();

        HideTranscript();

        if (prevNoteButton != null) prevNoteButton.interactable = currentNoteIndex > 0;
        if (nextNoteButton != null) nextNoteButton.interactable = currentNoteIndex < notes.Count - 1;
    }
    public void ToggleTranscriptFS()
    {
        isTranscriptVisible = !isTranscriptVisible;
        if (transcriptPanelFS != null) transcriptPanelFS.SetActive(isTranscriptVisible);
    }

    void HideTranscript()
    {
        isTranscriptVisible = false;
        if (transcriptPanelFS != null) transcriptPanelFS.SetActive(false);
    }

    void CloseNoteFS()
    {
        if (notePanelFS != null) notePanelFS.SetActive(false);
        HideTranscript();
    }

    void NextNote()
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();
        if (notes != null && currentNoteIndex < notes.Count - 1)
        {
            currentNoteIndex++;
            UpdateNoteDisplay(notes);
        }
    }
    void PrevNote()
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();
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
