using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotesUI : MonoBehaviour
{
    [Header("Note Settings")]
    [SerializeField] Image noteDisplayImage;
    [SerializeField] Button nextNoteButton, prevNoteButton;
    [SerializeField] Button openNoteButtonFS;

    [Header("Fullscreen Note Settings")]
    [SerializeField] GameObject notePanelFS;
    [SerializeField] Image noteImageFS;
    [SerializeField] TextMeshProUGUI noteDescriptionFS;
    [SerializeField] Button toggleTranscriptButtonFS;
    [SerializeField] Button closeNoteButtonFS;

    int currentNoteIndex = 0;
    bool isTranscriptVisible = false;

    void Awake()
    {
        if (nextNoteButton != null) nextNoteButton.onClick.AddListener(NextNote);
        if (prevNoteButton != null) prevNoteButton.onClick.AddListener(PrevNote);
        if (openNoteButtonFS != null) openNoteButtonFS.onClick.AddListener(OpenNoteFS);
        if (toggleTranscriptButtonFS != null) toggleTranscriptButtonFS.onClick.AddListener(ToggleTranscriptFS);
        if (closeNoteButtonFS != null) closeNoteButtonFS.onClick.AddListener(CloseNoteFS);
        if (notePanelFS != null) notePanelFS.SetActive(false);
    }

    void Start()
    {
        if (NotesManager.Instance != null)
        {
            NotesManager.Instance.OnNoteCollected += RefreshNotesUI;
        }
    }
    void OnEnable()
    {
        RefreshNotesUI();
    }
    void OnDisable()
    {
        CloseNoteFS();
    }
    public void RefreshNotesUI()
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();

        if (notes == null || notes.Count == 0)
        {
            if (noteDisplayImage != null) noteDisplayImage.enabled = false;
            if (nextNoteButton != null) nextNoteButton.interactable = false;
            if (prevNoteButton != null) prevNoteButton.interactable = false;
            if (openNoteButtonFS != null) openNoteButtonFS.interactable = false;
            currentNoteIndex = 0;
            return;
        }

        if (noteDisplayImage != null) noteDisplayImage.enabled = true;
        UpdateNoteDisplay(notes);
    }
    void UpdateNoteDisplay(List<NoteData> notes)
    {
        if (notes == null || notes.Count == 0) return;

        currentNoteIndex = Mathf.Clamp(currentNoteIndex, 0, notes.Count - 1);
        NoteData current = notes[currentNoteIndex];

        if (current != null)
        {
            if (noteDisplayImage != null)
            {
                noteDisplayImage.sprite = current.image;
                noteDisplayImage.enabled = current.image != null;
            }
        }

        if (prevNoteButton != null) prevNoteButton.interactable = currentNoteIndex > 0;
        if (nextNoteButton != null) nextNoteButton.interactable = currentNoteIndex < notes.Count - 1;
    }
    void OpenNoteFS()
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();
        if (notes == null || notes.Count == 0) return;
        NoteData current = notes[currentNoteIndex];

        if (notePanelFS != null) notePanelFS.SetActive(true);
        if (noteImageFS != null) noteImageFS.sprite = current.image;
        if (noteDescriptionFS != null) noteDescriptionFS.text = current.NoteDescription;
        isTranscriptVisible = false;
        if (noteDescriptionFS != null) noteDescriptionFS.gameObject.SetActive(false);
    }
    void ToggleTranscriptFS()
    {
        isTranscriptVisible = !isTranscriptVisible;
        if (noteDescriptionFS != null) noteDescriptionFS.gameObject.SetActive(isTranscriptVisible);
    }
    void CloseNoteFS()
    {
        if (notePanelFS != null) notePanelFS.SetActive(false);
    }
    void NextNote()
    {
        currentNoteIndex++; 
        RefreshNotesUI();
    }
    void PrevNote()
    {
        currentNoteIndex--;
        RefreshNotesUI();
    }
    void OnDestroy()
    {
        if (NotesManager.Instance != null)
            NotesManager.Instance.OnNoteCollected -= RefreshNotesUI;
    }
}
