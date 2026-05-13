using UnityEngine;
using System.Collections;
public class DoorController : MonoBehaviour
{
    private Transform hinge;
    [Header("Door Settings")]
    [SerializeField] float openAngle = 90f;
    [SerializeField] float openDuration = 2f;
    [Header("Direction Restrictions")]
    [SerializeField] bool revertDirection = false;
    [SerializeField][Range(-1, 1)] int forcedDirection = 0;
    [Header("Lock Toggle")]
    [SerializeField] bool lockDoor = false;
    private int lastOpenSide = 1;
    private bool isOpen = false;
    private bool isMoving = false;
    private Quaternion closedRotation;
    public bool IsLocked => lockDoor;
    public bool IsOpen => isOpen;
    public bool IsMoving => isMoving;
    private void Awake()
    {
        hinge = transform.GetChild(0);
        closedRotation = hinge.localRotation;
    }
    public void Interact(Vector3 playerPosition)
    {
        if (IsLocked || isMoving) return;

        StopAllCoroutines();

        if (!isOpen)
        {
            int side = CalculateSide(playerPosition);
            lastOpenSide = side;
            float targetAngle = side * openAngle;
            StartCoroutine(MoveDoor(targetAngle));
        }
        else
        {
            StartCoroutine(MoveDoor(0));
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

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / openDuration);
            hinge.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        hinge.localRotation = endRot;
        isOpen = (targetAngle != 0f);
        isMoving = false;
    }
    public void UnlockDoor() => lockDoor = false;
    public void LockDoor() => lockDoor = true;
    public void ForceClose()
    {
        if (isMoving) return;
        StopAllCoroutines();
        StartCoroutine(MoveDoor(0));
    }
    public void ForceOpen(int side = 1)
    {
        if (isMoving) return;
        StopAllCoroutines();
        lastOpenSide = side;
        float targetAngle = side * openAngle;
        StartCoroutine(MoveDoor(targetAngle));
    }
}
