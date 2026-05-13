using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    CharacterController playerCtrl;
    Transform cameraTransform;
    [Header("Movement Settings")]
    [SerializeField] Transform orientation;
    [SerializeField] float walkSpeed = 15;
    [SerializeField] float gravity = -10;
    [Header("FlyMode Settings")]
    [SerializeField] KeyCode flyModeKey = KeyCode.Space;
    [SerializeField] float flySpeed = 10;
    [SerializeField] float flyImpulse = 15;
    bool flying;
    bool canMove = true;
    Vector2 moveInput;
    float verticalVelocity;
    private void Awake()
    {
        playerCtrl = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<CinemachineCamera>().transform;
    }
    void Update()
    {
        Orientate();

        if (!canMove)
        {
            return;
        }
        InputHandle();
        if (flying) FlyingMovement();
        else GroundMovement();
    }
    void InputHandle()
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
        Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        if (cameraForward != Vector3.zero) orientation.rotation = Quaternion.LookRotation(cameraForward);
    }
    void FlyingMovement()
    {
        verticalVelocity = Mathf.Lerp(verticalVelocity, 0, Time.deltaTime * 5);

        Vector3 direction = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        playerCtrl.Move((direction.normalized * flySpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

        if (playerCtrl.isGrounded) flying = false;
    }
    void GroundMovement()
    {
        if (playerCtrl.isGrounded) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;

        Vector3 direction = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        playerCtrl.Move((direction.normalized * walkSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
    }
}
