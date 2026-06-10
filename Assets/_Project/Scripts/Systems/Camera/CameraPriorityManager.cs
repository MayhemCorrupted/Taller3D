using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraPriorityManager : MonoBehaviour
{
    public static CameraPriorityManager Instance { get; private set; }

    [Header("Core Reference")]
    [Tooltip("La cámara que sigue al jugador por defecto.")]
    [SerializeField] CinemachineCamera mainCamera;

    [Header("Priority Settings")]
    [SerializeField] int activePriority = 20;
    [SerializeField] int inactivePriority = 10;

    CinemachineCamera currentActiveCamera;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (mainCamera != null)
        {
            mainCamera.Priority = activePriority;
            currentActiveCamera = mainCamera;
        }
    }

    public void SwitchCameraTemporarily(CinemachineCamera targetCam, float duration)
    {
        if (targetCam == null || targetCam == currentActiveCamera) return;

        StopAllCoroutines();
        StartCoroutine(TemporarySwitchRoutine(targetCam, duration));
    }
    public void SwitchCameraPermanently(CinemachineCamera targetCam)
    {
        if (targetCam == null || targetCam == currentActiveCamera) return;

        StopAllCoroutines();
        SetCameraPriority(targetCam);
    }
    public void ReturnToMainCamera()
    {
        StopAllCoroutines();
        SetCameraPriority(mainCamera);
    }
    IEnumerator TemporarySwitchRoutine(CinemachineCamera targetCam, float duration)
    {
        SetCameraPriority(targetCam);

        yield return new WaitForSeconds(duration);

        SetCameraPriority(mainCamera);
    }
    void SetCameraPriority(CinemachineCamera newCam)
    {
        if (currentActiveCamera != null) currentActiveCamera.Priority = inactivePriority;

        newCam.Priority = activePriority;
        currentActiveCamera = newCam;
    }
}
