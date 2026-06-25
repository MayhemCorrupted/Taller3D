using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CameraTrigger))]
public class SpyCameraInteractable : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] KeyCode interactKey = KeyCode.E;

    [Header("Duración")]
    [SerializeField] bool isTemporary = false;
    [SerializeField] float spyDuration = 5f;

    [Header("Comportamiento")]
    [SerializeField] bool exitOnLeaveArea = true;

    [Header("Unity Events")]
    public UnityEvent OnPromptShow;      // Mostrar "Presiona E"
    public UnityEvent OnPromptHide;      // Ocultar prompt
    public UnityEvent OnSpyStart;        // Entró a modo espía
    public UnityEvent OnSpyEnd;          // Salió de modo espía

    CameraTrigger trigger;
    bool isSpying = false;
    bool promptVisible = false;

    void Awake()
    {
        trigger = GetComponent<CameraTrigger>();
    }

    void Update()
    {
        if (trigger == null) return;

        bool inside = trigger.PlayerIsInside;

        // Entró al área
        if (inside && !promptVisible && !isSpying)
        {
            promptVisible = true;
            OnPromptShow?.Invoke();
        }

        // Salió del área
        if (!inside && promptVisible)
        {
            promptVisible = false;
            OnPromptHide?.Invoke();
        }

        // Salió mientras espiaba
        if (!inside && isSpying && exitOnLeaveArea)
        {
            EndSpyMode();
        }

        // Presionó E dentro del área
        if (inside && Input.GetKeyDown(interactKey))
        {
            ToggleSpyMode();
        }
    }

    void ToggleSpyMode()
    {
        CinemachineCamera cam = trigger.TargetCamera;
        if (cam == null) return;

        bool nowSpying = CameraPriorityManager.Instance.ToggleSpyMode(cam);
        isSpying = nowSpying;

        if (nowSpying)
        {
            promptVisible = false;
            OnPromptHide?.Invoke();
            OnSpyStart?.Invoke();

            if (isTemporary)
            {
                Invoke(nameof(EndSpyMode), spyDuration);
            }
        }
        else
        {
            CancelInvoke(nameof(EndSpyMode));
            OnSpyEnd?.Invoke();
        }
    }

    void EndSpyMode()
    {
        if (!isSpying) return;

        CameraPriorityManager.Instance.ExitSpyMode();
        isSpying = false;
        CancelInvoke(nameof(EndSpyMode));
        OnSpyEnd?.Invoke();

        // Si sigue dentro, mostrar prompt de nuevo
        if (trigger.PlayerIsInside)
        {
            promptVisible = true;
            OnPromptShow?.Invoke();
        }
    }

    void OnDisable()
    {
        CancelInvoke(nameof(EndSpyMode));
    }
}
