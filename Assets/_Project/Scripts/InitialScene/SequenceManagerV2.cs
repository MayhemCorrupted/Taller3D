using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SequenceManagerV2 : MonoBehaviour
{
    [SerializeField] private List<ComicPanel> panels;
    [SerializeField] private string nextSceneName;
    [SerializeField] private float cameraTime = 2f;

    private int currentPanelIndex = 0;
    private bool isTransitioning = false;

    private void Start()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i] != null)
            {
                if (i == 0) panels[i].ActivatePanel();
                else panels[i].gameObject.SetActive(false);
            }
        }
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
            StartCoroutine(DisablePanelAfterDelay(panelToDisable, cameraTime));
        }
        else
        {
            FinishComic();
        }
    }

    private IEnumerator DisablePanelAfterDelay(ComicPanel panel, float delay)
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
        SceneManager.LoadScene(nextSceneName);
    }
}