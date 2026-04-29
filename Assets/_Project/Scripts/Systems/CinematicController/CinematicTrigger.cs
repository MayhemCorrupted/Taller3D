using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CinematicTrigger : MonoBehaviour
{
    public enum TriggerMode { OnEnter, Manual }

    [Header("Cinematic")]
    [SerializeField] string cinematicID;
    [SerializeField] TriggerMode mode = TriggerMode.OnEnter;
    [SerializeField] bool triggerOnce = true;

    bool hasTriggered;

    void Start()
    {
        if (mode == TriggerMode.OnEnter)
            GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (mode != TriggerMode.OnEnter) return;
        if (!other.CompareTag("Player")) return;

        Activate();
    }

    public void Activate()
    {
        if (triggerOnce && hasTriggered) return;
        if (string.IsNullOrEmpty(cinematicID)) return;

        hasTriggered = true;
        CinematicController.Instance.PlayCinematic(cinematicID);
    }

    public void Reset() => hasTriggered = false;
}
