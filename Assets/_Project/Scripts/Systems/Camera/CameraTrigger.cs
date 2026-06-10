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

    bool hasBeenTriggered = false;
    float cameraTimer;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (triggerPoint == null) triggerPoint = transform;
    }

    private void Update()
    {
        if (playerTransform == null || triggerPoint == null) return;

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

        if (IsPlayerInsideBox())
        {
            ExecuteCameraChange();
            hasBeenTriggered = true;

            if (triggerOnlyOnce) enabled = false;
        }
    }
    bool IsPlayerInsideBox()
    {
        Vector3 difference = playerTransform.position - triggerPoint.position;
        Vector3 extents = activeBoxSize / 2f;

        bool insideX = Mathf.Abs(difference.x) <= extents.x;
        bool insideY = Mathf.Abs(difference.y) <= extents.y;
        bool insideZ = Mathf.Abs(difference.z) <= extents.z;

        return insideX && insideY && insideZ;
    }
    public void ExecuteCameraChange()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning($"[CameraTrigger] No hay cámara asignada en {gameObject.name} :(");
            return;
        }

        if (isTemporary) CameraPriorityManager.Instance.SwitchCameraTemporarily(targetCamera, activeDuration);
        else CameraPriorityManager.Instance.SwitchCameraPermanently(targetCamera);
    }
    void OnDrawGizmosSelected()
    {
        if (triggerPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(triggerPoint.position, activeBoxSize);
    }
}
