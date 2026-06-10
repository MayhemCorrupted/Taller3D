using UnityEngine;
using UnityEngine.Events;

public class TriggerInteract : MonoBehaviour, IInteractable
{
    [SerializeField] string textInteract;
    [SerializeField] GameObject[] objectsToActivate;
    [SerializeField] GameObject[] objectsToDeactivate;
    [SerializeField] UnityEvent OnTriggered;
    bool isTriggered = false;
    public string GetTextInteract() => isTriggered ? string.Empty : textInteract;
    public void Interact(Transform interactorTransform)
    {
        TriggerObject();
    }
    void TriggerObject()
    {
        if (isTriggered) return;
        isTriggered = true;

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null) obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null) obj.SetActive(false);
        }
        OnTriggered?.Invoke();
    }
}
