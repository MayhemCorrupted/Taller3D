using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SecuenceManager : MonoBehaviour
{
    [System.Serializable] public struct DialogueStep
    {
        [TextArea(2, 5)] public string dialogueText;
        public TextMeshProUGUI textUI;
        public AudioClip audioClip;
        public CanvasGroup panelImage;
    }

    [System.Serializable] public struct PanelData
    {
        public string panelName;
        public GameObject virtualCamera;
        public List<DialogueStep> steps;
    }

    public AudioSource audioSource;
    public Button nextButton;
    public string nextSceneName;

    public TMP_FontAsset globalFont;
    public float globalFontSize = 36f;
    public Color globalColor = Color.white;

    public float fadeSpeed = 2f;
    public float textSpeed = 0.03f;

    public List<PanelData> storyPanels;

    private int currentPanelIndex = 0;
    private int currentStepIndex = -1;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullText = "";
    private TextMeshProUGUI activeTextUI;

    void Start()
    {
        if (storyPanels.Count > 0)
        {
            SetupInitialScene();

            if (storyPanels[0].steps.Count > 0 && storyPanels[0].steps[0].panelImage != null)
            {
                storyPanels[0].steps[0].panelImage.alpha = 1f;
            }
        }
    }

    void SetupInitialScene()
    {
        for (int i = 0; i < storyPanels.Count; i++)
        {
            if (storyPanels[i].virtualCamera != null)
            {
                storyPanels[i].virtualCamera.SetActive(i == 0);
            }

            foreach (var step in storyPanels[i].steps)
            {
                if (step.panelImage != null) step.panelImage.alpha = 0f;

                if (step.textUI != null)
                {
                    if (globalFont != null) step.textUI.font = globalFont;
                    step.textUI.fontSize = globalFontSize;
                    step.textUI.color = globalColor;
                    step.textUI.alignment = TextAlignmentOptions.Center;
                    step.textUI.text = "";
                    step.textUI.gameObject.SetActive(false);
                }
            }
        }
    }

    public void AdvanceSequence()
    {
        if (isTyping)
        {
            CompleteTextInstantly();
            return;
        }

        if (currentStepIndex < storyPanels[currentPanelIndex].steps.Count - 1)
        {
            currentStepIndex++;
            ActivateCurrentStep();
        }
        else
        {
            FadeOutCurrentPanel();

            if (storyPanels[currentPanelIndex].virtualCamera != null)
            {
                storyPanels[currentPanelIndex].virtualCamera.SetActive(false);
            }

            currentPanelIndex++;
            currentStepIndex = 0;

            if (currentPanelIndex < storyPanels.Count)
            {
                if (storyPanels[currentPanelIndex].virtualCamera != null)
                {
                    storyPanels[currentPanelIndex].virtualCamera.SetActive(true);
                }
                ActivateCurrentStep();
            }
            else
            {
                FinishComic();
            }
        }
    }

    void ActivateCurrentStep()
    {
        DialogueStep currentStep = storyPanels[currentPanelIndex].steps[currentStepIndex];

        if (activeTextUI != null)
        {
            activeTextUI.text = "";
            activeTextUI.gameObject.SetActive(false);
        }

        activeTextUI = currentStep.textUI;

        if (activeTextUI != null)
        {
            activeTextUI.gameObject.SetActive(true);
            currentFullText = currentStep.dialogueText;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(currentStep.dialogueText));
        }

        if (currentStep.audioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(currentStep.audioClip);
        }

        if (currentStep.panelImage != null)
        {
            StartCoroutine(FadeInObject(currentStep.panelImage));
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        activeTextUI.text = "";
        foreach (char letter in text.ToCharArray())
        {
            activeTextUI.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    void CompleteTextInstantly()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        activeTextUI.text = currentFullText;
        isTyping = false;
    }

    IEnumerator FadeInObject(CanvasGroup cg)
    {
        while (cg.alpha < 1f)
        {
            cg.alpha += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1f;
    }

    void FadeOutCurrentPanel()
    {
        foreach (var step in storyPanels[currentPanelIndex].steps)
        {
            if (step.panelImage != null)
            {
                StartCoroutine(FadeOutObject(step.panelImage));
            }

            if (step.textUI != null)
            {
                step.textUI.text = "";
                step.textUI.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator FadeOutObject(CanvasGroup cg)
    {
        while (cg.alpha > 0f)
        {
            cg.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0f;
    }

    void FinishComic()
    {
        if (nextButton != null) nextButton.interactable = false;
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            print ("finish book");
        }
    }
}