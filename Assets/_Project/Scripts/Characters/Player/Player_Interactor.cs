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

    GameObject currentInteractable;

    void Update()
    {
        if (Cursor.visible) return;
        InteractDetector();
        InteractInput();
    }

    void InteractDetector()
    {
        if (interactPoint == null)
        {
            Debug.LogWarning("Falta asignar el 'interactPoint' en el Inspector.");
            return;
        }

        if (Physics.Raycast(interactPoint.position, interactPoint.forward, out RaycastHit hit, interactRange, interactLayer))
        {
            Item targetItem = hit.collider.GetComponentInParent<Item>();
            if (targetItem != null)
            {
                string itemName = targetItem.itemData != null ? targetItem.itemData.itemName : targetItem.name;
                SetCurrentInteractable(targetItem.gameObject, string.Format(targetItem.ItemPrompt, itemName));
                return;
            }

            Puzzle_Switch puzzle = hit.collider.GetComponentInParent<Puzzle_Switch>();
            if (puzzle != null)
            {
                SetCurrentInteractable(puzzle.gameObject, puzzle.PuzzlePrompt);
                return;
            }

            DoorController door = hit.collider.GetComponentInParent<DoorController>();
            if (door != null)
            {
                string text;
                if (door.TryGetComponent(out KeyDoor keyDoor) && keyDoor.HasCorrectKey()) text = keyDoor.KeyTextPrompt;
                else text = door.IsLocked ? door.LockTextPrompt : door.InteractablePrompt;

                SetCurrentInteractable(door.gameObject, text);
                return;
            }
        }
        ClearInteractable();
    }

    void InteractInput()
    {
        if (currentInteractable == null || !Input.GetKeyDown(KeyCode.E)) return;

        if (currentInteractable.TryGetComponent(out Item item)) item.PickUp();
        else if (currentInteractable.TryGetComponent(out Puzzle_Switch puzzle)) puzzle.Interact();
        else if (currentInteractable.TryGetComponent(out DoorController door))
        {
            if (door.TryGetComponent(out KeyDoor keyDoor))
            {
                ItemData heldItem = EquipmentManager.Instance.CurrentEquippedItem;
                keyDoor.TryUnlock(heldItem, transform.position);
            }
            else door.Interact(transform.position);
        }
        ClearInteractable();
    }

    void SetCurrentInteractable(GameObject obj, string prompt)
    {
        currentInteractable = obj;
        if (interactPrompt != null) interactPrompt.SetActive(true);
        if (promptText != null) promptText.text = prompt;
    }

    void ClearInteractable()
    {
        currentInteractable = null;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}