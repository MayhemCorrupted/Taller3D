using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] int mouseSensitivity = 100;
    [SerializeField] Transform cameraTarget;
    CinemachineInputAxisController axisController;
    CharacterController playerMovement;
    Vector3 originalPos;

    [Space(2)]
    [Header("Camera Bobbing - General")]
    [SerializeField] float minBobSpeed = 0.18f;
    [SerializeField] float returnSpeed = 5;

    [Space(2)]
    [Header("Camera Bobbing - Idle")]
    [SerializeField] float idleBobSpeed = 1f;
    [SerializeField] float idleBobAmountX = 0.02f;
    [SerializeField] float idleBobAmountY = 0.04f;

    [Space(2)]
    [Header("Camera Bobbing - Walking")]
    [SerializeField] float walkBobSpeed = 10;
    [SerializeField] float walkBobAmountX = 0.05f;
    [SerializeField] float walkBobAmountY = 0.05f;

    [Space(2)]
    [Header("Camera Bobbing - Stairs")]
    [SerializeField] float stairBobSpeed = 12;
    [SerializeField] float stairBobAmountX = 0.02f;
    [SerializeField] float stairBobAmountY = 0.08f;
    [SerializeField] float stairDetectThreshold = 0.1f;

    [Space(2)]
    [Header("Camera Bobbing - Flying")]
    [SerializeField] float flyBobSpeed = 2f;
    [SerializeField] float flyBobAmount = 0.2f;
    [SerializeField] float flyInertiaMultiplier = 3f;

    float bobTimer = 0;
    Vector3 targetBobPos;
    Vector3 lastPosition;
    bool isCameraLocked = false;
    enum MovementState { idle, walking, stairs, flying }
    MovementState currentState;
    private void Awake()
    {
        playerMovement = GetComponent<CharacterController>();
        axisController = GetComponentInChildren<CinemachineInputAxisController>();
        originalPos = cameraTarget.localPosition;
        targetBobPos = originalPos;
        lastPosition = transform.position;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        SensibilityCinemachine();
        lastPosition = transform.position;
    }

    void StateDetect()
    {
        if (!playerMovement.isGrounded)
        {
            currentState = MovementState.flying;
            return;
        }
        float speed = new Vector3  (playerMovement.velocity.x, 0, playerMovement.velocity.z).magnitude;
        float verticalDelta = Mathf.Abs (transform.position.y - lastPosition.y);
        if (verticalDelta > stairDetectThreshold && speed < minBobSpeed)
        {
            currentState = MovementState.stairs;
            return;
        }
        currentState = speed > minBobSpeed ? MovementState.walking : MovementState.idle;
    }

    void HeadBobbing()
    {
        switch (currentState)
        {
            case MovementState.idle: IdleBob(); break;
            case MovementState.walking: WalkBob(); break;
            case MovementState.stairs: StairBob(); break;
            case MovementState.flying: FlyBob(); break;
        }
        cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, targetBobPos, Time.deltaTime * returnSpeed);
    }
    void IdleBob()
    {
        bobTimer += Time.deltaTime * idleBobSpeed;
        targetBobPos = new Vector3(
            originalPos.x + Mathf.Cos(bobTimer * 0.5f) * idleBobAmountX,
            originalPos.y + Mathf.Sin(bobTimer) * idleBobAmountY,
            originalPos.z
        );  
    }
    void WalkBob()
    {
        bobTimer += Time.deltaTime * walkBobSpeed;
        targetBobPos = new Vector3(
            originalPos.x + Mathf.Cos(bobTimer * 0.5f) * walkBobAmountX,
            originalPos.y + Mathf.Sin(bobTimer) * walkBobAmountY,
            originalPos.z
        );
    }

    void StairBob()
    {
        bobTimer += Time.deltaTime * stairBobSpeed;
        targetBobPos = new Vector3(
            originalPos.x + Mathf.Cos(bobTimer * 0.5f) * stairBobAmountX,
            originalPos.y + Mathf.Abs(Mathf.Sin(bobTimer)) * stairBobAmountY,
            originalPos.z
        );
    }

    void FlyBob()
    {
        bobTimer += Time.deltaTime * flyBobSpeed;
        float inertia = -playerMovement.velocity.y * flyInertiaMultiplier;
        inertia = Mathf.Clamp(inertia, -0.4f, 0.4f);
        targetBobPos = new Vector3(
            originalPos.x + Mathf.Cos(bobTimer * 0.5f) * (flyBobAmount * 1.5f),
            originalPos.y +  Mathf.Sin(bobTimer) * flyBobAmount * inertia,
            originalPos.z
        );
    }

    void SensibilityCinemachine()
    {
        foreach (var c in axisController.Controllers)
        {
            if (isCameraLocked) c.Input.LegacyGain = 0;
            else
            {
                  if (c.Name == "Look X (Pan)") c.Input.LegacyGain = mouseSensitivity;
                  if (c.Name == "Look Y (Tilt)") c.Input.LegacyGain = -mouseSensitivity;
            }
        }
    }
    public void LockCamera(bool lockCam) => isCameraLocked = lockCam;
}