using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] int mouseSensitivity = 100;
    [SerializeField] Transform cameraTarget;
    CinemachineCamera playerCamera;
    CinemachineInputAxisController axisController;
    CharacterController playerMovement;
    Vector3 originalPos;

    [Space(5)]
    [Header("Camera Bobbing - General")]
    [SerializeField] float minBobSpeed = 0.18f;
    [SerializeField] float returnSpeed = 0.2f;
    [SerializeField] float idleReturnSpeed = 0.1f;

    [Space(5)]
    [Header("Camera Bobbing - Walking")]
    [SerializeField] float walkBobSpeed = 6;
    [SerializeField] float walkBobAmountX = 0.2f;
    [SerializeField] float walkBobAmountY = 0.5f;

    [Space(5)]
    [Header("Camera Bobbing - Stairs")]
    [SerializeField] float stairBobSpeed = 6;
    [SerializeField] float stairBobAmountX = 0.1f;
    [SerializeField] float stairBobAmountY = 0.3f;
    [SerializeField] float stairDetectThreshold = 0.1f;

    [Space(5)]
    [Header("Camera Bobbing - Flying")]
    [SerializeField] float flyDampingSpeed = 3f;
    [SerializeField] float flyBobAmount = 0.2f;
    [SerializeField] float flyBobSpeed = 2f;

    float bobTimer = 0;
    Vector3 targetBobPos;
    Vector3 lastPosition;
    Vector3 smoothVelocity;

    enum MovementState { idle, walking, stairs, flying }
    MovementState currentState;

    private void Awake()
    {
        playerMovement = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<CinemachineCamera>();
        axisController = GetComponentInChildren<CinemachineInputAxisController>();
        originalPos = cameraTarget.localPosition;
        targetBobPos = originalPos;
        lastPosition = transform.position;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        StateDetect();
        HeadBobbing();
        SensibilityCinemachine();
        lastPosition = transform.position;
    }

    void StateDetect()
    {
        float speed = new Vector3(playerMovement.velocity.x, 0, playerMovement.velocity.z).magnitude;

        if (!playerMovement.isGrounded && playerMovement.velocity.y > 0.5f)
        {
            currentState = MovementState.flying;
            return;
        }

        float verticalDelta = Mathf.Abs(transform.position.y - lastPosition.y);
        if (playerMovement.isGrounded && verticalDelta > stairDetectThreshold && speed > minBobSpeed)
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
            case MovementState.walking: WalkBob(); break;
            case MovementState.stairs: StairBob(); break;
            case MovementState.flying: FlyBob(); break;
            case MovementState.idle: IdleBob(); break;
        }

        if (currentState != MovementState.idle)
        {
            cameraTarget.localPosition = Vector3.Lerp(
                cameraTarget.localPosition,
                targetBobPos,
                Time.deltaTime * returnSpeed
            );
        }
    }

    void WalkBob()
    {
        bobTimer += Time.deltaTime * walkBobSpeed;
        targetBobPos = new Vector3(
            originalPos.x + Mathf.Sin(bobTimer * 0.5f) * walkBobAmountX,
            originalPos.y + Mathf.Sin(bobTimer) * walkBobAmountY,
            originalPos.z
        );
    }

    void StairBob()
    {
        bobTimer += Time.deltaTime * stairBobSpeed;
        targetBobPos = new Vector3(
            originalPos.x + Mathf.Sin(bobTimer * 0.5f) * stairBobAmountX,
            originalPos.y + Mathf.Abs(Mathf.Sin(bobTimer)) * stairBobAmountY,
            originalPos.z
        );
    }

    void FlyBob()
    {
        bobTimer += Time.deltaTime * flyBobSpeed;
        targetBobPos = new Vector3(
            originalPos.x,
            originalPos.y + Mathf.Sin(bobTimer) * flyBobAmount,
            originalPos.z
        );

        cameraTarget.localPosition = Vector3.Lerp(
            cameraTarget.localPosition,
            targetBobPos,
            Time.deltaTime * flyDampingSpeed
        );
    }

    void IdleBob()
    {
        bobTimer = 0;
        targetBobPos = originalPos;

        cameraTarget.localPosition = Vector3.SmoothDamp(
            cameraTarget.localPosition,
            originalPos,
            ref smoothVelocity,
            idleReturnSpeed
        );
    }

    void SensibilityCinemachine()
    {
        foreach (var c in axisController.Controllers)
        {
            if (c.Name == "Look X (Pan)") c.Input.LegacyGain = mouseSensitivity;
            if (c.Name == "Look Y (Tilt)") c.Input.LegacyGain = -mouseSensitivity;
        }
    }
}