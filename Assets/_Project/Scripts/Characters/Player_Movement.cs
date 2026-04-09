using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 15;
    [SerializeField] float gravity = -10;
    [SerializeField] Transform orientation;
    [SerializeField] bool flyMode = false;
    CharacterController controller;
    Vector3 movement;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        Move();
    }
    void Move()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        if (controller.isGrounded) movement.y = -2f;
        else movement.y += gravity * Time.deltaTime;

        Vector3 direction = orientation.forward * movement.z + orientation.right * movement.x;

        if (flyMode)
        {
            Vector3 dirY = direction + orientation.up * movement.y;
        }

        controller.Move((direction.normalized * walkSpeed + Vector3.up * movement.y) * Time.deltaTime);

    }
}
