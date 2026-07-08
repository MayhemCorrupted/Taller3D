using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Playables;

public class FadeEndingController : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] CanvasGroup fadeGroup;
    [SerializeField] PlayableDirector timelineDirector;

    [Header("Configuración")]
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] string nextSceneName;

    void Start()
    {
        StartCoroutine(Fade(1, 0));

        if (timelineDirector != null)
        {
            StartCoroutine(WaitUntilTimelineEnds());
        }
    }

    IEnumerator WaitUntilTimelineEnds()
    {
        yield return new WaitUntil(() => timelineDirector.state == PlayState.Playing);
        float totalDuration = (float)timelineDirector.duration;
        yield return new WaitForSeconds(totalDuration - fadeDuration);
        yield return StartCoroutine(Fade(0, 1));
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        fadeGroup.alpha = to;
    }
}