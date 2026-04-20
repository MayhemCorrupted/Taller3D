using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 15;
    [SerializeField] float gravity = -10;
    [SerializeField] Transform orientation;
    CharacterController controller;
    [Header("FlyMode Settings")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] KeyCode flyModeKey = KeyCode.Space;
    [SerializeField] float flySpeed = 10f;
    bool flying;
    Vector3 movement;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        if(Input.GetKeyDown(flyModeKey)) flying = !flying;
        if (flying) Fly();
        else Move();
    }
    void Fly()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        Vector3 direction = cameraTransform.forward * movement.z + cameraTransform.right * movement.x;
        controller.Move((direction.normalized * flySpeed) * Time.deltaTime);

    }
        void Move()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        if (controller.isGrounded) movement.y = -2f;
        else movement.y += gravity * Time.deltaTime;

        Vector3 direction = orientation.forward * movement.z + orientation.right * movement.x;
        controller.Move((direction.normalized * walkSpeed + Vector3.up * movement.y) * Time.deltaTime);
    }
}
