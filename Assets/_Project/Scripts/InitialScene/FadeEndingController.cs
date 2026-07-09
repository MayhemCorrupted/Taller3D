using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Playables;

public class FadeEndingController : MonoBehaviour
{
    [SerializeField] CanvasGroup fadeGroup;
    [SerializeField] PlayableDirector timeline;
    [SerializeField] float fadeD = 1f;
    [SerializeField] string sceneName;

    void Start()
    {
        StartCoroutine(Fade(1, 0));

        if (timeline != null)
        {
            StartCoroutine(TimelineEnd());
        }
    }

    IEnumerator TimelineEnd()
    {
        yield return new WaitUntil(() => timeline.state == PlayState.Playing);
        float totalDuration = (float)timeline.duration;
        yield return new WaitForSeconds(totalDuration - fadeD);
        yield return StartCoroutine(Fade(0, 1));
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Fade(float fr, float to)
    {
        float tim = 0;
        while (tim < fadeD)
        {
            tim += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(fr, to, tim / fadeD);
            yield return null;
        }
        fadeGroup.alpha = to;
    }
}