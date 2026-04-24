using Unity.Cinemachine;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class Player_Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] int mouseSensitivity = 100;
    CinemachineCamera playerCamera;
    float currrentSensibility;
    CharacterController playerMovement;
    Vector3 originalPos;
    float bobTimer = 0;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = GetComponentInChildren<CinemachineCamera>();
        originalPos = playerCamera.transform.localPosition;
        currrentSensibility = mouseSensitivity;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        GetSensibility();
    }
    void GetSensibility()
    {
        var axisController = GetComponentInChildren<CinemachineInputAxisController>();
        
        foreach (var c in axisController.Controllers)
        {
            if (c.Name == "Look X (Pan)") c.Input.LegacyGain = mouseSensitivity;
            if (c.Name == "Look Y (Tilt)") c.Input.LegacyGain = -mouseSensitivity;    
        }

    }
}
