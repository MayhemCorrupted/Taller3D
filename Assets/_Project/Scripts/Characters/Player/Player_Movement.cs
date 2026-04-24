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
        if (Input.GetKeyDown(flyModeKey) && playerCtrl.isGrounded)
        {
            flying = !flying;
            if (flying) verticalVelocity = flyImpulse;
        }
        if(flying) FlyingMovement();
        else GroundMovement();
    }
    void Orientate()
    {
        Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        if (cameraForward != Vector3.zero) orientation.rotation = Quaternion.LookRotation(cameraForward);
    }
    void FlyingMovement()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        verticalVelocity = Mathf.Lerp(verticalVelocity, 0, Time.deltaTime * 5);

        Vector3 direction = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        playerCtrl.Move((direction.normalized * flySpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

        if (playerCtrl.isGrounded) flying = false;
    }
        void GroundMovement()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (playerCtrl.isGrounded) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;

        Vector3 direction = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        playerCtrl.Move((direction.normalized * walkSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
    }
}
