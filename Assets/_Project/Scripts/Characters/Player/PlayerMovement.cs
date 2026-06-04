using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] Transform orientation;
    [SerializeField] float walkSpeed = 15f;
    [SerializeField] float gravity = -10f;

    [Header("Fly Mode (Debug)")]
    [SerializeField] KeyCode flyModeKey = KeyCode.Space;
    [SerializeField] float flySpeed = 10f;
    [SerializeField] float flyImpulse = 15f;

    CharacterController playerCtrl;
    Transform cameraTransform;

    bool flying;
    bool canMove = true;
    Vector2 moveInput;
    float verticalVelocity;

    void Awake()
    {
        playerCtrl = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<CinemachineCamera>().transform;
    }

    void Update()
    {
        Orientate();

        if (!canMove) return;

        GatherInput();

        if (flying) FlyMovement();
        else GroundMovement();
    }

    void GatherInput()
    {
        if (Input.GetKeyDown(flyModeKey) && playerCtrl.isGrounded)
        {
            flying = !flying;
            if (flying) verticalVelocity = flyImpulse;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    public void CanMove(bool state)
    {
        canMove = state;
        if (!canMove)
        {
            moveInput = Vector2.zero;
            verticalVelocity = 0;
        }
    }

    void Orientate()
    {
        if (cameraTransform == null || orientation == null) return;

        Vector3 forward = cameraTransform.forward;
        if (forward != Vector3.zero)
            orientation.rotation = Quaternion.LookRotation(forward, cameraTransform.up);
    }

    void GroundMovement()
    {
        if (playerCtrl.isGrounded) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;

        Vector3 fwd = new Vector3(orientation.forward.x, 0f, orientation.forward.z).normalized;
        Vector3 right = new Vector3(orientation.right.x, 0f, orientation.right.z).normalized;

        Vector3 dir = fwd * moveInput.y + right * moveInput.x;
        playerCtrl.Move((dir.normalized * walkSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    void FlyMovement()
    {
        verticalVelocity = Mathf.Lerp(verticalVelocity, 0f, Time.deltaTime * 5f);

        Vector3 dir = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        playerCtrl.Move((dir.normalized * flySpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

        if (playerCtrl.isGrounded) flying = false;
    }
}