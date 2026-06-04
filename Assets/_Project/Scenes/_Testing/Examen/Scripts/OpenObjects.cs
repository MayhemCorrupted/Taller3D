using System.Collections;
using UnityEngine;

public class OpenObjects : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] float openDistance = 0.4f;
    [SerializeField] float speed = 1f;

    [Header("Prompt")]
    [SerializeField] string interactPrompt = "[E] Abrir";

    Vector3 closedPosition;
    Vector3 openPosition;
    bool isOpen;
    bool isMoving;

    void Awake()
    {
        closedPosition = transform.localPosition;
        openPosition = closedPosition + new Vector3(0, 0, openDistance);
    }

    public string GetTextInteract() => interactPrompt;

    public void Interact(Transform interactorTransform) => ToggleObject();

    public void ToggleObject()
    {
        if (isMoving) return;

        StopAllCoroutines();
        StartCoroutine(MoveObject(isOpen ? closedPosition : openPosition));
    }

    IEnumerator MoveObject(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < speed)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, target, elapsed / speed);
            yield return null;
        }

        transform.localPosition = target;
        isOpen = !isOpen;
        isMoving = false;
    }
}