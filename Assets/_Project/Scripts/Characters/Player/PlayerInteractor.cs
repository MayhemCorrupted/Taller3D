using UnityEngine;
using TMPro;
public interface IInteractable
{
    string GetTextInteract();
    void Interact(Transform interactorTransform);
}
public class PlayerInteractor : MonoBehaviour
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
           IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
           if (interactable != null)
            {
                SetCurrentInteractable(interactable, interactable.GetTextInteract());
                return;
            }
        }
        ClearInteractable();
    }

    void InteractInput()
    {
        if (currentInteractable == null || !Input.GetKeyDown(KeyCode.E)) return;
        currentInteractable.Interact(transform);
        ClearInteractable();
    }

    void SetCurrentInteractable(IInteractable interactable, string interactText)
    {
        currentInteractable = interactable;
        if (interactPrompt != null) interactPrompt.SetActive(true);
        if (promptText != null) promptText.text = interactText;
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