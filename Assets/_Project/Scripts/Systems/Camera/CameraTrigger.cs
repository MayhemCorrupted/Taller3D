using Unity.Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [Header("Camera Reference")]
    [SerializeField] CinemachineCamera targetCamera;
    Transform playerTransform;

    [Header("Transition Rules")]
    [SerializeField] bool isTemporary = true;
    [SerializeField] float activeDuration = 3.5f;

    [Header("Trigger Settings")]
    [SerializeField] Transform triggerPoint;
    [SerializeField] Vector3 activeBoxSize = new(2f, 2f, 2f);
    [SerializeField] bool triggerOnlyOnce = true;

    // ← NUEVO: Propiedad pública para que SpyCameraInteractable consulte
    public bool PlayerIsInside { get; private set; } = false;
    public CinemachineCamera TargetCamera => targetCamera;

    bool hasBeenTriggered = false;
    float cameraTimer;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        if (triggerPoint == null) triggerPoint = transform;
    }

    void Update()
    {
        if (playerTransform == null || triggerPoint == null) return;

        // Actualizar estado de detección
        bool wasInside = PlayerIsInside;
        PlayerIsInside = IsPlayerInsideBox();

        // Si hay un SpyCameraInteractable adjunto, NO ejecutar automático
        // (él se encarga del input)
        if (GetComponent<SpyCameraInteractable>() != null) return;

        // Modo automático (sin componente de espía)
        if (hasBeenTriggered)
        {
            if (!triggerOnlyOnce)
            {
                cameraTimer += Time.deltaTime;
                if (cameraTimer >= activeDuration)
                {
                    hasBeenTriggered = false;
                    cameraTimer = 0f;
                }
            }
            return;
        }

        if (PlayerIsInside)
        {
            ExecuteCameraChange();
            hasBeenTriggered = true;
            if (triggerOnlyOnce) enabled = false;
        }
    }

    public void ExecuteCameraChange()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning($"[CameraTrigger] No hay cámara asignada en {gameObject.name}");
            return;
        }

        if (isTemporary)
            CameraPriorityManager.Instance.SwitchCameraTemporarily(targetCamera, activeDuration);
        else
            CameraPriorityManager.Instance.SwitchCameraPermanently(targetCamera);
    }

    bool IsPlayerInsideBox()
    {
        Vector3 difference = playerTransform.position - triggerPoint.position;
        Vector3 extents = activeBoxSize / 2f;

        return Mathf.Abs(difference.x) <= extents.x &&
               Mathf.Abs(difference.y) <= extents.y &&
               Mathf.Abs(difference.z) <= extents.z;
    }

    void OnDrawGizmosSelected()
    {
        if (triggerPoint == null) return;

        bool hasSpy = GetComponent<SpyCameraInteractable>() != null;
        Gizmos.color = hasSpy ? Color.yellow : Color.green;
        Gizmos.DrawWireCube(triggerPoint.position, activeBoxSize);

        if (hasSpy)
        {
            Gizmos.color = Color.cyan;
            Vector3 eyePos = triggerPoint.position + Vector3.up * (activeBoxSize.y / 2f + 0.3f);
            Gizmos.DrawSphere(eyePos, 0.15f);
        }
    }
}