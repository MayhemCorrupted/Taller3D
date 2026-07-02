using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class ComicPanel : MonoBehaviour
{
    [System.Serializable] private class StepData 

    {
        [Header("UI Visuals")]
        public CanvasGroup panelImage;
        public TextMeshProUGUI textUI;
        [TextArea(2, 5)] public string dialogueText;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip audioClip; 
    }

    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float textSpeed = 0.03f;

    [Header("Text style")]
    [SerializeField] private TMP_FontAsset globalFont;
    [SerializeField] private Color globalColor = Color.black;
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
            if (step.panelImage != null) step.panelImage.alpha = 0f;

            if (step.textUI != null)

            {
                step.textUI.enableAutoSizing = false;
                if (globalFont != null) step.textUI.font = globalFont;
                step.textUI.fontSize = globalFontSize;
                step.textUI.color = globalColor;
                step.textUI.alignment = globalAlignment;

                step.textUI.text = "";
                step.textUI.gameObject.SetActive(false);
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
            CompleteText();
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
            virtualCamera.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }
    
    private void ActivateStep(int index)
    {
        StepData currentStep = steps[index];

        if (currentStep.textUI != null)
        {
            currentStep.textUI.gameObject.SetActive(true);
            currentFullText = currentStep.dialogueText;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(currentStep.textUI, currentStep.dialogueText));
        }

        if (currentStep.audioSource != null && currentStep.audioClip != null)
        {
            if (currentStep.audioSource.isPlaying) currentStep.audioSource.Stop();
            currentStep.audioSource.clip = currentStep.audioClip;
            currentStep.audioSource.Play();
        }

        if (currentStep.panelImage != null)
        {
            StartCoroutine(FadeObject(currentStep.panelImage));
        }
    }

    private void CompleteText()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        steps[currentStepIndex].textUI.text = currentFullText;
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