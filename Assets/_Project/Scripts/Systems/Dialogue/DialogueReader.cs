using UnityEngine;
using UnityEngine.Events;

public class DialogueReader : MonoBehaviour
{
    Transform playerTransform;

    [Header("Library Reference")]
    [Tooltip("La llave que buscará en el DialogueLibrary")]
    [SerializeField] string dialogueKey;

    [Header("Playback Settings")]
    [SerializeField] float durationPerLine = 3f;
    [SerializeField] bool moveCameraToTarget = true;
    [SerializeField] float cameraLookAtSpeed = 2f;

    [Header("Trigger Settings")]
    [SerializeField] Transform triggerPoint;
    [SerializeField] Vector3 activeBoxSize = new();
    [SerializeField] bool disableTriggerAfterUse = true;

    public UnityEvent OnTriggered;

    bool triggered = false;
    float dialogueTimer;
    float totalCalculatedDuration;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (triggerPoint == null) triggerPoint = transform;
    }

    void Update()
    {
        if (playerTransform == null || triggerPoint == null) return;

        if (triggered)
        {
            if (!disableTriggerAfterUse)
            {
                dialogueTimer += Time.deltaTime;
                if (dialogueTimer >= totalCalculatedDuration)
                {
                    triggered = false;
                    dialogueTimer = 0f;
                }
            }
            return;
        }

        if (IsPlayerInsideBox())
        {
            ShotDialogue();
            triggered = true;

            if (disableTriggerAfterUse)
            {
                enabled = false;
            }
        }
    }
    private bool IsPlayerInsideBox()
    {
        Vector3 difference = playerTransform.position - triggerPoint.position;

        Vector3 extents = activeBoxSize / 2f;

        bool insideX = Mathf.Abs(difference.x) <= extents.x;
        bool insideY = Mathf.Abs(difference.y) <= extents.y;
        bool insideZ = Mathf.Abs(difference.z) <= extents.z;

        return insideX && insideY && insideZ;
    }

    public void ShotDialogue()
    {
        if (Dialogues.Instance == null) return;

        var dataInfo = Dialogues.Instance.GetDialogue(dialogueKey);
        if (dataInfo == null) return;

        var data = dataInfo.Value;

        totalCalculatedDuration = data.dialogueLines.Length * (durationPerLine + 0.4f);

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(
                data.uiTextComponent,
                data.targetToLookAt,
                data.dialogueLines,
                cameraLookAtSpeed,
                moveCameraToTarget,
                durationPerLine
            );
        }
        OnTriggered?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (triggerPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(triggerPoint.position, activeBoxSize);
    }
}   
