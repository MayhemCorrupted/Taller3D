using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] float mouseSensitivity = 5;
    [SerializeField] Transform orientation;
    float xRotation;
    float yRotation;
    Vector2 sensitivity;

    [Header("Head Bob Settings")]
    [SerializeField] CharacterController playerMovement;
    [SerializeField] float bobSpeed = 6;
    [SerializeField] float bobAmountY = 0.05f;
    [SerializeField] float bobAmountX = 0.02f;
    [SerializeField] float minBobSpeed = 0.5f;
    Vector3 originalPos;
    float bobTimer = 0;

    private void Awake()
    {
        originalPos = transform.localPosition; 
        sensitivity = Vector2.one * mouseSensitivity;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Orientation();
        HeadBobbing();
    }
    void Orientation()
    {
        float MouseX = Input.GetAxis("Mouse X") * (sensitivity.x * 10) * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * (sensitivity.y * 10) * Time.deltaTime;

        yRotation += MouseX;
        xRotation -= MouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    void HeadBobbing()
    {
        float speed = playerMovement.velocity.magnitude;

        if (speed > minBobSpeed)
        {
            bobTimer += Time.deltaTime * bobSpeed;

            float bobX = Mathf.Sin(bobTimer * 0.5f) * bobAmountX;
            float bobY = Mathf.Sin(bobTimer) * bobAmountY;

            transform.localPosition = new Vector3(originalPos.x + bobX, originalPos.y + bobY, originalPos.z);
        }
        else
        {
            bobTimer = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * 5f);
        }
    }
}
