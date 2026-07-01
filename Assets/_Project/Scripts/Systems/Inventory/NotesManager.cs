using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public static NotesManager Instance { get; private set; }
    readonly List<NoteData> CollectedNotes = new();
    public event System.Action OnNoteCollected;
    public NoteData LastObtainedNote { get; private set; }
    public string LastObtainedNoteText => LastObtainedNote != null ? LastObtainedNote.itemName : string.Empty;
    void Awake()
    {
        if (Instance != null && Instance != this)
        { 
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void AddNote(NoteData note)
    {
        if(!CollectedNotes.Contains(note)) 
        {
            CollectedNotes.Add(note);
            LastObtainedNote = note;
            OnNoteCollected?.Invoke();
        }
    }
    public int NoteCount => CollectedNotes.Count;
    public List<NoteData> GetCollectedNotes() => CollectedNotes;
}
