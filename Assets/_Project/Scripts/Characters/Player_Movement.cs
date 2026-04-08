using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 15;
    [SerializeField] float gravity = -10;
    [SerializeField] Transform orientation;
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
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");

        if (controller.isGrounded && movement.y < 0) movement.y = -2f;

        movement.y += gravity * Time.deltaTime;

        Vector3 direction = orientation.forward * movement.z + orientation.right * movement.y;

        controller.Move((movement.normalized * walkSpeed + Vector3.up * movement.y) * Time.deltaTime);
    }
}
