using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FadeUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] CanvasGroup canvasGroup;   
    [SerializeField] GameObject panelObject;

    [Header("Fade Settings")]
    [SerializeField] float fadeInDuration = 1.5f;
    [SerializeField] float stayDuration = 5.0f;
    [SerializeField] float fadeOutDuration = 1.5f;

    [Header("Completion Options")]
    [SerializeField] bool disablePanelOnComplete = true;
    [Header("Events")]
    [SerializeField] private UnityEvent OnSequenceStart;
    [Tooltip("Se invoca al terminar la duración prolongada, ANTES de que el panel empiece a desaparecer.")]
    [SerializeField] private UnityEvent OnDurationEnded;
    [Tooltip("Opcional: Se invoca cuando el Fade Out termina y el panel ya es invisible.")]
    [SerializeField] private UnityEvent OnSequenceComplete;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (panelObject == null) panelObject = gameObject;
    }

    public void StartSequence()
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        panelObject.SetActive(true);
        canvasGroup.alpha = 0f;

        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, fadeInDuration));
        OnSequenceStart?.Invoke();
        yield return new WaitForSeconds(stayDuration);
        OnDurationEnded?.Invoke();
        float waitForSeconds = 0.7f;
        yield return new WaitForSeconds(waitForSeconds);
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, fadeOutDuration));

        OnSequenceComplete?.Invoke();

        if (disablePanelOnComplete)
        {
            panelObject.SetActive(false);
        }
    }

    private IEnumerator FadeCanvasGroup(float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        canvasGroup.alpha = startAlpha;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
