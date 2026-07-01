using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[System.Serializable]
public class NoteInspectEvent
{
    [Tooltip("Escribe exactamente el 'itemName' del ScriptableObject de la nota.")]
    public string targetNoteName;
    public bool triggerOnce;
    [HideInInspector] public bool triggered = false;
    public UnityEvent onInspectTriggered;
}
[RequireComponent(typeof(InventoryUI))]
public class NotesUI : MonoBehaviour
{
    [Header("Note Settings")]
    [SerializeField] Image noteImage;
    [SerializeField] Transform noteContainer;
    [SerializeField] TextMeshProUGUI inspectNoteText;
    [Header("Interface References")]
    [SerializeField] GameObject notePrefab;
    [SerializeField] GameObject notePanel;
    [SerializeField] GameObject inspectPanel;
    [SerializeField] Button nextNoteButton;
    [SerializeField] Button prevNoteButton;
    [SerializeField] Button toggleInspectButton;
    [SerializeField] Button closeNoteButton;

    [Header("Special Note Inspect Events")]
    [SerializeField] NoteInspectEvent[] specialInspectEvents;
    public UnityEvent OnReadNote;

    InventoryUI inventoryUI;
    bool openedFromInventory = false;
    int currentNoteIndex = 0;
    readonly List<GameObject> UInotes = new();
    public bool IsNoteOpen => notePanel != null && notePanel.activeSelf;
    public string CurrentViewingNoteName
    {
        get
        {
            if (NotesManager.Instance == null) return string.Empty;
            var notes = NotesManager.Instance.GetCollectedNotes();
            if (notes != null && currentNoteIndex >= 0 && currentNoteIndex < notes.Count)
            {
                return notes[currentNoteIndex].itemName;
            }
            return string.Empty;
        }
    }
    void Awake()
    {
        inventoryUI = GetComponent<InventoryUI>();

        if (nextNoteButton != null) nextNoteButton.onClick.AddListener(NextNote);

        if (prevNoteButton != null) prevNoteButton.onClick.AddListener(PrevNote);

        if (toggleInspectButton != null) toggleInspectButton.onClick.AddListener(ToggleInspectText);

        if (closeNoteButton != null) closeNoteButton.onClick.AddListener(CloseNoteFS);

        if (notePanel != null) notePanel.SetActive(false);

        if (inspectPanel != null) inspectPanel.SetActive(false);
    }
    void Start()
    {
        if (NotesManager.Instance != null) NotesManager.Instance.OnNoteCollected += RefreshNotesUI;

        UserInterfaceManager.Instance.RegisterPanel(
            UserInterfaceManager.PanelType.Notes,
            () =>
            {
                var notes = NotesManager.Instance.GetCollectedNotes();
                if (notes != null && notes.Count > 0)
                {
                    if (notePanel != null) notePanel.SetActive(true);
                    UpdateNoteDisplay(notes);
                }
            },
            () =>
            {
                if (notePanel != null) notePanel.SetActive(false);
                if (inspectPanel != null) inspectPanel.SetActive(false);

                if (openedFromInventory && inventoryUI != null)
                {
                    openedFromInventory = false;
                    UserInterfaceManager.Instance.TryOpenPanel(UserInterfaceManager.PanelType.Inventory);
                }
            }
        );
    }
    void OnEnable() => RefreshNotesUI();
    public void ForceCloseAll()
    {
        openedFromInventory = false;
        CloseNoteFS();               
    }
    public void RefreshNotesUI()
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();

        ClearList();

        if (notes == null || notes.Count == 0) return;

        for (int i = 0; i < notes.Count; i++)
        {
            int index = i;
            
            GameObject newNote = Instantiate(notePrefab, noteContainer);
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

        if (inventoryUI != null && inventoryUI.IsInventoryOpen)
        {
            openedFromInventory = true;
            UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Inventory);
        }
        else openedFromInventory = false;
        UserInterfaceManager.Instance.TryOpenPanel(UserInterfaceManager.PanelType.Notes);
   
    }
    void UpdateNoteDisplay(List<NoteData> notes)
    {
        if (notes == null || currentNoteIndex < 0 || currentNoteIndex >= notes.Count) return;

        NoteData current = notes[currentNoteIndex];
        
        if (current == null) return;

        if (noteImage != null) noteImage.sprite = current.image;

        if (inspectNoteText != null) inspectNoteText.text = current.GetParsedDescription();

        if (inspectPanel != null) inspectPanel.SetActive(false);

        if (prevNoteButton != null) prevNoteButton.interactable = currentNoteIndex > 0;

        if (nextNoteButton != null) nextNoteButton.interactable = currentNoteIndex < notes.Count - 1;
    }
    public void ToggleInspectText()
    {
        if (inspectPanel != null)
        {
            bool isOpen = !inspectPanel.activeSelf;
            inspectPanel.SetActive(isOpen);

            if (isOpen)
            {
                string viewedNoteName = CurrentViewingNoteName;
                foreach (var noteEvent in specialInspectEvents)
                {
                    if (noteEvent.targetNoteName == viewedNoteName && (!noteEvent.triggerOnce))
                    {
                        if (noteEvent.triggerOnce && noteEvent.triggered) continue;

                        Debug.Log($"[NotesUI] Evento invocado por: {viewedNoteName}");
                        noteEvent.onInspectTriggered?.Invoke();
                        noteEvent.triggered = true;
                    }
                }
            }
        }
    }
    public void CloseNoteFS()
    {
        UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Notes);
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
        if (NotesManager.Instance != null) NotesManager.Instance.OnNoteCollected -= RefreshNotesUI;
    }
}
