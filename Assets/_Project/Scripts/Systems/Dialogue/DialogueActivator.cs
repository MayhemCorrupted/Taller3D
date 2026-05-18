using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueActivator : MonoBehaviour
{
    Transform playerTransform;
    [Header("Dialogue Settings")]
    [SerializeField] TMP_Text uiTextComponent;
    [TextArea(3, 5)]
    [SerializeField] string dialogueText;
    [SerializeField] float dialogueDuration;
    [Header("Camera Settings")]
    [SerializeField] bool moveCameraToTarget = true;
    [SerializeField] Transform targetToLookAt;
    [SerializeField] float cameraLookAtSpeed = 2;
    [Header("Trigger Settings")]
    [SerializeField] Transform triggerPoint;
    [SerializeField] float activeRadius = 0.75f;
    [SerializeField] bool disableTriggerAfterUse = true;
    bool triggered = false;
    public UnityEvent OnTriggered;
    float dialogueTimer;
    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        if (playerTransform == null || triggerPoint == null) return;
       
        if (triggered)
        {
            if (!disableTriggerAfterUse)
            {
                dialogueTimer += Time.deltaTime;

                if (dialogueTimer >= dialogueDuration * 2f)
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
            FireDialogue();
            triggered = true;

            if (disableTriggerAfterUse)
            {
                enabled = false;
            }
        }
    }
    public void FireDialogue()
    {
        if (DialogueManager.Instance != null) 
            DialogueManager.Instance.StartDialogue(uiTextComponent, 
                targetToLookAt, dialogueText, cameraLookAtSpeed, moveCameraToTarget, dialogueDuration);
        else Debug.LogWarning("No DialogueManager instance encontrado.");

        OnTriggered?.Invoke();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(triggerPoint.position, activeRadius);
    }
}   
