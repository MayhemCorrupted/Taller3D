using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(NotesUI))]
public class InventoryUI : MonoBehaviour
{
    private const int MAX_SLOTS = 3;

    [Header("Inventory Settings")]
    [SerializeField] KeyCode toggleKey = KeyCode.Tab;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;

    [Header("Equip Prompt")]
    [SerializeField] GameObject equipPromptPanel;
    [SerializeField] Button equipConfirmButton;
    TextMeshProUGUI equipConfirmText;

    [Tooltip("Asigna las referencias de manera ordenada (0 = primer slot, 1 = segundo slot, 2 = tercer slot)")]
    [Header("Inventory Visuals")]
    [SerializeField] Button[] inventorySlots = new Button[MAX_SLOTS];
    [SerializeField] Image[] inventoryIcons = new Image[MAX_SLOTS];
    [SerializeField] Image[] inventoryBackground = new Image[MAX_SLOTS];

    int slotSelectedToEquip = 0;
    int equippedSlotIndex = -1;

    readonly float[] lastClickTimes = new float[MAX_SLOTS];
    const float doubleClickThreshold = 0.3f;

    bool isInventoryOpen = false;
    NotesUI noteUI;

    public bool IsInventoryOpen => isInventoryOpen;

    void Awake()
    {
        noteUI = GetComponent<NotesUI>();
        inventoryPanel.SetActive(false);
        ClearAllInfo();

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (inventoryIcons[i] != null)
            {
                inventoryIcons[i].enabled = false;
                SetSlotEvents(i);
            }
        }

        if (equipPromptPanel != null) equipPromptPanel.SetActive(false);

        if (equipConfirmButton != null)
        {
            equipConfirmButton.onClick.AddListener(() => EquipFromSlot(slotSelectedToEquip));
            equipConfirmText = equipConfirmButton.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void Start()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RechargeUI;

        UserInterfaceManager.Instance.RegisterPanel(
            UserInterfaceManager.PanelType.Inventory,
            () => TogglePanel(true));
    }

    void Update()
    {
        if (!Input.GetKeyDown(toggleKey)) return;

        if (noteUI != null && noteUI.IsNoteOpen) noteUI.ForceCloseAll();
        else if (isInventoryOpen) TogglePanel(false);
        else TogglePanel(true);
    }

    void SetSlotEvents(int index)
    {
        if (inventorySlots[index] == null) return;

        if (!inventorySlots[index].gameObject.TryGetComponent<EventTrigger>(out var trigger))
            trigger = inventorySlots[index].gameObject.AddComponent<EventTrigger>();

        AddTriggerEntry(trigger, EventTriggerType.PointerClick, _ => OnSlotPointerClick(index));
        AddTriggerEntry(trigger, EventTriggerType.PointerEnter, _ => ShowItemDetails(index));
        AddTriggerEntry(trigger, EventTriggerType.PointerExit, _ => ClearItemInfo());
    }

    void AddTriggerEntry(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new() { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    void OnSlotPointerClick(int slotIndex)
    {
        ShowItemDetails(slotIndex);
        ShowEquipPrompt(slotIndex);

        float timeSince = Time.time - lastClickTimes[slotIndex];
        if (timeSince <= doubleClickThreshold) EquipFromSlot(slotIndex);
        else lastClickTimes[slotIndex] = Time.time;
    }

    public void TogglePanel(bool state)
    {
        if (state)
        {
            if (!UserInterfaceManager.Instance.RequestOpenPanel(UserInterfaceManager.PanelType.Inventory)) return;
        }
        else
        {
            UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Inventory);
        }

        isInventoryOpen = state;
        inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen) RechargeUI();
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
        for (int i = 0; i < MAX_SLOTS; i++) inventoryBackground[i].color = Color.white;
    }

    public void ShowEquipPrompt(int slotIndex)
    {
        ItemData item = InventoryManager.Instance.GetItem(slotIndex);
        if (item == null) return;

        slotSelectedToEquip = slotIndex;
        equipPromptPanel.SetActive(true);

        if (equipConfirmText != null)
            equipConfirmText.text = (slotIndex == equippedSlotIndex) ? "Unequip" : "Equip";

        PositionPromptNextToSlot(slotIndex);
    }

    void PositionPromptNextToSlot(int slotIndex)
    {
        RectTransform slotRect = inventoryBackground[slotIndex].rectTransform;
        RectTransform promptRect = equipPromptPanel.GetComponent<RectTransform>();
        if (slotRect == null || promptRect == null) return;

        promptRect.position = slotRect.position;

        float offsetX = (slotRect.rect.width * slotRect.lossyScale.x * 0.5f)
                      + (promptRect.rect.width * promptRect.lossyScale.x * 0.5f);

        float offsetY = (slotRect.rect.height * slotRect.lossyScale.y * 0.5f)
                      - (promptRect.rect.height * promptRect.lossyScale.y * 0.5f);

        promptRect.position += new Vector3(offsetX, -offsetY * 2.5f, 0);
    }

    public void EquipFromSlot(int slotIndex)
    {
        ItemData item = InventoryManager.Instance.GetItem(slotIndex);
        if (item == null) return;

        if (slotIndex == equippedSlotIndex)
        {
            EquipmentManager.Instance.Unequip();
            equippedSlotIndex = -1;
        }
        else
        {
            EquipmentManager.Instance.EquipItem(item);
            equippedSlotIndex = slotIndex;
        }

        HideEquipPrompt();
        RechargeUI();
        TogglePanel(false);
    }

    void RechargeUI()
    {
        if (InventoryManager.Instance == null) return;

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            ItemData item = InventoryManager.Instance.GetItem(i);

            bool hasItem = item != null && item.itemType == ItemData.ItemType.Interactable;
            inventoryIcons[i].sprite = hasItem ? item.sprite : null;
            inventoryIcons[i].enabled = hasItem;
            inventoryIcons[i].raycastTarget = hasItem;

            if (!hasItem && equippedSlotIndex == i) equippedSlotIndex = -1;
        }

        UpdateSlotVisuals();
    }

    void UpdateSlotVisuals()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
            inventoryBackground[i].color = (i == equippedSlotIndex) ? Color.green : Color.white;
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

    public bool IsSlotEquipped(int slotIndex) => slotIndex == equippedSlotIndex;

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RechargeUI;
    }
}