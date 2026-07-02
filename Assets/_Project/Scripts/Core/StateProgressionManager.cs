using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct StateProgressionStep
{
    [Tooltip("El valor exacto de 'currentState' para que este evento ocurra.")]
    public int requiredStateValue;
    [TextArea(2, 4)] public string stateDisplayText;
    public UnityEvent onStateTriggered;
}
public class StateProgressionManager : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Arrastra aquí tu componente de texto de la jerarquía de Canvas.")]
    [SerializeField] TextMeshProUGUI stateTextMesh;
    [SerializeField] CanvasGroup indicatorCG;
    [SerializeField] float fadeSpeed = 0.5f;
    [SerializeField] float fadeDuration = 1.0f;
    [SerializeField] float fadeHoldDuration = 1.0f;

    [Header("System Memory")]
    [Tooltip("NO TOCAR, es para debuggear")]
    [SerializeField] int currentState = 0;
    [Header("Configuración de Estados")]
    [SerializeField] StateProgressionStep[] stateSequence;

    readonly Dictionary<int, StateProgressionStep> quickLookup = new();
    readonly HashSet<string> triggeredHints = new();
    void Awake()
    {
        currentState = 0;
        if (indicatorCG != null) indicatorCG.alpha = 0;
        if (stateTextMesh != null) stateTextMesh.text = string.Empty;
        foreach (var step in stateSequence)
        {
            if (!quickLookup.TryAdd(step.requiredStateValue, step))
            {
                Debug.LogError($"[StateManager] Error: valor repetido ({step.requiredStateValue}). Revisa el Inspector.");
            }
        }
    }
    void Start()
    {
        EvaluateState();
    }
    public void AddStatePoint()
    {
        currentState++;
        EvaluateState();
    }
    public void SetExactState(int exactTargetState)
    {
        currentState = exactTargetState;
        EvaluateState();
    }
    /// <summary>
    /// Permite sobreescribir el texto del indicador de estado, con la opción de que solo se muestre una vez si se antepone "[ONCE]" al texto. Ejemplo: "[ONCE]Ahora debo hacer tal cosa."
    /// </summary>
    /// <param name="newHint"> El nuevo texto para el indicador de estado del hint.</param>
    public void OverrideHintText(string newHint)
    {
        bool isRunOnce = false;
        string displayText = newHint;

        if (newHint.StartsWith("[ONCE]"))
        {
            isRunOnce = true;
            displayText = newHint.Replace("[ONCE]", "").Trim();
        }

        if (isRunOnce)
        {
            if (triggeredHints.Contains(displayText)) return;
            triggeredHints.Add(displayText);
        }
        stateTextMesh.text = displayText;
        StopAllCoroutines();
        if (indicatorCG != null) StartCoroutine(FadeIndicator());
    }
    void EvaluateState()
    {
        if (quickLookup.TryGetValue(currentState, out StateProgressionStep currentStep))
        {
            if (indicatorCG != null) StartCoroutine(FadeIndicator());
            if (stateTextMesh != null) stateTextMesh.text = currentStep.stateDisplayText;
            else Debug.LogWarning("[StateManager] El texto cambió, pero el TextMeshProUGUI no esta en el inspector.");
            currentStep.onStateTriggered?.Invoke();
        }
    }
    IEnumerator FadeIndicator()
    {
        indicatorCG.alpha = 0f;
        yield return StartCoroutine(SetFade(0f, 1f, fadeDuration, fadeSpeed));
        yield return new WaitForSeconds(fadeHoldDuration);
        yield return StartCoroutine(SetFade(1f, 0f, fadeDuration, fadeSpeed));
    }
    IEnumerator SetFade(float startAlpha, float targetAlpha, float duration, float speed)
    {
        float elapsedTime = 0f;
        indicatorCG.alpha = startAlpha;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime * speed;
            indicatorCG.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        indicatorCG.alpha = targetAlpha;
    }
    
}
