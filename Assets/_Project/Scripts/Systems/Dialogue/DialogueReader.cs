using UnityEngine;
using UnityEngine.Events;
public class DialogueReader : MonoBehaviour
{
    Transform playerTransform;

    [Header("Library Reference")]
    [Tooltip("La llave que buscará en el DialogueLibrary")]
    [SerializeField] string dialogueKey;

    [Header("Trigger Settings")]
    [SerializeField] Transform triggerPoint;
    [SerializeField] Vector3 activeBoxSize = new(1,1,1);
    [SerializeField] bool disableTriggerAfterUse = true;
    [SerializeField] bool singleTriggerOnlyViaEvent = true;
    public UnityEvent OnTriggered;

    bool triggered = false;
    bool hasBeenExecuted = false;
    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        if (triggerPoint == null) triggerPoint = transform;
    }
    void Update()
    {
        if (triggered || playerTransform == null || triggerPoint == null) return;

        if (IsPlayerInsideBox())
        {
            ShotDialogue();
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
    public void ShotDialogue()
    {
        if (singleTriggerOnlyViaEvent && hasBeenExecuted) return;

        var dataInfo = Dialogues.Instance.GetDialogue(dialogueKey);
        if (dataInfo == null) return;

        bool dialogueStarted = DialogueManager.Instance.StartDialogue(dataInfo.Value);
        if (!dialogueStarted) return;

        hasBeenExecuted = true;
        triggered = true;

        OnTriggered?.Invoke();

        if (disableTriggerAfterUse) this.enabled = false;
    }
    void OnDrawGizmosSelected()
    {
        if (triggerPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(triggerPoint.position, activeBoxSize);
    }
}   
