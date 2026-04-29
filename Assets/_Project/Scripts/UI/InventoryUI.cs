using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] KeyCode toggleKey = KeyCode.Tab;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] Transform slotsParent;
    [SerializeField] GameObject slotPrefab;

    bool isOpen = false;

    void Start()
    {
        inventoryPanel.SetActive(false);
        InventoryManager.Instance.OnInventoryChanged += RechargeUI;
    }
    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) ToggleInventory();
    }
    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        if(isOpen) RechargeUI();
    }
    void RechargeUI()
    {
        foreach (Transform child in slotsParent) Destroy(child.gameObject);
        foreach (ItemData item in InventoryManager.Instance.GetItems())
        {
            GameObject slot = Instantiate(slotPrefab, slotsParent);
            slot.GetComponent<InventorySlot>().Setup(item);
        }
    }
    void OnDestroy()
    {
        InventoryManager.Instance.OnInventoryChanged -= RechargeUI;
    }
}
