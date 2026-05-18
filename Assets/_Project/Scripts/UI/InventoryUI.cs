using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [Header("Equip Prompt")]
    [SerializeField] GameObject equipPromptPanel;
    [SerializeField] Button equipConfirmButton;

    [Tooltip("Asigna las referencias de manera ordenada (0 = primer slot, 1 = segundo slot, 2 = tercer slot)")]
    [Header("Inventory Settings")]
    [SerializeField] Button[] inventorySlots = new Button[MAX_SLOTS];
    [SerializeField] Image[] inventoryIcons = new Image[MAX_SLOTS];
    [SerializeField] Image[] inventoryBackground = new Image[MAX_SLOTS];

    int slotSelectedToEquip = 0;
    int equipedSlotIndex = -1;
    readonly float[] lastClickTimes = new float[MAX_SLOTS];
    const float doubleClickThreshold = 0.3f;
    bool isOpen = false;

    void Awake()
    {
        inventoryPanel.SetActive(false);
        ClearAllInfo();

        for (int i = 0; i < MAX_SLOTS; i++)
            if (inventoryIcons[i] != null)
            {
                inventoryIcons[i].enabled = false;
                SetSlotEvents(i);
            }

        if (player != null) playerCamera = player.GetComponent<Player_Camera>();
        if (player != null) playerMovement = player.GetComponent<Player_Movement>();
        if (equipPromptPanel != null) equipPromptPanel.SetActive(false);
        if (equipConfirmButton != null) equipConfirmButton.onClick.AddListener(() => EquipFromSlot(slotSelectedToEquip));
    }

    void Start()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RechargeUI;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) TogglePanel();
    }
    void SetSlotEvents(int index)
    {
        if (inventorySlots[index] == null) return;

        if (!inventorySlots[index].gameObject.TryGetComponent<EventTrigger>(out var trigger)) trigger = inventorySlots[index].gameObject.AddComponent<EventTrigger>();

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
        ShowEquipPrompt(slotIndex);
        float timeSinceLastClick = Time.time - lastClickTimes[slotIndex];
        if (timeSinceLastClick <= doubleClickThreshold && !IsSlotEquipped(slotIndex))
            EquipFromSlot(slotIndex);
        else lastClickTimes[slotIndex] = Time.time;
    }

    void TogglePanel()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
        playerCamera.LockCamera(isOpen);
        playerMovement.CanMove(!isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        if (isOpen)
        {
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
            inventoryBackground[i].color = Color.white;
    }
    #region Inventory_Display
    public void ShowEquipPrompt(int slotIndex)
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

        RectTransform slotRect = inventoryBackground[slotIndex].rectTransform;
        RectTransform promptRect = equipPromptPanel.GetComponent<RectTransform>();

        if (slotRect != null && promptRect != null)
        {
            promptRect.position = slotRect.position;

            float slotHalfWidth = (slotRect.rect.width * slotRect.lossyScale.x) * 0.5f;
            float promptHalfWidth = (promptRect.rect.width * promptRect.lossyScale.x) * 0.5f;

            float slotHalfHeight = (slotRect.rect.height * slotRect.lossyScale.y) * 0.5f;
            float promptHalfHeight = (promptRect.rect.height * promptRect.lossyScale.y) * 0.5f;

            float offsetX = slotHalfWidth + promptHalfWidth;
            float offsetY = slotHalfHeight - promptHalfHeight;

            Vector3 offset = new(offsetX, -offsetY * 2.5f, 0);
            promptRect.position += offset;
        }
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
    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RechargeUI;
    }
}