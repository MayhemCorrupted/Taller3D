using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    CharacterController playerCtrl;
    Transform cameraTransform;

    [Header("Movement Settings")]
    [SerializeField] Transform orientation;
    [SerializeField] float walkSpeed = 15;
    [SerializeField] float gravity = -10;

    [Header("FlyMode Settings")]
    [SerializeField] float flySpeed = 10;
    [SerializeField] float flyImpulse = 15;

    bool flying;
    bool canFly = true;
    bool canMove = true;
    Vector2 moveInput;
    float verticalVelocity;

    public Vector3 Velocity => playerCtrl.velocity;
    public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    public bool IsGrounded => playerCtrl.isGrounded;
    public bool Flying => flying;
    public bool CanFly { get => canFly; set => canFly = value; }

    void Awake()
    {
        playerCtrl = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<CinemachineCamera>().transform;
    }

    void Update()
    {
        Orientate();
        if (!canMove) return;

        InputHandle();
        if (flying && canFly) FlyingMovement();
        else if (!flying || playerCtrl.isGrounded) GroundMovement();
    }

    void InputHandle()
    {
        if (Input.GetKeyDown(InputManager.Instance.FlyKey) && canFly)
        {
            flying = !flying;
            if (flying && playerCtrl.isGrounded) verticalVelocity = flyImpulse;
        }

        float y = GetAxis(InputManager.Instance.ForwardKey, InputManager.Instance.BackwardKey);
        float x = GetAxis(InputManager.Instance.RightKey, InputManager.Instance.LeftKey);
        moveInput = new Vector2(x, y).normalized;
    }

    float GetAxis(KeyCode positiveKey, KeyCode negativeKey)
    {
        float axisValue = 0;
        if (Input.GetKey(positiveKey)) axisValue += 1;
        if (Input.GetKey(negativeKey)) axisValue -= 1;
        return axisValue;
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
        Vector3 cameraForward = cameraTransform.forward;
        if (cameraForward != Vector3.zero)
            orientation.rotation = Quaternion.LookRotation(cameraForward, cameraTransform.up);
    }

    void FlyingMovement()
    {
        Vector3 direction = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
                  
        verticalVelocity = Mathf.Lerp(verticalVelocity, 0f, Time.deltaTime * 5f);

        playerCtrl.Move((direction.normalized * flySpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

        if (playerCtrl.isGrounded) flying = false;
    }

    void GroundMovement()
    {
        if (playerCtrl.isGrounded) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;

        Vector3 forwardOrientate = new Vector3(orientation.forward.x, 0f, orientation.forward.z).normalized;
        Vector3 rightOrientate = new Vector3(orientation.right.x, 0f, orientation.right.z).normalized;

        Vector3 direction = forwardOrientate * moveInput.y + rightOrientate * moveInput.x;
        playerCtrl.Move((direction.normalized * walkSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
    }
}