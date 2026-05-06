using UnityEngine;
using TMPro;

public class Player_Interaction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] float interactRange = 3f;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] Transform interactPoint;


    [Header("Interaction UI")]
    [SerializeField] GameObject interactPrompt;
    [SerializeField] TextMeshProUGUI promptText;

    Item currentItemInRange;

    void Update()
    {
        InteractDetector();
        InteractInput();
    }

    void InteractDetector()
    {
        Vector3 rayOrigin = interactPoint.position;
        Vector3 direction = interactPoint.forward;

        Item itemInRange = null;

        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, interactRange, interactLayer))
        {
            itemInRange = hit.collider.GetComponent<Item>();
        }
        if (itemInRange != null)

        {
            currentItemInRange = itemInRange;
            Showpromt(itemInRange.itemData.itemName);
        }
        else
        {
            currentItemInRange = null;
            HidePrompt();
        }
    }

    void TryPickUp(Item item)
    {
        bool added = InventoryManager.Instance.AddItem(item.itemData);

        if (added)
        {
            item.PickUp();
            HidePrompt();
            Debug.Log($"Picked up: {item.itemData.itemName}");
        }
        else
        {
            Debug.Log("Inventory is full.");

            if (promptText != null)
            {
                promptText.text = "Inventario lleno";
            }
        }

    }
    void InteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentItemInRange != null) TryPickUp(currentItemInRange);
    }
    void Showpromt(string itemName)
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(true);

        if (promptText != null)
            promptText.text = $"[E] Recoger {itemName}";
    }

    void HidePrompt()
    {
        if (interactPrompt  != null)
            interactPrompt.SetActive(false);
    }
}