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

        timeline.stopped += TimelineEnd;
    }
    private void TimelineEnd (PlayableDirector director)
    {
        StartCoroutine(ChangeScene());
    }
    IEnumerator ChangeScene()
    {
        yield return StartCoroutine(Fade(0, 1));
        SceneManager.LoadScene(sceneName);
    }
    
    IEnumerator Fade(float b, float f)
    {
        float tim = 0;
        while (tim < duration)
        {
            tim += Time.deltaTime;
            fade.alpha = Mathf.Lerp(b, f, tim / duration);
            yield return null;
        }
        fade.alpha = f;
    }
}