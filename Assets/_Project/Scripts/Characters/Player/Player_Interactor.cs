using UnityEngine;
using TMPro;

public class Player_Interactor : MonoBehaviour
{
    [Header("Prompt Formats")]
    [SerializeField] string itemPromptFormat = "[E] Recoger {0}";
    [SerializeField] string doorPromptFormat = "[E] Abrir / Cerrar";
    [SerializeField] string lockedDoorFormat = "Cerrado";

    [Header("Interaction Settings")]
    [SerializeField] float interactRange = 3f;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] Transform interactPoint;

    [Header("Interaction UI")]
    [SerializeField] GameObject interactPrompt;
    [SerializeField] TextMeshProUGUI promptText;

    Item currentItem;
    DoorController currentDoor;

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
                string fullText = string.Format(itemPromptFormat, item.itemData.itemName);
                ShowPrompt(fullText);
                return;
            }

            if (hit.collider.TryGetComponent(out DoorController door))
            {
                currentDoor = door;
                currentItem = null;

                string text = door.IsLocked ? lockedDoorFormat : doorPromptFormat;
                ShowPrompt(text);
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
            currentDoor.Interact(transform.position);
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