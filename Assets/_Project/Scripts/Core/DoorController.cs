using UnityEngine;
using System.Collections;
public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] Transform hinge;
    [SerializeField] float openAngle = 90;
    [SerializeField] float openDuration = 2;
    [Header("Direction restrictions")]
    [SerializeField] bool revertDirection = false;
    [Tooltip("0 para automático, -1 para sentido antihorario, 1 para sentido horario")]
    [SerializeField][Range(-1,1)] int forcedDirection = 1;
    [Header("Lock Toggle")]
    [SerializeField] bool LockDoor = false;
    public bool IsLocked => LockDoor;
    bool isOpen = false;    
    bool isMoving = false;
    public void Interact(Vector3 playerPosition)
    {
        if (IsLocked)
        {
            Debug.Log("La puerta está cerrada.");
            return;
        } 
        if (isMoving) return;
        StopAllCoroutines();
        if (!isOpen)
        {
            float target = CalculateTargetAngle(playerPosition);
            StartCoroutine(MoveDoor(target));
        }
        else StartCoroutine(MoveDoor(0));
    }
    float CalculateTargetAngle(Vector3 playerPosition)
    {
        if (forcedDirection != 0) return openAngle * forcedDirection;
        Vector3 doorToPlayer = (playerPosition - hinge.position).normalized;
        float dot = Vector3.Dot(hinge.forward, doorToPlayer);
        int side = dot >= 0 ? 1 : -1;
        if (revertDirection) side *= -1;
        return openAngle * side;
    }
    IEnumerator MoveDoor(float targetAngle)
    {
        isMoving = true;
        Quaternion startRot = hinge.localRotation;
        Quaternion endRot = Quaternion.Euler(0, targetAngle, 0);
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
