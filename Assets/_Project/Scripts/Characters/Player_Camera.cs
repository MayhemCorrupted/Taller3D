using UnityEditor;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] float mouseSensitivity = 5f;
    [SerializeField] Transform orientation;
    float xRotation;
    float yRotation;
    Vector2 sensitivity;
    private void Awake()
    {
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
}
