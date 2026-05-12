using UnityEngine;
using System.Collections;
public class DoorController : MonoBehaviour
{
    Transform hinge;
    [Header("Door Settings")]
    [SerializeField] float openAngle = 90;
    [SerializeField] float openDuration = 2;
    [Header("Direction restrictions")]
    [SerializeField] bool revertDirection = false;
    [Tooltip("0 para automático, -1 para sentido antihorario, 1 para sentido horario")]
    [SerializeField][Range(-1,1)] int forcedDirection = 1;
    [Header("Lock Toggle")]
    [SerializeField] bool LockDoor = false;
    bool isOpen = false;    
    bool isMoving = false;
    Quaternion closedRotation;
    public bool IsLocked => LockDoor;
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
            float side = CalculateSide(playerPosition);
            float targetAngle = side * openAngle;
            StartCoroutine(MoveDoor(targetAngle));
        }
        else StartCoroutine(MoveDoor(0));
    }
    float CalculateSide(Vector3 playerPosition)
    {
        if (forcedDirection != 0) return forcedDirection;
        Vector3 localPlayerPos = transform.InverseTransformPoint(playerPosition);
        float side = localPlayerPos.z    >= 0 ? 1 : -1;
        if (revertDirection) side *= -1;
        return side;

    }
    IEnumerator MoveDoor(float targetAngle)
    {
        isMoving = true;
        Quaternion startRot = hinge.localRotation;
        Quaternion endRot = closedRotation * Quaternion.Euler(0, targetAngle, 0);
        float elapsed = 0;
        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / openDuration);
            hinge.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
        hinge.localRotation = endRot;
        isOpen = (targetAngle != 0);
        isMoving = false;   
    }
    public void UnlockDoor() => LockDoor = false;
}
