using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float SensitivityX = 175;
    [SerializeField] private float SensitivityY = 175;
    [SerializeField] Transform orientation;
    float xRotation;
    float yRotation;
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
        float MouseX = Input.GetAxis("Mouse X") * SensitivityX * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * SensitivityY * Time.deltaTime;

        yRotation += MouseX;
        xRotation -= MouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
