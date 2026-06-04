using System.Collections.Generic;
using UnityEngine;

public class NotesManager : BaseManager<NotesManager>
{
    readonly List<NoteData> collectedNotes = new();

    public event System.Action OnNoteCollected;

    public int NoteCount => collectedNotes.Count;

    public void AddNote(NoteData note)
    {
        if (collectedNotes.Contains(note)) return;

        collectedNotes.Add(note);
        OnNoteCollected?.Invoke();
    }

    public List<NoteData> GetCollectedNotes() => collectedNotes;
}