using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class BoxTrigger : MonoBehaviour
{
    BoxCollider colliderBox;
    [Header("Settings")]
    [SerializeField] bool disableTriggerAfterUse = true;
    [SerializeField] bool singleTriggerOnlyViaEvent = true;
    public UnityEvent OnTriggered;
    bool hasBeenExecuted = false;
    void Awake()
    {
        colliderBox = GetComponent<BoxCollider>();
        colliderBox.isTrigger = true;
    }
    void DetectTrigger()
    {
        if (singleTriggerOnlyViaEvent && hasBeenExecuted) return; 

        hasBeenExecuted = true;

        OnTriggered?.Invoke();

        if (disableTriggerAfterUse) this.enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DetectTrigger();
        }
    }
}
