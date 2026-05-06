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
    [Header("Note Settings")]
    [SerializeField] Image noteDisplayImage;
    [SerializeField] TextMeshProUGUI noteDescription;
    [SerializeField] Button nextNoteButton, prevNoteButton;
    [Header("Equip Prompt")]
    [SerializeField] GameObject equipPromptPanel;
    [SerializeField] Button equipConfirmButton;
    [Header("Inventory Slots")]
    [SerializeField] Image[] inventoryIcons = new Image[MAX_SLOTS];
    [SerializeField] Image[] inventoryBackground = new Image[MAX_SLOTS];

    int currentNoteIndex = 0;
    int slotSelectedToEquip;
    bool isOpen = false;

    void Awake()
    {
        inventoryPanel.SetActive(false);        
        ClearAllInfo();

        if(InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RechargeUI;
        if(NotesManager.Instance != null)
            NotesManager.Instance.OnNoteCollected += RefreshNotesUI;

        for (int i = 0; i < MAX_SLOTS; i++) 
            if (inventoryIcons[i] != null) inventoryIcons[i].enabled = false;

        if (equipPromptPanel != null) equipPromptPanel.SetActive(false);
        if (equipConfirmButton != null) equipConfirmButton.onClick.AddListener(() => EquipFromSlot(slotSelectedToEquip));
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
        if (Input.GetKeyDown(toggleKey)) TogglePanel();
    }
    void TogglePanel()
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
        else ClearAllInfo(); 
    }
   public void ClearItemInfo()
    {
        if (itemNameText != null) itemNameText.text = "--";
        if (itemDescriptionText != null) itemDescriptionText.text = "";
    }
    public void HideEquipPrompt()
    {
        if (equipPromptPanel != null) equipPromptPanel.SetActive(false);
    }
    public void ClearAllInfo()
    {
        ClearItemInfo();
        HideEquipPrompt();
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            inventoryBackground[i].color = Color.white;
        }
    }
    #region Equipment_Logic
    public void ShowEquipPrompt(int slotIndex, Vector2 mousePos)
    {
        ItemData itemData = InventoryManager.Instance.GetItem(slotIndex);
        if(itemData == null) return;
        
        slotSelectedToEquip = slotIndex;
        equipPromptPanel.SetActive(true);
        equipPromptPanel.transform.position = mousePos + new Vector2(40, -40);
    }
    public void EquipFromSlot(int slotIndex)
    {
        ItemData item = InventoryManager.Instance.GetItem(slotIndex);
        if(item!= null)
        {
            EquipmentManager.Instance.EquipItem(item);
            equipPromptPanel.SetActive(false);
            TogglePanel();
        }
    }
    #endregion
    #region Inventory_Logic
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
        }
    }
#endregion
    #region Note_Logic
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
    #endregion

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RechargeUI;
    }
}