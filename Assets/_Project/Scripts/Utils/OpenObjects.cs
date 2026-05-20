using System.Collections;
using UnityEngine;

public class OpenObjects : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float openDistance = 0.4f;
    [SerializeField] float speed = 1f;

    [Header("Prompt")]
    public string interactPrompt = "[E] Abrir";

    Vector3 closedPosition;
    Vector3 openPosition;
    bool isOpen;
    bool isMoving;

    void Awake()
    {
        closedPosition = transform.localPosition;
        openPosition = closedPosition + new Vector3(0, 0, openDistance);
    }

    public void Interact()
    {
        if (isMoving) return;

        StopAllCoroutines();

        if (!isOpen)
            StartCoroutine(MoveDrawer(openPosition));
        else
            StartCoroutine(MoveDrawer(closedPosition));
    }

    IEnumerator MoveDrawer(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.localPosition;
        float moveTime = 0f;

        while (moveTime < speed)
        {
            moveTime += Time.deltaTime;
            float time = moveTime / speed;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time);
            yield return null;
        }

        transform.localPosition = targetPosition;
        isOpen = !isOpen;
        isMoving = false;
    }
}