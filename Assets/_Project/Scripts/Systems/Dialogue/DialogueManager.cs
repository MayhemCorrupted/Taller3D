using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public bool IsDialogueActive { get; private set; }
    [Header("Main Camera Reference")]
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera playerCamera;
    CinemachinePanTilt panTiltComponent;
    Coroutine typeCoroutine;
    Coroutine lookCoroutine;
    Coroutine fallbackCoroutine;

    [Header("Default Values")]
    [Tooltip("Velocidad de tipeo por defecto en segundos por carácter.")]
    [SerializeField] float defaultTimePerCharacter = 0.015f;
    [Tooltip("Tiempo de fade out por defecto en segundos.")]
    [SerializeField] float defaultFadeDuration = 0.4f;

    [Header("UI Fallback Settings (For 3D)")]
    [Tooltip("El panel que aparecerá cuando el texto 3D salga de la pantalla.")]
    [SerializeField] CanvasGroup fallbackCanvasGroup;
    [SerializeField] TMP_Text fallbackUIText;
    [SerializeField] float fallbackFadeSpeed = 5f;

    [Header("Camera & Mouse Settings")]
    [Tooltip("Tiempo de suavizado para que la cámara gire (menor = más rápido, mayor = más suave).")]
    [SerializeField] float cameraSmoothTime = 0.2f;
    [Tooltip("Segundos moviendo el mouse para cancelar el apuntado.")]
    [SerializeField] float mouseInterruptDuration = 0.3f;
    [SerializeField] float mouseSensitivityThreshold = 0.001f;

    bool isUsingFallbackUI = false;
    bool isLookInterrupted = false;
    Transform currentLookTarget = null;
    DialogueType currentLineType;
    TMP_Text currentActiveMainText = null;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (playerCamera != null) panTiltComponent = playerCamera.GetComponent<CinemachinePanTilt>();
        if (fallbackCanvasGroup != null) fallbackCanvasGroup.alpha = 0f;
    }
    public bool StartDialogue(DialogueEntry data)
    {
        if (IsDialogueActive) StopCurrentDialogue();

        IsDialogueActive = true;
        isLookInterrupted = false;
        currentLookTarget = null;

        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        if (lookCoroutine != null) StopCoroutine(lookCoroutine);
        if (fallbackCoroutine != null) StopCoroutine(fallbackCoroutine);

        typeCoroutine = StartCoroutine(TypingTextSequence(data));

        fallbackCoroutine = StartCoroutine(Track3DTextVisibility());
        return true; 
    }
    public void StopCurrentDialogue()
    {
        CancelInvoke();

        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        if (lookCoroutine != null) StopCoroutine(lookCoroutine);
        if (fallbackCoroutine != null) StopCoroutine(fallbackCoroutine);

        if (currentActiveMainText != null) PrepareTextComponent(currentActiveMainText, "");
        if (fallbackUIText != null) PrepareTextComponent(fallbackUIText, "");
        if (fallbackCanvasGroup != null) fallbackCanvasGroup.alpha = 0f;

        currentLookTarget = null;
        IsDialogueActive = false;
    }
    IEnumerator TypingTextSequence(DialogueEntry data)
    {
        TMP_Text activeMainText = null;
        float currentDuration = 3f;

        foreach (var lineData in data.dialogueSequence)
        {
            lineData.onLineStart?.Invoke();
            currentLineType = lineData.dialogueType;

            if (lineData.targetToLookAt != null)
            {
                if (lineData.targetToLookAt != currentLookTarget)
                {
                    currentLookTarget = lineData.targetToLookAt;
                    isLookInterrupted = false;

                    if (lookCoroutine != null) StopCoroutine(lookCoroutine);
                    if (panTiltComponent != null) lookCoroutine = StartCoroutine(LookAtTarget(currentLookTarget));
                }
            }
            else if (currentLookTarget == null && currentLineType == DialogueType.World_3D)
            {
                Debug.LogWarning("[DialogueManager] Línea puesto como World_3D no cuenta con un targetToLookAt, ojito con eso.");
            }

            if (lineData.textMeshComponent != null) activeMainText = lineData.textMeshComponent;
            currentActiveMainText = activeMainText;

            isUsingFallbackUI = (currentLineType == DialogueType.World_3D) || (currentLineType == DialogueType.UI && activeMainText == null);

            if (lineData.lineTypingDuration > 0f) currentDuration = lineData.lineTypingDuration;

            float timePerCharacter = lineData.timePerCharacter > 0f ? lineData.timePerCharacter : defaultTimePerCharacter; ;
            float fadeDuration = lineData.fadeDuration > 0f ? lineData.fadeDuration : defaultFadeDuration; ;

            string currentText = lineData.textLine;

            PrepareTextComponent(activeMainText, currentText);
            if (currentLineType == DialogueType.World_3D) PrepareTextComponent(fallbackUIText, currentText);

            int charCount = activeMainText != null ? activeMainText.textInfo.characterCount : (isUsingFallbackUI && fallbackUIText != null ? fallbackUIText.textInfo.characterCount : 0);

            for (int i = 0; i < charCount; i++)
            {
                bool isVisibleInMain = activeMainText != null && activeMainText.textInfo.characterInfo[i].isVisible;
                bool isVisibleInFallback = isUsingFallbackUI && fallbackUIText != null && fallbackUIText.textInfo.characterInfo[i].isVisible;
               
                if (activeMainText != null && !activeMainText.textInfo.characterInfo[i].isVisible) continue;
                
                if (!isVisibleInMain && !isVisibleInFallback) continue;
                
                float t = 0;
                
                while (t < 1)
                {
                    t += Time.deltaTime / timePerCharacter;
                    byte currentAlpha = (byte)Mathf.Lerp(0, 255, t);

                    SetCharacterAlpha(activeMainText, i, currentAlpha);
                    if (currentLineType == DialogueType.World_3D) SetCharacterAlpha(fallbackUIText, i, currentAlpha);

                    yield return null;
                }
            }

            yield return new WaitForSeconds(currentDuration);

            float fadeTimer = 0f;
            while (fadeTimer < fadeDuration)
            {
                fadeTimer += Time.deltaTime;
                float normalizedTime = 1f - (fadeTimer / fadeDuration);
                byte targetAlpha = (byte)Mathf.Lerp(0, 255, normalizedTime);

                for (int i = 0; i < charCount; i++)
                {
                    SetCharacterAlpha(activeMainText, i, targetAlpha);
                    if (currentLineType == DialogueType.World_3D) SetCharacterAlpha(fallbackUIText, i, targetAlpha);
                }
                yield return null;
            }
        }

        if (fallbackCoroutine != null) StopCoroutine(fallbackCoroutine);
        if (fallbackCanvasGroup != null) fallbackCanvasGroup.alpha = 0f;

        currentLookTarget = null;
        IsDialogueActive = false;
    }
    void PrepareTextComponent(TMP_Text textComponent, string textLine)
    {
        if (textComponent == null) return;
        textComponent.text = textLine;
        textComponent.ForceMeshUpdate();

        for (int i = 0; i < textComponent.textInfo.characterCount; i++)
        {
            SetCharacterAlpha(textComponent, i, 0);
        }
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
    void SetCharacterAlpha(TMP_Text textComponent, int charIndex, byte alpha)
    {
        if (textComponent == null || charIndex >= textComponent.textInfo.characterInfo.Length) return;
        if (!textComponent.textInfo.characterInfo[charIndex].isVisible) return;

        int materialIndex = textComponent.textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textComponent.textInfo.characterInfo[charIndex].vertexIndex;
        Color32[] vertexColors = textComponent.textInfo.meshInfo[materialIndex].colors32;

        vertexColors[vertexIndex + 0].a = alpha;
        vertexColors[vertexIndex + 1].a = alpha;
        vertexColors[vertexIndex + 2].a = alpha;
        vertexColors[vertexIndex + 3].a = alpha;
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
    IEnumerator Track3DTextVisibility()
    {
        if (fallbackCanvasGroup == null || mainCamera == null) yield break;

        while (true)
        {
            if (currentLineType == DialogueType.UI)
            {
                float targetAlpha = isUsingFallbackUI ? 1f : 0f;
                fallbackCanvasGroup.alpha = Mathf.Lerp(fallbackCanvasGroup.alpha, targetAlpha, Time.deltaTime * fallbackFadeSpeed);
            }
            else
            {
                bool isOffScreen = false;
                if (currentLookTarget != null)
                {
                    Vector3 viewportPos = mainCamera.WorldToViewportPoint(currentLookTarget.position);
                    isOffScreen = viewportPos.z < 0 || viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;
                }

                bool isActivelyLooking = (currentLookTarget != null && !isLookInterrupted);
                float targetAlpha = isActivelyLooking ? 0f : (isOffScreen ? 1f : 0f);

                fallbackCanvasGroup.alpha = Mathf.Lerp(fallbackCanvasGroup.alpha, targetAlpha, Time.deltaTime * fallbackFadeSpeed);
            }
            yield return null;
        }
    }
    IEnumerator LookAtTarget(Transform target)
    {
        float angleTolerance = 0.7f;
        float mouseMoveTimer = 0f;

        float panVelocity = 0f;
        float tiltVelocity = 0f;

        while (true)
        {
            if (target == null) break;

            Vector3 dirToTarget = target.position - playerCamera.transform.position;
            if (dirToTarget == Vector3.zero) break;

            Quaternion targetRotation = Quaternion.LookRotation(dirToTarget.normalized);
            float idealPan = targetRotation.eulerAngles.y;
            float idealTilt = targetRotation.eulerAngles.x;

            float currentPan = panTiltComponent.PanAxis.Value;
            float currentTilt = panTiltComponent.TiltAxis.Value;

            float deltaPan = Mathf.DeltaAngle(currentPan, idealPan);
            float deltaTilt = Mathf.DeltaAngle(currentTilt, idealTilt);

            if (Mathf.Abs(deltaPan) <= angleTolerance && Mathf.Abs(deltaTilt) <= angleTolerance)
            {
                isLookInterrupted = true;
                break;
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(mouseX) > mouseSensitivityThreshold || Mathf.Abs(mouseY) > mouseSensitivityThreshold)
            {
                mouseMoveTimer += Time.deltaTime;
                if (mouseMoveTimer >= mouseInterruptDuration)
                {
                    isLookInterrupted = true;
                    break;
                }
            }
            else mouseMoveTimer = 0f;

            panTiltComponent.PanAxis.Value = Mathf.SmoothDampAngle(currentPan, currentPan + deltaPan, ref panVelocity, cameraSmoothTime);
            panTiltComponent.TiltAxis.Value = Mathf.SmoothDampAngle(currentTilt, currentTilt + deltaTilt, ref tiltVelocity, cameraSmoothTime);

            yield return null;
        }
    }
}
