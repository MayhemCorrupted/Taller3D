using UnityEngine;
using UnityEngine.Events;
public class DialogueReader : MonoBehaviour
{
    [Header("Library Reference")]
    [Tooltip("La llave que buscará en el DialogueLibrary")]
    [SerializeField] string dialogueKey;
    [Header("Settings")]
    [SerializeField] bool disableAfterUse = true;
    [SerializeField] bool OneUseOnly = true;
    [Header("Events")]
    public UnityEvent OnTriggered;

    bool hasBeenExecuted = false;
    public void ShotDialogue()
    {
        if (OneUseOnly && hasBeenExecuted) return;

        var dataInfo = Dialogues.Instance.GetDialogue(dialogueKey);
        if (dataInfo == null) return;

        CancelInvoke();

        bool dialogueStarted = DialogueManager.Instance.StartDialogue(dataInfo.Value);
        if (!dialogueStarted) return;

        hasBeenExecuted = true;
        OnTriggered?.Invoke();

        if (disableAfterUse) this.enabled = false;
    }
}   
