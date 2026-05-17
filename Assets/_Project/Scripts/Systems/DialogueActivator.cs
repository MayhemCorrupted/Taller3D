using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueActivator : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] TMP_Text uiTextComponent;
    [TextArea(3, 5)]
    [SerializeField] string dialogueText;
    [Header("Camera Settings")]
    [SerializeField] bool moveCameraToTarget = true;
    [SerializeField] Transform targetToLookAt;
    [SerializeField] float cameraLookDuration = 2;
    [Header("Trigger Settings")]
    [SerializeField] Collider triggerCollider;
    [SerializeField] string playerTag = "Player";
    [SerializeField] bool disableTriggerAfterUse = true;
    public UnityEvent OnTriggered;
    public void FireDialogue()
    {
        if (DialogueManager.Instance != null) 
            DialogueManager.Instance.StartDialogue(uiTextComponent, 
                targetToLookAt, dialogueText, cameraLookDuration, moveCameraToTarget);
        else Debug.LogWarning("No DialogueManager instance encontrado.");

        OnTriggered?.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (triggerCollider != null && other.CompareTag(playerTag))
        {
            FireDialogue();
            if (disableTriggerAfterUse) triggerCollider.enabled = false;
        }
    }
}   
