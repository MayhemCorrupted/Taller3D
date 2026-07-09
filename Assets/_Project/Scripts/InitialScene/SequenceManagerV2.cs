using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


public class SequenceManagerV2 : MonoBehaviour
{
    [SerializeField] private List<ComicPanel> panels;
    [SerializeField] private string nextSceneName;
    [SerializeField] private float cameraTime = 2f;
    [SerializeField] private Image imageFade;
    [SerializeField] private float delay = 1f;
    [SerializeField] private float duration = 1f;

    private int currentPanelIndex = 0;
    private bool isTransitioning = false;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (imageFade != null)
        {
            imageFade.gameObject.SetActive(true);
            Color c = imageFade.color;
            c.a = 1f;
            imageFade.color = c;

            StartCoroutine(FadeInitial());
        }

        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i] != null)
            {
                if (i == 0) panels[i].ActivatePanel();
                else panels[i].gameObject.SetActive(false);
            }
        }
    }
    private IEnumerator FadeInitial()
    {
        isTransitioning = true;

        yield return new WaitForSeconds(delay);

        float timev = 0f;
        Color colorC = imageFade.color;

        while (timev < duration)
        {
            timev += Time.deltaTime;
            float t = timev / duration;
            colorC.a = Mathf.Lerp(1f, 0f, t);

            imageFade.color = colorC;
            yield return null;
        }
        colorC.a = 0f;
        imageFade.color = colorC;
        isTransitioning = false;
    }


    public void AdvanceSequence()
    {
        if (isTransitioning) return;
        if (!panels[currentPanelIndex].Advance())
        {
            ChangeNextPanel();
        }
    }

    private void ChangeNextPanel()
    {
        isTransitioning = true;
        ComicPanel panelToDisable = panels[currentPanelIndex];
        currentPanelIndex++;

        if (currentPanelIndex < panels.Count)
        {
            panels[currentPanelIndex].ActivatePanel();
            StartCoroutine(DisablePanel(panelToDisable, cameraTime));
        }
        else
        {
            FinishComic();
        }
    }

    private IEnumerator DisablePanel(ComicPanel panel, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (panel != null)
        {
            panel.DeactivatePanel();
        }

        isTransitioning = false;
    }

    public void FinishComic()
    {
        isTransitioning = true;
        StartCoroutine(FadeAndChangeScene());
    }

    private IEnumerator FadeAndChangeScene()
    {
        {
            if (imageFade != null)
            {
                float timev = 0f;
                Color colorC = imageFade.color;

                while (timev < duration)
                {
                    timev += Time.deltaTime;
                    float t = timev / duration;
                    colorC.a = Mathf.Lerp(0f, 1f, t);
                    imageFade.color = colorC;
                    yield return null;
                }
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }
}