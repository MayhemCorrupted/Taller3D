using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SequenceManagerV2 : MonoBehaviour
{
    [SerializeField] private List<ComicPanel> panels;
    [SerializeField] private string nextSceneName;
    [SerializeField] private float cameraTime = 2f;

    private int currentPanelIndex = 0;
    private bool isTransitioning = false;
    private ComicPanel oldPanel;

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
        oldPanel = panels[currentPanelIndex];
        currentPanelIndex++;

        if (currentPanelIndex < panels.Count)
        {
            panels[currentPanelIndex].ActivatePanel();
            Invoke("OffOldPanel", cameraTime);
        }
        else
        {
            FinishComic();
        }
    }

    private void OffOldPanel()
    {
        if (oldPanel != null)
        {
            oldPanel.DeactivatePanel();
        }

        isTransitioning = false;
    }

    public void FinishComic()
    {
            SceneManager.LoadScene(nextSceneName);
    }
}