using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct StateProgressionStep
{
    [Tooltip("El valor exacto de 'currentState' para que este evento ocurra.")]
    public int requiredStateValue;

    [TextArea(2, 4)]
    public string stateDisplayText;

    public UnityEvent onStateTriggered;
}
public class StateProgressionManager : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Arrastra aquí tu componente de texto de la jerarquía de Canvas.")]
    [SerializeField] TextMeshProUGUI stateTextMesh;

    [Header("System Memory")]
    [Tooltip("NO TOCAR, es para ver el state como debug")]
    [SerializeField] int currentState = 0;

    [Header("Configuración de Estados")]
    [SerializeField] StateProgressionStep[] stateSequence;

    readonly Dictionary<int, StateProgressionStep> quickLookup = new();

    void Awake()
    {
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
    /// <summary>
    /// Función pública para sumar puntos. llamala en una acción de UnityEvent para que aumente el contador.
    /// </summary>
    public void AddStatePoint()
    {
        currentState++;
        EvaluateState();
    }
    /// <summary>
    /// (Opcional) Función de emergencia por si un puzzle requiere forzar un salto a un número exacto.
    /// </summary>
    public void SetExactState(int exactTargetState)
    {
        currentState = exactTargetState;
        EvaluateState();
    }

    private void EvaluateState()
    {
        if (quickLookup.TryGetValue(currentState, out StateProgressionStep currentStep))
        {
            if (stateTextMesh != null) stateTextMesh.text = currentStep.stateDisplayText;
            else Debug.LogWarning("[StateManager] El texto cambió, pero el TextMeshProUGUI no esta en el inspector.");
            currentStep.onStateTriggered?.Invoke();
        }
    }
}
