using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraPriorityManager : MonoBehaviour
{
    public static CameraPriorityManager Instance { get; private set; }

    [Header("Core Reference")]
    [SerializeField] CinemachineCamera mainCamera;

    [Header("Priority Settings")]
    [SerializeField] int activePriority = 20;
    [SerializeField] int inactivePriority = 10;

    CinemachineCamera currentActiveCamera;
    CinemachineCamera lastActiveCamera;

    public CinemachineCamera CurrentCamera => currentActiveCamera;
    public bool IsMainCameraActive => currentActiveCamera == mainCamera;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (mainCamera != null)
        {
            mainCamera.Priority = activePriority;
            currentActiveCamera = mainCamera;
            lastActiveCamera = mainCamera;
        }
    }

    // ─── MÉTODOS ORIGINALES (para CameraTrigger automático) ───

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

    // ─── MÉTODOS NUEVOS (para SpyCameraInteractable) ───

    public void EnterSpyMode(CinemachineCamera spyCam)
    {
        if (spyCam == null || spyCam == currentActiveCamera) return;
        StopAllCoroutines();
        lastActiveCamera = currentActiveCamera;
        SetCameraPriority(spyCam);
    }

    public void ExitSpyMode()
    {
        StopAllCoroutines();
        if (lastActiveCamera != null && lastActiveCamera != currentActiveCamera)
            SetCameraPriority(lastActiveCamera);
        else
            SetCameraPriority(mainCamera);
    }

    public bool ToggleSpyMode(CinemachineCamera spyCam)
    {
        if (currentActiveCamera == spyCam)
        {
            ExitSpyMode();
            return false;
        }
        else
        {
            EnterSpyMode(spyCam);
            return true;
        }
    }

    // ─── PRIVADOS ───

    IEnumerator TemporarySwitchRoutine(CinemachineCamera targetCam, float duration)
    {
        lastActiveCamera = currentActiveCamera;
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
