using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Playables;

public class FadeEndingController : MonoBehaviour
{
    [SerializeField] CanvasGroup fade;
    [SerializeField] PlayableDirector timeline;
    [SerializeField] float duration = 1f;
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
        yield return new WaitForSeconds(totalDuration - duration);
        yield return StartCoroutine(Fade(0, 1));
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Fade(float fr, float to)
    {
        float tim = 0;
        while (tim < duration)
        {
            tim += Time.deltaTime;
            fade.alpha = Mathf.Lerp(fr, to, tim / duration);
            yield return null;
        }
        fade.alpha = to;
    }
}