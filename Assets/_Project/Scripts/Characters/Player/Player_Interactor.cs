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
    }

    void InteractDetector()
    {
        Vector3 rayOrigin = interactPoint.position;
        Vector3 direction = interactPoint.forward;
        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, interactRange, interactLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)

            {
                currentItemInRange = item;
                Showpromt(item.itemData.itemName);

                if(Input.GetKeyDown(KeyCode.E))
                    TryPickUp(item);

                return;
            }

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