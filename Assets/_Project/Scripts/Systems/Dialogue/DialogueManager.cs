using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Main Camera Reference")]
    [SerializeField] CinemachineCamera playerCamera;
    CinemachinePanTilt panTiltComponent;
    Coroutine typeCoroutine;
    Coroutine lookCoroutine;

    [Header("Mouse Interruption Settings")]
    [Tooltip("Tiempo decimal (en segundos) que el jugador debe mover el mouse seguido para cancelar el apuntado.")]
    [SerializeField] float mouseInterruptDuration = 0.3f;
    [SerializeField] float mouseSensitivityThreshold = 0.001f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (playerCamera != null) panTiltComponent = playerCamera.GetComponent<CinemachinePanTilt>();
    }

    public void StartDialogue(TMP_Text textUI, Transform target, string[] textLines, float rotationSpeed, bool lookAtTarget, float durationPerLine)
    {
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        if (lookCoroutine != null) StopCoroutine(lookCoroutine);

        typeCoroutine = StartCoroutine(TypingTextSequence(textUI, textLines, durationPerLine));

        if (lookAtTarget && target != null && panTiltComponent != null)
        {
            lookCoroutine = StartCoroutine(LookAtTarget(target, rotationSpeed));
        }
    }

    void SetCharacterAlpha(TMP_Text textComponent, int charIndex, byte alpha)
    {
        int materialIndex = textComponent.textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textComponent.textInfo.characterInfo[charIndex].vertexIndex;
        Color32[] vertexColors = textComponent.textInfo.meshInfo[materialIndex].colors32;

        if (textComponent.textInfo.characterInfo[charIndex].isVisible)
        {
            vertexColors[vertexIndex + 0].a = alpha;
            vertexColors[vertexIndex + 1].a = alpha;
            vertexColors[vertexIndex + 2].a = alpha;
            vertexColors[vertexIndex + 3].a = alpha;
        }
    }

    IEnumerator TypingTextSequence(TMP_Text textComponent, string[] texts, float duration)
    {
        float timePerCharacter = 0.03f;
        float fadeDuration = 0.4f;

        foreach (string textLine in texts)
        {
            textComponent.text = textLine;
            textComponent.ForceMeshUpdate();
            TMP_TextInfo textInfo = textComponent.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                SetCharacterAlpha(textComponent, i, 0);
            }
            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            // Escribir texto
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;
                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime / timePerCharacter;
                    byte currentAlpha = (byte)Mathf.Lerp(0, 255, t);
                    SetCharacterAlpha(textComponent, i, currentAlpha);
                    textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                    yield return null;
                }
            }

            yield return new WaitForSeconds(duration);

            float fadeTimer = 0f;
            while (fadeTimer < fadeDuration)
            {
                fadeTimer += Time.deltaTime;
                float normalizedTime = 1f - (fadeTimer / fadeDuration);
                byte targetAlpha = (byte)Mathf.Lerp(0, 255, normalizedTime);

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;
                    SetCharacterAlpha(textComponent, i, targetAlpha);
                }

                textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }
        }

        textComponent.text = "";
    }

    IEnumerator LookAtTarget(Transform target, float providedSpeedMultiplier)
    {
        float angleTolerance = 0.5f;
        float mouseMoveTimer = 0f;

        float constantBaseSpeed = 45f;

        while (true)
        {
            if (target == null) break;

            Vector3 dirToTarget = target.position - playerCamera.transform.position;
            if (dirToTarget == Vector3.zero) break;

            Quaternion targetRotation = Quaternion.LookRotation(dirToTarget.normalized);
            float idealPan = targetRotation.eulerAngles.y;
            float idealTilt = targetRotation.eulerAngles.x;

            float deltaPan = Mathf.DeltaAngle(panTiltComponent.PanAxis.Value, idealPan);
            float deltaTilt = Mathf.DeltaAngle(panTiltComponent.TiltAxis.Value, idealTilt);

            Vector2 angleDelta = new(deltaPan, deltaTilt);
            float distance = angleDelta.magnitude;

            if (distance <= angleTolerance) break;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(mouseX) > mouseSensitivityThreshold || Mathf.Abs(mouseY) > mouseSensitivityThreshold)
            {
                mouseMoveTimer += Time.deltaTime;
                if (mouseMoveTimer >= mouseInterruptDuration) break;
            }
            else mouseMoveTimer = 0f;

            Vector2 normalizedDelta = angleDelta.normalized;
            float step = constantBaseSpeed * providedSpeedMultiplier * Time.deltaTime;

            if (step > distance) step = distance;

            panTiltComponent.PanAxis.Value += normalizedDelta.x * step;
            panTiltComponent.TiltAxis.Value += normalizedDelta.y * step;

            yield return null;
        }
    }
}
