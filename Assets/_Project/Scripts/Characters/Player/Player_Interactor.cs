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

    Item currentItem;
    PuzzleDoorInteractable currentDoor;

    void Update()
    {
        if (Cursor.visible) return;
        InteractDetector();
        InteractInput();
    }

    void InteractDetector()
    {
        if (Physics.Raycast(interactPoint.position, interactPoint.forward,
                            out RaycastHit hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent(out Item item))
            {
                currentItem = item;
                currentDoor = null;
                ShowPrompt($"[E] Recoger {item.itemData.itemName}");
                return;
            }

            if (hit.collider.TryGetComponent(out PuzzleDoorInteractable door))
            {
                currentDoor = door;
                currentItem = null;
                ShowPrompt("[E] Interactuar");
                return;
            }
        }

        currentItem = null;
        currentDoor = null;
        HidePrompt();
    }

    void InteractInput()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (currentItem != null)
        {
            currentItem.PickUp();
            Debug.Log($"[Interactor] Recogido: {currentItem.itemData.itemName}");
            currentItem = null;
            HidePrompt();
            return;
        }

        if (currentDoor != null)
        {
            currentDoor.Interact();
        }
    }

    void ShowPrompt(string text)
    {
        if (interactPrompt != null) interactPrompt.SetActive(true);
        if (promptText != null) promptText.text = text;
    }

    void HidePrompt()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}