using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class ComicPanel : MonoBehaviour
{
    [System.Serializable] private class StepData 
    {
        [SerializeField] private string stepName;

        [Header("UI y Visuales")]
        [SerializeField] private CanvasGroup panelImage;
        [SerializeField] private TextMeshProUGUI textUI;
        [TextArea(2, 5)][SerializeField] private string dialogueText;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;

        public CanvasGroup PanelImage => panelImage;
        public TextMeshProUGUI TextUI => textUI; 
        public string DialogueText => dialogueText;
        public AudioSource AudioSource => audioSource;
        public AudioClip AudioClip => audioClip;
    }

    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float textSpeed = 0.03f;

    [Header("Estilo de texto")]
    [SerializeField] private TMP_FontAsset globalFont;
    [SerializeField] private Color globalColor = Color.white;
    [SerializeField] private float globalFontSize = 36f;
    [SerializeField] private TextAlignmentOptions globalAlignment = TextAlignmentOptions.Center;

    [Space(10)]
    [SerializeField] private List<StepData> steps;

    private int currentStepIndex = 0;
    private Coroutine typingCoroutine;
    private bool isWriting = false;
    private string currentFullText = "";

    public void ActivatePanel()
    {
        gameObject.SetActive(true);

        if (virtualCamera != null)
        {
            virtualCamera.gameObject.SetActive(true);
            virtualCamera.Priority = 10;
        }

        foreach (var step in steps)
        {
            if (step.PanelImage != null) step.PanelImage.alpha = 0f;

            if (step.TextUI != null)

            {
                step.TextUI.enableAutoSizing = false;
                if (globalFont != null) step.TextUI.font = globalFont;
                step.TextUI.fontSize = globalFontSize;
                step.TextUI.color = globalColor;
                step.TextUI.alignment = globalAlignment;

                step.TextUI.text = "";
                step.TextUI.gameObject.SetActive(false);
            }
        }

        currentStepIndex = 0;
        if (steps.Count > 0)
        {
            ActivateStep(currentStepIndex);
        }
    }

    public bool Advance()
    {
        if (isWriting)
        {
            CompleteTextInstantly();
            return true;
        }

        if (currentStepIndex < steps.Count - 1)
        {
            currentStepIndex++;
            ActivateStep(currentStepIndex);
            return true;
        }

        return false;
    }

    public void DeactivatePanel()
    {
        StopAllCoroutines();

        if (virtualCamera != null)
        {
            virtualCamera.Priority = 0;
            StartCoroutine(DisableCameraDelayed(virtualCamera.gameObject));
        }

        gameObject.SetActive(false);
    }
    private IEnumerator DisableCameraDelayed(GameObject camObj)
    {
        yield return new WaitForSeconds(2f);
        camObj.SetActive(false);
    }

    private void ActivateStep(int index)
    {
        StepData currentStep = steps[index];

        if (currentStep.TextUI != null)
        {
            currentStep.TextUI.gameObject.SetActive(true);
            currentFullText = currentStep.DialogueText;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(currentStep.TextUI, currentStep.DialogueText));
        }

        if (currentStep.AudioSource != null && currentStep.AudioClip != null)
        {
            if (currentStep.AudioSource.isPlaying) currentStep.AudioSource.Stop();
            currentStep.AudioSource.clip = currentStep.AudioClip;
            currentStep.AudioSource.Play();
        }

        if (currentStep.PanelImage != null)
        {
            StartCoroutine(FadeObject(currentStep.PanelImage));
        }
    }

    private void CompleteTextInstantly()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        steps[currentStepIndex].TextUI.text = currentFullText;
        isWriting = false;
    }

    private IEnumerator TypeText(TextMeshProUGUI tmpText, string text)
    {
        isWriting = true;
        tmpText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            tmpText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        isWriting = false;
    }

    private IEnumerator FadeObject(CanvasGroup cg)
    {
        while (cg.alpha < 1f)
        {
            cg.alpha += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1f;
    }
}