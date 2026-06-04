using TMPro;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] float interactRange = 1f;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] LayerMask obstructLayer;
    [SerializeField] Transform interactPoint;

    [Header("Interaction UI")]
    [SerializeField] GameObject interactPrompt;
    [SerializeField] TextMeshProUGUI promptText;

    [Header("Gizmos Debug")]
    [SerializeField] bool showGizmos = true;
    [SerializeField] Color noHitColor = Color.green;
    [SerializeField] Color hitColor = Color.red;
    [SerializeField] float hitIndicatorRadius = 0.1f;

    IInteractable currentInteractable;
    public float InteractRange => interactRange;

    void Update()
    {
        if (UserInterfaceManager.Instance != null && UserInterfaceManager.Instance.IsAnyPanelOpen())
        {
            ClearInteractable();
            return;
        }

        if (Cursor.visible) return;

        DetectInteractable();
        HandleInput();
    }

    void DetectInteractable()
    {
        if (interactPoint == null)
        {
            return;
        }

        float effectiveRange = interactRange;

        if (Physics.Raycast(interactPoint.position, interactPoint.forward,
            out RaycastHit obstruction, interactRange, obstructLayer))
        {
            effectiveRange = obstruction.distance + 0.01f;
        }

        if (Physics.Raycast(interactPoint.position, interactPoint.forward,
            out RaycastHit hit, Mathf.Min(effectiveRange, interactRange), interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                SetCurrentInteractable(interactable, interactable.GetTextInteract());
                return;
            }
        }

        ClearInteractable();
    }

    void HandleInput()
    {
        if (currentInteractable == null || !Input.GetKeyDown(KeyCode.E)) return;
        currentInteractable.Interact(transform);
        ClearInteractable();
    }

    void SetCurrentInteractable(IInteractable interactable, string text)
    {
        currentInteractable = interactable;
        if (interactPrompt != null) interactPrompt.SetActive(true);
        if (promptText != null) promptText.text = text;
    }

    void ClearInteractable()
    {
        currentInteractable = null;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || interactPoint == null) return;

        Vector3 origin = interactPoint.position;
        Vector3 direction = interactPoint.forward;

        float effectiveRange = interactRange;
        if (Physics.Raycast(origin, direction, out RaycastHit obstruction, interactRange, obstructLayer))
            effectiveRange = obstruction.distance;

        bool hit = Physics.Raycast(origin, direction, out RaycastHit hitInfo, effectiveRange + 0.01f, interactLayer);

        Gizmos.color = hit ? hitColor : noHitColor;
        float dist = hit ? hitInfo.distance : effectiveRange;
        Vector3 end = origin + direction * dist;

        Gizmos.DrawLine(origin, end);
        Gizmos.DrawWireSphere(end, hitIndicatorRadius);
    }
}