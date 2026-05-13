using UnityEngine;
using TMPro;

public class Player_Interactor : MonoBehaviour
{
    [Header("Prompt Formats")]
    [SerializeField] string itemPromptFormat = "[E] Recoger {0}";
    [SerializeField] string doorPromptFormat = "[E] Abrir / Cerrar";
    [SerializeField] string switchPromptFormat = "[E] Interactuar";
    [SerializeField] string useKeyFormat = "[E] Usar llave";
    [SerializeField] string lockedDoorFormat = "Cerrado";

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
        if (Physics.Raycast(interactPoint.position, interactPoint.forward, out RaycastHit hit, interactRange, interactLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.TryGetComponent(out Item item) || hit.collider.GetComponentInParent<Item>())
            {
                Item targetItem = item != null ? item : hit.collider.GetComponentInParent<Item>();
                SetCurrentInteractable(targetItem.gameObject, string.Format(itemPromptFormat, targetItem.itemData.itemName));
                return;
            }

            if (hitObject.TryGetComponent(out Puzzle_Switch puzzle) || hit.collider.GetComponentInParent<Puzzle_Switch>())
            {
                SetCurrentInteractable((puzzle != null ? puzzle : hit.collider.GetComponentInParent<Puzzle_Switch>()).gameObject, switchPromptFormat);
                return;
            }
            DoorController door = hit.collider.GetComponentInParent<DoorController>();
            if (door != null)
            {
                string text;
                if (door.TryGetComponent(out KeyDoor keyDoor) && keyDoor.HasCorrectKey()) text = useKeyFormat;
                else text = door.IsLocked ? lockedDoorFormat : doorPromptFormat;

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