using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private const int MAX_SLOTS = 3;

    [Header("Player")]
    [SerializeField] GameObject player;
    Player_Camera playerCamera;
    Player_Movement playerMovement;

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
    int slotSelectedToEquip = 0;
    int equipedSlotIndex = -1;
    bool isOpen = false;

    void Awake()
    {

        inventoryPanel.SetActive(false);
        ClearAllInfo();

        for (int i = 0; i < MAX_SLOTS; i++)
            if (inventoryIcons[i] != null) inventoryIcons[i].enabled = false;

        if (player != null) playerCamera = player.GetComponentInChildren<Player_Camera>();
        if (player != null) playerMovement = player.GetComponent<Player_Movement>();
        if (equipPromptPanel != null) equipPromptPanel.SetActive(false);
        if (equipConfirmButton != null) equipConfirmButton.onClick.AddListener(() => EquipFromSlot(slotSelectedToEquip));
        if (nextNoteButton != null) nextNoteButton.onClick.AddListener(NextNote);
        if (prevNoteButton != null) prevNoteButton.onClick.AddListener(PrevNote);

#if UNITY_EDITOR
        if (inventoryBackground.Length != MAX_SLOTS || inventoryIcons.Length != MAX_SLOTS)
            Debug.LogError("[InventoryUI] Asigná exactamente 3 slots en el Inspector.");
#endif
    }

    void Start()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RechargeUI;
        if (NotesManager.Instance != null)
            NotesManager.Instance.OnNoteCollected += RefreshNotesUI;
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
        playerMovement.SetMovement(!isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        if (isOpen) { RefreshNotesUI(); RechargeUI(); }
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
            inventoryBackground[i].color = Color.white;
    }
    #region Inventory_Display
    public void ShowEquipPrompt(int slotIndex, Vector2 mousePos)
    {
        if (slotIndex == equipedSlotIndex) 
        {
            Debug.Log("Item equipado"); 
            return;
        } 

        ItemData item = InventoryManager.Instance.GetItem(slotIndex);
        if (item == null) return;

        slotSelectedToEquip = slotIndex;
        equipPromptPanel.SetActive(true);
        equipPromptPanel.transform.position = mousePos + new Vector2(40, -40);
    }

    public void EquipFromSlot(int slotIndex)
    {
        ItemData item = InventoryManager.Instance.GetItem(slotIndex);
        if (item != null)
        {
            EquipmentManager.Instance.EquipItem(item);
            equipPromptPanel.SetActive(false);
            equipedSlotIndex = slotIndex;

            HideEquipPrompt();
            RechargeUI();
            TogglePanel();
        }
    }
    void RechargeUI()
    {
        if (InventoryManager.Instance == null) return;
        _ = InventoryManager.Instance.ItemCount;

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            ItemData item = InventoryManager.Instance.GetItem(i);

            if (item != null && item.itemType == ItemData.ItemType.Interactable)
            {
                inventoryIcons[i].sprite = item.sprite;
                inventoryIcons[i].enabled = true;
                inventoryIcons[i].raycastTarget = true;
            }
            else
            {
                inventoryIcons[i].sprite = null;
                inventoryIcons[i].enabled = false;
                inventoryIcons[i].raycastTarget = false;
                if (equipedSlotIndex == i) equipedSlotIndex = -1; 
            }
        }
        UpdateSlotVisuals();
    }
    public bool IsSlotEquipped(int slotIndex) => slotIndex == equipedSlotIndex;
    void UpdateSlotVisuals()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
            inventoryBackground[i].color = (i == equipedSlotIndex) ? Color.green : Color.white;
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
        else ClearItemInfo();
    }
    #endregion
    #region Notes_Display
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
            if (noteDescription != null) noteDescription.text = current.NoteDescription;
        }

        if (prevNoteButton != null) prevNoteButton.interactable = currentNoteIndex > 0;
        if (nextNoteButton != null) nextNoteButton.interactable = currentNoteIndex < notes.Count - 1;
    }
    public void RefreshNotesUI()
    {
        if (NotesManager.Instance == null) return;
        var notes = NotesManager.Instance.GetCollectedNotes();

        if (notes == null || notes.Count == 0)
        {
            if (noteDisplayImage != null) noteDisplayImage.enabled = false;
            if (noteDescription != null) noteDescription.text = "No tienes ninguna nota.";
            if (nextNoteButton != null) nextNoteButton.interactable = false;
            if (prevNoteButton != null) prevNoteButton.interactable = false;
            currentNoteIndex = 0;
            return;
        }

        if (noteDisplayImage != null) noteDisplayImage.enabled = true;
        UpdateNoteDisplay(notes);
    }
    void NextNote() { currentNoteIndex++; RefreshNotesUI(); }
    void PrevNote() { currentNoteIndex--; RefreshNotesUI(); }
    #endregion
    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RechargeUI;
        if (NotesManager.Instance != null)
            NotesManager.Instance.OnNoteCollected -= RefreshNotesUI;
    }
}