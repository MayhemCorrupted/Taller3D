using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class DoorController : MonoBehaviour, IInteractable
{
    KeyDoor keyDoorComponent;
    [Header("Door Settings")]
    [SerializeField] Transform hinge;
    [SerializeField] float openAngle = 90f;
    [SerializeField] float doorSpeed = 2f;
    [Header("Prompt Settings")]
    [SerializeField] string lockTextPrompt = "Closed";
    [SerializeField] string openDoorPrompt = "{key} Open door";
    [SerializeField] string closeDoorPrompt = "{key} Close door";
    [Header("Direction Restrictions")]
    [SerializeField] bool revertDirection = false;
    [SerializeField][Range(-1, 1)] int forcedDirection = 0;
    [Header("Lock Toggle")]
    [SerializeField] bool doorLocked = false;
    int lastOpenSide = 1;
    bool isOpen = false;
    bool isMoving = false;
     Quaternion closedRotation;
    [Header("Events")]
    [SerializeField] UnityEvent OnLockedDoor;
    [SerializeField] UnityEvent OnOpeningDoor;
    [SerializeField] UnityEvent OnClosingDoor;
    public bool IsLocked => doorLocked;
    public string LockTextPrompt => lockTextPrompt;
    public string InteractablePrompt => openDoorPrompt;
    public bool IsOpen => isOpen;
    public bool IsMoving => isMoving;
    public float DoorSpeed { set { doorSpeed = value; } }
    void Awake()
    {
        if (hinge == null)
        {
            if (transform.childCount > 0) hinge = transform.GetChild(0);
            else
            {
                Debug.LogError($"[DoorController] No se asignó un Hinge en {gameObject.name} y tampoco tiene objetos hijos.");
                return;
            }
        }
        closedRotation = hinge.localRotation;
        TryGetComponent(out keyDoorComponent);
    }
    public string GetTextInteract()
    {
        if (isMoving) return "";

        if (keyDoorComponent != null && keyDoorComponent.HasCorrectKey()) return keyDoorComponent.KeyTextPrompt;
        if (doorLocked) return lockTextPrompt;

        return isOpen ? closeDoorPrompt : openDoorPrompt;
    }
    public void Interact(Transform interactorTransform)
    {
        if (isMoving) return;
        if (keyDoorComponent != null)
        {
            ItemData heldItem = EquipmentManager.Instance.CurrentEquippedItem;
            keyDoorComponent.TryUnlock(heldItem, interactorTransform.position);
        }
        else OpenOrCloseDoor(interactorTransform.position);
    }
    public void OpenOrCloseDoor(Vector3 playerPosition)
    {
        if (doorLocked)
        {
            OnLockedDoor?.Invoke();
            return;
        }
        if (isMoving) return;

        StopAllCoroutines();



        if (!isOpen)
        {
            int side = CalculateSide(playerPosition);
            lastOpenSide = side;
            float targetAngle = side * openAngle;
            StartCoroutine(MoveDoor(targetAngle));
            OnOpeningDoor?.Invoke();
        }
        else
        {
            StartCoroutine(MoveDoor(0));
            OnClosingDoor?.Invoke();
        }
    }
    int CalculateSide(Vector3 playerPosition)
    {
        if (forcedDirection != 0) return forcedDirection;

        Vector3 localPlayerPos = transform.InverseTransformPoint(playerPosition);
        int side = localPlayerPos.x >= 0 ? 1 : -1;

        if (revertDirection) side *= -1;

        return side;
    }
    IEnumerator MoveDoor(float targetAngle)
    {
        isMoving = true;
        Quaternion startRot = hinge.localRotation;
        Quaternion endRot = closedRotation * Quaternion.Euler(0, targetAngle, 0);
        float elapsed = 0f;

        while (elapsed < doorSpeed)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / doorSpeed);
            hinge.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        hinge.localRotation = endRot;
        isOpen = (targetAngle != 0f);
        isMoving = false;
    }
    public void UnlockDoor() => doorLocked = false;
    public void LockDoor() => doorLocked = true;
    public void ForceClose()
    {
        if (isMoving) return;
        StopAllCoroutines();
        StartCoroutine(MoveDoor(0));
        OnClosingDoor?.Invoke();
    }
    public void ForceOpen(int side = 1)
    {
        if (isMoving) return;
        StopAllCoroutines();
        lastOpenSide = side;
        float targetAngle = side * openAngle;
        StartCoroutine(MoveDoor(targetAngle));
        OnOpeningDoor?.Invoke();
    }
}
