using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(PlayerMovement))]
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
    [Tooltip("Referencia al componente de ruido de la Cinemachine Camera.")]
    [SerializeField] private CinemachineBasicMultiChannelPerlin noiseComponent;

    private CinemachineInputAxisController axisController;
    private PlayerMovement playerMovement;

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

    [Header("Post Processing")]
    [SerializeField] private Volume GhostVolume;
    [SerializeField] private float volumeTransition = 5f;

    MovementState currentState;
    Vector3 lastPosition;
    bool isCameraLocked;

    float targetAmplitude;
    float targetFrequency;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        axisController = GetComponentInChildren<CinemachineInputAxisController>();

        lastPosition = transform.position;
        isCameraLocked = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Start()
    {
        isCameraLocked = false;
    }
    void Update()
    {
        SensibilityCinemachine();
        StateDetect();
        ApplyHeadBobbing();
        ApplyPostProcessing();

        lastPosition = transform.position;
    }

    void StateDetect()
    {
        if (playerMovement.Flying)
        {
            currentState = MovementState.Flying;
            return;
        }

        float speed = new Vector3(playerMovement.Velocity.x, 0, playerMovement.Velocity.z).magnitude;
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
    void ApplyPostProcessing()
    {
        if (GhostVolume == null || !playerMovement.CanFly) return;
        float targetWeight = (currentState == MovementState.Flying) ? 1f : 0f;
        GhostVolume.weight = Mathf.Lerp(GhostVolume.weight, targetWeight, Time.deltaTime * volumeTransition);
    }

    void SensibilityCinemachine()
    {
        if (axisController == null) return;

        float currentSens = SettingsDataManager.MouseSensibility;
        bool invX = SettingsDataManager.InvertX;
        bool invY = SettingsDataManager.InvertY;

        foreach (var c in axisController.Controllers)
        {
            if (isCameraLocked) c.Input.LegacyGain = 0;
            else
            {
                if (c.Name == "Look X (Pan)") c.Input.LegacyGain = currentSens * (invX ? -1 : 1);
                if (c.Name == "Look Y (Tilt)") c.Input.LegacyGain = currentSens * (invY ? 1 : -1);
            }
        }
    }
    public void LockCamera(bool lockCam) => isCameraLocked = lockCam;
}