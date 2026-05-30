using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCamera : MonoBehaviour
{
    [System.Serializable]
    public struct BobbingProfile
    {
        [Tooltip("Intensidad o fuerza del movimiento de la cámara.")]
        public float amplitude;
        [Tooltip("Velocidad o rapidez del movimiento de la cámara.")]
        public float frequency;
    }

    enum MovementState { Idle, Walking, Stairs, Flying }

    [Header("Camera Settings")]
    [SerializeField] private int mouseSensitivity = 100;
    [Tooltip("Referencia al componente de ruido de la Cinemachine Camera.")]
    [SerializeField] private CinemachineBasicMultiChannelPerlin noiseComponent;

    private CinemachineInputAxisController axisController;
    private CharacterController playerMovement;

    [Header("Detection Settings")]
    [SerializeField] private float minBobSpeed = 0.18f;
    [SerializeField] private float stairDetectThreshold = 0.1f;
    [Tooltip("Velocidad de transición suave entre perfiles de movimiento.")]
    [SerializeField] private float profileTransitionSpeed = 8f;

    [Header("Movement Profiles Configuration")]
    [SerializeField] private BobbingProfile idleProfile = new(){ amplitude = 0.2f, frequency = 0.5f };
    [SerializeField] private BobbingProfile walkProfile = new() { amplitude = 0.7f, frequency = 1.5f };
    [SerializeField] private BobbingProfile stairsProfile = new() { amplitude = 0.9f, frequency = 2.0f };
    [SerializeField] private BobbingProfile flyingProfile = new() { amplitude = 0.4f, frequency = 0.8f };

    private MovementState currentState;
    private Vector3 lastPosition;
    private bool isCameraLocked = false;

    private float targetAmplitude;
    private float targetFrequency;

    private void Awake()
    {
        playerMovement = GetComponent<CharacterController>();
        axisController = GetComponentInChildren<CinemachineInputAxisController>();

        lastPosition = transform.position;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        SensibilityCinemachine();
        StateDetect();
        ApplyHeadBobbing();

        lastPosition = transform.position;
    }

    private void StateDetect()
    {
        if (!playerMovement.isGrounded)
        {
            currentState = MovementState.Flying;
            return;
        }

        float speed = new Vector3(playerMovement.velocity.x, 0, playerMovement.velocity.z).magnitude;
        float verticalDelta = Mathf.Abs(transform.position.y - lastPosition.y);

        if (verticalDelta > stairDetectThreshold && speed < minBobSpeed)
        {
            currentState = MovementState.Stairs;
            return;
        }

        currentState = speed > minBobSpeed ? MovementState.Walking : MovementState.Idle;
    }

    private void ApplyHeadBobbing()
    {
        if (noiseComponent == null) return;

        if (isCameraLocked)
        {
            noiseComponent.AmplitudeGain = 0;
            noiseComponent.FrequencyGain = 0;
            return;
        }

        switch (currentState)
        {
            case MovementState.Idle:
                targetAmplitude = idleProfile.amplitude;
                targetFrequency = idleProfile.frequency;
                break;
            case MovementState.Walking:
                targetAmplitude = walkProfile.amplitude;
                targetFrequency = walkProfile.frequency;
                break;
            case MovementState.Stairs:
                targetAmplitude = stairsProfile.amplitude;
                targetFrequency = stairsProfile.frequency;
                break;
            case MovementState.Flying:
                targetAmplitude = flyingProfile.amplitude;
                targetFrequency = flyingProfile.frequency;
                break;
        }

        noiseComponent.AmplitudeGain = Mathf.Lerp(noiseComponent.AmplitudeGain, targetAmplitude, Time.deltaTime * profileTransitionSpeed);
        noiseComponent.FrequencyGain = Mathf.Lerp(noiseComponent.FrequencyGain, targetFrequency, Time.deltaTime * profileTransitionSpeed);
    }
    private void SensibilityCinemachine()
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