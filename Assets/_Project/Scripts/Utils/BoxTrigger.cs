using UnityEngine;
using UnityEngine.Events;

public class BoxTrigger : MonoBehaviour
{
    Transform playerTransform;
    [Header("Settings")]
    [SerializeField] Vector3 activeBoxSize = new();
    [SerializeField] bool disableTriggerAfterUse = true;
    [SerializeField] bool singleTriggerOnlyViaEvent = true;
    public UnityEvent OnTriggered;
    bool triggered = false;
    bool hasBeenExecuted = false;
    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        if (triggered || playerTransform == null) return;

        if (IsPlayerInsideBox()) DetectTrigger();
    }
    bool IsPlayerInsideBox()
    {
        Vector3 difference = playerTransform.position - transform.position;

        Vector3 extents = activeBoxSize / 2f;

        bool insideX = Mathf.Abs(difference.x) <= extents.x;
        bool insideY = Mathf.Abs(difference.y) <= extents.y;
        bool insideZ = Mathf.Abs(difference.z) <= extents.z;

        return insideX && insideY && insideZ;
    }
    void DetectTrigger()
    {
        if (singleTriggerOnlyViaEvent && hasBeenExecuted) return; 

        hasBeenExecuted = true;
        triggered = true;

        OnTriggered?.Invoke();

        if (disableTriggerAfterUse) this.enabled = false;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, activeBoxSize);
    }
}
