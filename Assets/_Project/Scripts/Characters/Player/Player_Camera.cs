using Unity.Cinemachine;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class Player_Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] float mouseSensitivity = 10;
    CinemachineCamera playerCamera;
    float currrentSensibility;
    CinemachineInputAxisController controller;
    [Header("Head Bob Settings")]
    [SerializeField] float bobSpeed = 6;
    [SerializeField] float bobAmountY = 0.05f;
    [SerializeField] float bobAmountX = 0.02f;
    [SerializeField] float minBobSpeed = 0.5f;
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
        //HeadBobbing();
    }
    void SensibilityUpdate()
    {
    }   
    /*void HeadBobbing()
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
    }*/
}
