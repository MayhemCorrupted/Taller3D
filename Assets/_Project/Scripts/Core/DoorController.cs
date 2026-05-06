using UnityEngine;
using System.Collections;
public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] Transform hinge;
    [SerializeField] float openAngle = 90;
    [SerializeField] float openDuration = 2;
    [Header("Direction restrictions")]
    [SerializeField] bool lockDirection = false;
    [Tooltip("-1 para sentido antihorario, 1 para sentido horario")]
    [SerializeField][Range(-1,1)] int forcedDirection = 1;
    [Header("Lock Toggle")]
    [SerializeField] bool LockedDoor = false;
    public bool isLocked => LockedDoor;
    bool isOpen = false;    
    bool isMoving = false;
    public void Interact(Vector3 playerPosition)
    {
        if (isLocked)
        {
            Debug.Log("La puerta está cerrada.");
            return;
        } 
        if (isMoving) return;
        StopAllCoroutines();
        if (!isOpen) StartCoroutine(MoveDoor(CalculateTargetAngle(playerPosition)));
        else StartCoroutine(MoveDoor(0));
    }
    float CalculateTargetAngle(Vector3 playerPosition)
    {
        if (lockDirection) return openAngle * forcedDirection;
        Vector3 doorToPlayer = (playerPosition - hinge.position).normalized;
        float dot = Vector3.Dot(hinge.right, doorToPlayer);
        int side = dot >= 0 ? 1 : -1;
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
    public void UnlockDoor() => LockedDoor = false;
}
