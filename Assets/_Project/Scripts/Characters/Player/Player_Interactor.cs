using UnityEngine;
using TMPro;

public class Player_Interactor : MonoBehaviour
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
        if (Cursor.visible) return;
        InteractDetector();
        InteractInput();
    }

    void InteractDetector()
    {
        Vector3 rayOrigin = interactPoint.position;
        Vector3 direction = interactPoint.forward;

        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent<Item>(out var itemFound))
            {
                currentItemInRange = itemFound;
                ShowPrompt(itemFound.itemData.itemName);
                return;
            }
        }
        currentItemInRange = null;
        HidePrompt();
    }
    void InteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentItemInRange != null)
        {
            TryPickUp(currentItemInRange);
        }
    }
    void TryPickUp(Item item)
    {
        item.PickUp();

        Debug.Log($"Interacción enviada a: {item.itemData.itemName}");
        currentItemInRange = null;
        HidePrompt();
    }
    void ShowPrompt(string itemName)
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(true);

        if (promptText != null)
            promptText.text = $"[E] Recoger {itemName}";
    }
    void HidePrompt()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}
