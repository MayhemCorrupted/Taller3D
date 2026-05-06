using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private const int MAX_SLOTS = 3;

    [Header("Camera Player")]
    [SerializeField] Player_Camera playerCamera;
    [Header("Inventory Settings")]
    [SerializeField] KeyCode toggleKey = KeyCode.Tab;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;
    [Header("Inventory Slots")]
    [SerializeField] Image[] inventoryIcons = new Image[MAX_SLOTS];
    [SerializeField] Image[] inventoryBackground = new Image[MAX_SLOTS];
    [Header("Note Settings")]
    [SerializeField] Image noteDisplayImage;
    [SerializeField] TextMeshProUGUI noteDescription;
    [SerializeField] Button nextNoteButton, prevNoteButton;

    int currentNoteIndex = 0;
    bool isOpen = false;

    void Awake()
    {
        inventoryPanel.SetActive(false);        
        ClearInfo();

        if(InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RechargeUI;
        if(NotesManager.Instance != null)
            NotesManager.Instance.OnNoteCollected += RefreshNotesUI;

        for (int i = 0; i < MAX_SLOTS; i++)
        {
           if(inventoryIcons[i] != null) inventoryIcons[i].enabled = false;
        }
        if (nextNoteButton != null) nextNoteButton.onClick.AddListener(NextNote);
        if (prevNoteButton != null) prevNoteButton.onClick.AddListener(PrevNote);
        #region Validation
#if UNITY_EDITOR
        if (inventoryBackground.Length != MAX_SLOTS || inventoryIcons.Length != MAX_SLOTS)
            Debug.LogError("Asegúrate de tener exactamente 3 slots asignados en el Inspector.");
#endif
        #endregion
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) ToggleInventory();
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
        playerCamera.CameraMovement(isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        if (isOpen)
        {
            RefreshNotesUI();
            RechargeUI();
        }
        else ClearInfo(); 
    }
    public void ClearInfo()
    {
        itemNameText.text = "--";
        itemDescriptionText.text = "";

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            inventoryBackground[i].color = Color.white;
        }
    }
    void RechargeUI()
    {
        if (InventoryManager.Instance == null) return;
        int itemCount = InventoryManager.Instance.ItemCount;
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (i < itemCount)
            {
                ItemData item = InventoryManager.Instance.GetItem(i);
                if (item != null && item.itemType == ItemData.ItemType.Interactable)
                {
                    inventoryIcons[i].sprite = item.sprite;
                    inventoryIcons[i].enabled = true;
                    inventoryIcons[i].raycastTarget = true;
                }
            }
            else
            {
                inventoryIcons[i].sprite = null;
                inventoryIcons[i].enabled = false;
                inventoryIcons[i].raycastTarget = false;
            }

            inventoryBackground[i].color = Color.white;
        }
    }
    public void ShowItemDetails(int slotIndex)
    {
        if (InventoryManager.Instance == null) return;
        ItemData item = InventoryManager.Instance.GetItem(slotIndex);
        if (item != null)
        {
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;

            inventoryBackground[slotIndex].color = Color.yellow;
        }
    }
    void UpdateNoteDisplay(List<NoteData> notes)
    {
        if (notes == null || notes.Count == 0) return;
        currentNoteIndex = Mathf.Clamp(currentNoteIndex, 0, notes.Count - 1);
        NoteData currentNote = notes[currentNoteIndex];
        if (currentNote != null)
        {
            if(noteDisplayImage != null) noteDisplayImage.sprite = currentNote.image;
            if(noteDescription != null) noteDescription.text = currentNote.NoteDescription;
        }
        if (prevNoteButton != null) prevNoteButton.interactable = currentNoteIndex > 0;
        if (nextNoteButton != null) nextNoteButton.interactable = currentNoteIndex < notes.Count - 1;
    }
    public void RefreshNotesUI()
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();
        if (notes.Count == 0 || notes == null) 
        {
            if (noteDisplayImage != null) noteDisplayImage.enabled = false;
            if (noteDescription != null) noteDescription.text = "No cuentas con ninguna nota";
            if (nextNoteButton != null) nextNoteButton.interactable = false;
            if (prevNoteButton != null) prevNoteButton.interactable = false;
            currentNoteIndex = 0;
            return; 
        }
        if (noteDisplayImage != null) noteDisplayImage.enabled = true;
        UpdateNoteDisplay(notes);
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
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RechargeUI;
    }
}