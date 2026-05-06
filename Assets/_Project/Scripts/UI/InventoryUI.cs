using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private const int MAX_SLOTS = 3;

    [Header("Inventory Settings")]
    [SerializeField] KeyCode toggleKey = KeyCode.Tab;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] Image[] inventoryIcons = new Image[MAX_SLOTS];
    [SerializeField] Image[] inventoryBackground = new Image[MAX_SLOTS];
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;

    [Header("Camera Player")]
    [SerializeField] Player_Camera playerCamera;

    private bool isOpen = false;
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        inventoryPanel.SetActive(false);
        ClearInfo();

        InventoryManager.Instance.OnInventoryChanged += RechargeUI;

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            inventoryIcons[i].enabled = false;
        }
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

        if (isOpen) RechargeUI();
        else ClearInfo(); 
    }

    void RechargeUI()
    {
        int itemCount = inventoryManager.ItemCount;

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (i < itemCount)
            {
                ItemData item = inventoryManager.GetItem(i);
                if (item != null)
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
        ItemData item = inventoryManager.GetItem(slotIndex);
        if (item != null)
        {
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;

            inventoryBackground[slotIndex].color = Color.yellow;
        }
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

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RechargeUI;
    }
}