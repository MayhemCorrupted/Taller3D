using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(NotesUI))]
public class InventoryUI : MonoBehaviour
{
    private const int MAX_SLOTS = 3;

    [Header("Inventory Settings")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;

    [Tooltip("Asigna las referencias de manera ordenada (0 = primer slot, 1 = segundo slot, 2 = tercer slot)")]
    [Header("Inventory Visuals")]
    [SerializeField] Button[] inventorySlots = new Button[MAX_SLOTS];
    [SerializeField] Image[] inventoryIcons = new Image[MAX_SLOTS];
    [SerializeField] Image[] inventoryBackground = new Image[MAX_SLOTS];

    int equipedSlotIndex = -1;
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
    }
    void Start()
    {
        if (InventoryManager.Instance != null) InventoryManager.Instance.OnInventoryChanged += RechargeUI;
        UserInterfaceManager.Instance.RegisterPanel(UserInterfaceManager.PanelType.Inventory, () => TogglePanel(true));
    }
    void Update()
    {
        if (Input.GetKeyDown(InputManager.Instance.InventoryKey))
        {
            if (noteUI != null && noteUI.IsNoteOpen) noteUI.ForceCloseAll();
            else if (isInventoryOpen) TogglePanel(false);
            else TogglePanel(true);
        }
    }
    void SetSlotEvents(int index)
    {
        if (inventorySlots[index] == null) return;

        if (!inventorySlots[index].gameObject.TryGetComponent<EventTrigger>(out var trigger))
            trigger = inventorySlots[index].gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry clickEntry = new() { eventID = EventTriggerType.PointerClick };
        clickEntry.callback.AddListener((data) => { OnSlotPointerClick(index); });
        trigger.triggers.Add(clickEntry);

        EventTrigger.Entry enterEntry = new() { eventID = EventTriggerType.PointerEnter };
        enterEntry.callback.AddListener((data) => { ShowItemDetails(index); });
        trigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new() { eventID = EventTriggerType.PointerExit };
        exitEntry.callback.AddListener((data) => { ClearItemInfo(); });
        trigger.triggers.Add(exitEntry);
    }
    void OnSlotPointerClick(int slotIndex)
    {
        ShowItemDetails(slotIndex);
        float timeSinceLastClick = Time.time - lastClickTimes[slotIndex];
        if (timeSinceLastClick <= doubleClickThreshold) EquipFromSlot(slotIndex);
        else lastClickTimes[slotIndex] = Time.time;
    }
    public void TogglePanel(bool state)
    {
        if (state)
        {
            if (!UserInterfaceManager.Instance.RequestOpenPanel(UserInterfaceManager.PanelType.Inventory)) return;
        }
        else UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Inventory);

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
    public void ClearAllInfo()
    {
        ClearItemInfo();
        for (int i = 0; i < MAX_SLOTS; i++) inventoryBackground[i].color = Color.white;
    }
    #region Inventory_Display
    
    public void EquipFromSlot(int slotIndex)
    {
        ItemData item = InventoryManager.Instance.GetItem(slotIndex);
        if (item == null) return;

        if (slotIndex == equipedSlotIndex) EquipmentManager.Instance.Unequip();
        else EquipmentManager.Instance.EquipItem(item);
        RechargeUI();
    }
    void RechargeUI()
    {
        if (InventoryManager.Instance == null) return;

        SyncEquippedSlotIndex();

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
    void SyncEquippedSlotIndex()
    {
        equipedSlotIndex = -1;
        ItemData currentEquipped = EquipmentManager.Instance.CurrentEquippedItem;

        if (currentEquipped != null)
        {
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if (InventoryManager.Instance.GetItem(i) == currentEquipped)
                {
                    equipedSlotIndex = i;
                    break;
                }
            }
        }
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
    void OnDestroy()
    {
        if (InventoryManager.Instance != null) InventoryManager.Instance.OnInventoryChanged -= RechargeUI;
    }
}