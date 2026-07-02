using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Rendering;
[System.Serializable]
public struct FadeEvents
{
    public string eventName;
    public float fadeInDuration;
    public float fadeStayDuration;
    public float fadeOutDuration;
    public UnityEvent OnSequenceStart;
    [Tooltip("Se invoca al terminar la duración prolongada, ANTES de que el panel empiece a desaparecer.")]
    public UnityEvent OnDurationEnded;
    [Tooltip("Opcional: Se invoca cuando el Fade Out termina y el panel ya es invisible.")]
    public UnityEvent OnSequenceComplete;
}
[RequireComponent(typeof(CanvasGroup))]
public class FadeUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] CanvasGroup canvasGroup;   
    [SerializeField] GameObject panelObject;
    [SerializeField] bool disablePanelOnComplete = true;
    [Header("Completion Options")]
    [Header("Unity Events")]
    [SerializeField] FadeEvents[] fadeSettings;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (panelObject == null) panelObject = gameObject;
    }

    public void StartSequence(string sequenceName)
    {
        StopAllCoroutines();
        bool eventFound = false;
        FadeEvents selectedEvents = default;
        foreach (var eventGroup in fadeSettings)
        {
            if (eventGroup.eventName == sequenceName)
            {
                selectedEvents = eventGroup;
                eventFound = true;
                break;
            }

        }
        StartCoroutine(FadeRoutine(selectedEvents, eventFound));
    }

    private IEnumerator FadeRoutine(FadeEvents activeEvents, bool hasEvents)
    {
        panelObject.SetActive(true);
        canvasGroup.alpha = 0f;

        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, activeEvents.fadeInDuration));
        if (hasEvents) activeEvents.OnSequenceStart?.Invoke();

        yield return new WaitForSeconds(activeEvents.fadeStayDuration);
        if (hasEvents) activeEvents.OnDurationEnded?.Invoke();

        float waitForSeconds = 0.7f;
        yield return new WaitForSeconds(waitForSeconds);
        
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, activeEvents.fadeOutDuration));

        if (hasEvents) activeEvents.OnSequenceComplete?.Invoke();

        if (disablePanelOnComplete) panelObject.SetActive(false);
        
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
