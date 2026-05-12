using UnityEngine;
using TMPro;

public class Player_Interactor : MonoBehaviour
{
    [Header("Prompt Formats")]
    [SerializeField] string itemPromptFormat = "[E] Recoger {0}";
    [SerializeField] string doorPromptFormat = "[E] Abrir / Cerrar";
    [SerializeField] string lockedDoorFormat = "Cerrado";
    [SerializeField] string puzzlePromptFormat = "[E] Interactuar";

    [Header("Interaction Settings")]
    [SerializeField] float interactRange = 3f;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] Transform interactPoint;

    [Header("Interaction UI")]
    [SerializeField] GameObject interactPrompt;
    [SerializeField] TextMeshProUGUI promptText;

    Item currentItem;
    DoorController currentDoor;
    PuzzleBoxInteractable currentFuseBox;

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
                SetTarget(item, null, null);
                ShowPrompt(string.Format(itemPromptFormat, item.itemData.itemName));
                return;
            }

            if (hit.collider.TryGetComponent(out PuzzleBoxInteractable fuseBox))
            {
                SetTarget(null, null, fuseBox);
                ShowPrompt(puzzlePromptFormat);
                return;
            }

            if (hit.collider.TryGetComponent(out DoorController door))
            {
                SetTarget(null, door, null);
                ShowPrompt(door.IsLocked ? lockedDoorFormat : doorPromptFormat);
                return;
            }
        }

        SetTarget(null, null, null);
        HidePrompt();
    }

    void InteractInput()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (currentItem != null)
        {
            currentItem.PickUp();
            currentItem = null;
            HidePrompt();
            return;
        }

        if (currentFuseBox != null)
        {
            currentFuseBox.Interact();
            return;
        }

        if (currentDoor != null)
        {
            if (currentDoor.TryGetComponent(out KeyDoor keyDoor))
            {
                ItemData held = EquipmentManager.Instance.CurrentEquippedItem;
                keyDoor.TryUnlock(held, transform.position);
            }
            else currentDoor.Interact(transform.position);
        }
    }

    void SetTarget(Item item, DoorController door, PuzzleBoxInteractable fuseBox)
    {
        currentItem = item;
        currentDoor = door;
        currentFuseBox = fuseBox;
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