using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCamera : MonoBehaviour
{
    [System.Serializable]
    public struct BobbingProfile
    {
        [Tooltip("Intensidad del movimiento de la cámara.")]
        public float amplitude;
        [Tooltip("Velocidad del movimiento de la cámara.")]
        public float frequency;
    }

    enum MovementState { Idle, Walking, Stairs, Flying }

    [Header("Camera Settings")]
    [SerializeField] int mouseSensitivity = 100;
    [SerializeField] CinemachineBasicMultiChannelPerlin noiseComponent;

    [Header("Detection Settings")]
    [SerializeField] float minBobSpeed = 0.18f;
    [SerializeField] float stairDetectThreshold = 0.1f;
    [SerializeField] float profileTransitionSpeed = 8f;

    [Header("Movement Profiles")]
    [SerializeField] BobbingProfile idleProfile = new() { amplitude = 0.2f, frequency = 0.5f };
    [SerializeField] BobbingProfile walkProfile = new() { amplitude = 0.7f, frequency = 1.5f };
    [SerializeField] BobbingProfile stairsProfile = new() { amplitude = 0.9f, frequency = 2.0f };
    [SerializeField] BobbingProfile flyingProfile = new() { amplitude = 0.4f, frequency = 0.8f };

    CinemachineInputAxisController axisController;
    CharacterController charController;

    MovementState currentState;
    Vector3 lastPosition;
    bool isCameraLocked;

    float targetAmplitude;
    float targetFrequency;

    void Awake()
    {
        charController = GetComponent<CharacterController>();
        axisController = GetComponentInChildren<CinemachineInputAxisController>();
        lastPosition = transform.position;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        UpdateSensitivity();
        DetectMovementState();
        ApplyHeadBobbing();
        lastPosition = transform.position;
    }

    void DetectMovementState()
    {
        if (!charController.isGrounded)
        {
            currentState = MovementState.Flying;
            return;
        }

        float speed = new Vector3(charController.velocity.x, 0, charController.velocity.z).magnitude;
        float verticalDelta = Mathf.Abs(transform.position.y - lastPosition.y);

        if (verticalDelta > stairDetectThreshold && speed < minBobSpeed)
        {
            currentState = MovementState.Stairs;
            return;
        }

        currentState = speed > minBobSpeed ? MovementState.Walking : MovementState.Idle;
    }

    void ApplyHeadBobbing()
    {
        if (noiseComponent == null) return;

        if (isCameraLocked)
        {
            noiseComponent.AmplitudeGain = 0;
            noiseComponent.FrequencyGain = 0;
            return;
        }

        BobbingProfile profile = currentState switch
        {
            MovementState.Walking => walkProfile,
            MovementState.Stairs => stairsProfile,
            MovementState.Flying => flyingProfile,
            _ => idleProfile
        };

        targetAmplitude = profile.amplitude;
        targetFrequency = profile.frequency;

        noiseComponent.AmplitudeGain = Mathf.Lerp(noiseComponent.AmplitudeGain, targetAmplitude, Time.deltaTime * profileTransitionSpeed);
        noiseComponent.FrequencyGain = Mathf.Lerp(noiseComponent.FrequencyGain, targetFrequency, Time.deltaTime * profileTransitionSpeed);
    }

    void UpdateSensitivity()
    {
        if (axisController == null) return;

        foreach (var c in axisController.Controllers)
        {
            if (isCameraLocked)
            {
                c.Input.LegacyGain = 0;
            }
            else
            {
                if (c.Name == "Look X (Pan)") c.Input.LegacyGain = mouseSensitivity;
                if (c.Name == "Look Y (Tilt)") c.Input.LegacyGain = -mouseSensitivity;
            }
        }
    }

    public void LockCamera(bool lockCam) => isCameraLocked = lockCam;
}