using UnityEngine;
using TMPro;

public class Player_Interactor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] float interactRange = 1;
    [SerializeField] LayerMask interactLayer, obstructLayer;
    [SerializeField] Transform interactPoint;

    [Header("Interaction UI")]
    [SerializeField] GameObject interactPrompt;
    [SerializeField] TextMeshProUGUI promptText;

    [Header("Gizmos Debug Settings")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color noHitColor = Color.green;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitIndicatorRadius = 0.1f;

    GameObject currentInteractable;
    public float InteractRange => interactRange;

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

        float effectiveRange = interactRange;

        if (Physics.Raycast(interactPoint.position, interactPoint.forward, out RaycastHit obstructionHit, interactRange, obstructLayer))
        {
            effectiveRange = obstructionHit.distance + 0.01f;
        }

        if (Physics.Raycast(interactPoint.position, interactPoint.forward, out RaycastHit hit, Mathf.Min(effectiveRange, interactRange), interactLayer))
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

            Puzzle_PanelCode puzzlePanel = hit.collider.GetComponentInParent<Puzzle_PanelCode>();
            if (puzzlePanel != null)
            {
                SetCurrentInteractable(puzzlePanel.gameObject, puzzlePanel.TextPrompt);
                return;
            }

            DoorController door = hit.collider.GetComponentInParent<DoorController>();
            if (door != null)
            {
                string text;

                if (door.TryGetComponent(out KeyDoor keyDoor) && keyDoor.HasCorrectKey())
                    text = keyDoor.KeyTextPrompt;
                else
                    text = door.IsLocked ? door.LockTextPrompt : door.InteractablePrompt;

                SetCurrentInteractable(door.gameObject, text);
                return;
            }

            if (hit.collider.TryGetComponent(out OpenObjects currentDrawer))
            {
                SetCurrentInteractable(currentDrawer.gameObject, currentDrawer.interactPrompt);
                return;
            }
        }

        ClearInteractable();
    }

    private void InteractInput()
    {
        if (currentInteractable == null || !Input.GetKeyDown(KeyCode.E)) return;

        if (currentInteractable.TryGetComponent(out Item item)) item.PickUp();
        else if (currentInteractable.TryGetComponent(out Puzzle_Switch puzzle)) puzzle.Interact();
        else if (currentInteractable.TryGetComponent(out Puzzle_PanelCode puzzlePanel)) puzzlePanel.Interact();
        else if (currentInteractable.TryGetComponent(out OpenObjects drawer)) drawer.Interact();
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

    private void SetCurrentInteractable(GameObject obj, string prompt)
    {
        currentInteractable = obj;
        if (interactPrompt != null) interactPrompt.SetActive(true);
        if (promptText != null) promptText.text = prompt;
    }

    private void ClearInteractable()
    {
        currentInteractable = null;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || interactPoint == null) return;

        Vector3 origin = interactPoint.position;
        Vector3 direction = interactPoint.forward;

        float effectiveRange = interactRange;
        if (Physics.Raycast(origin, direction, out RaycastHit obstructionHit, interactRange, obstructLayer))
        {
            effectiveRange = obstructionHit.distance;
        }

        bool isHittingInteractable = Physics.Raycast(origin, direction, out RaycastHit hit, effectiveRange + 0.01f, interactLayer);

        Gizmos.color = isHittingInteractable ? hitColor : noHitColor;

        float finalGizmoDistance = isHittingInteractable ? hit.distance : effectiveRange;
        Vector3 endPoint = origin + (direction * finalGizmoDistance);

        Gizmos.DrawLine(origin, endPoint);
        Gizmos.DrawWireSphere(endPoint, hitIndicatorRadius);
    }
}