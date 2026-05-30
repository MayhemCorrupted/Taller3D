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
    [SerializeField] float activeRadius = 0.75f;
    [SerializeField] bool disableTriggerAfterUse = true;

    public UnityEvent OnTriggered;

    bool triggered = false;
    float dialogueTimer;
    float totalCalculatedDuration;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
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
        float currentDistance = Vector3.Distance(triggerPoint.position, playerTransform.position);
        if (currentDistance <= activeRadius)
        {
            ShotDialogue();
            triggered = true;

            if (disableTriggerAfterUse)
            {
                enabled = false;
            }
        }
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
        Gizmos.DrawWireSphere(triggerPoint.position, activeRadius);
    }
}   
