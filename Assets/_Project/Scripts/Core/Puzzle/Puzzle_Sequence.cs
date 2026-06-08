using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle_Sequence : MonoBehaviour
{
    [SerializeField] Button[] numpadButtons;
    [SerializeField] Image[] sequenceLights;
    [Header("Settings")]
    [SerializeField] Color lightOffColor = Color.black;
    [SerializeField] Color lightCorrectColor = Color.green;
    [SerializeField] Color lightErrorColor = Color.red;
    [SerializeField] UnityEngine.Events.UnityEvent OnSolved;
    private int currentSequenceStep = 0; 
    private int expectedNextValue = 0;   
    private bool disablePuzzle = false;    
    void Start()
    {
        InitializePuzzle();
    }

    void InitializePuzzle()
    {
        for (int i = 0; i < numpadButtons.Length; i++)
        {
            int buttonValue = i + 1; 
            Button button = numpadButtons[i];

            if (button != null) button.onClick.AddListener(() => OnButtonPressed(buttonValue));
        }

        ResetLights();
    }

    void OnButtonPressed(int pressedValue)
    {
        if (disablePuzzle || currentSequenceStep >= 9) return;

        if (currentSequenceStep == 0 || pressedValue == expectedNextValue) ProcessCorrectPress(pressedValue);
        else ProcessError();
    }

    void ProcessCorrectPress(int pressedValue)
    {
        if (currentSequenceStep < sequenceLights.Length) 
            sequenceLights[currentSequenceStep].color = lightCorrectColor;

        currentSequenceStep++;

        expectedNextValue = (pressedValue % 9) + 1;
        if (currentSequenceStep >= 9) CompletePuzzle();
    }

    void ProcessError()
    {
        StartCoroutine(ErrorResetRoutine());
    }

    IEnumerator ErrorResetRoutine()
    {
        WaitForSeconds waitForSeconds = new(0.5f);
        disablePuzzle = true; 

        if (currentSequenceStep < sequenceLights.Length) 
            sequenceLights[currentSequenceStep].color = lightErrorColor;
        yield return waitForSeconds;

        currentSequenceStep = 0;
        expectedNextValue = 0;
        ResetLights();

        disablePuzzle = false;
    }

    void ResetLights()
    {
        foreach (Image light in sequenceLights) 
            if (light != null) light.color = lightOffColor;
    }

    void CompletePuzzle()
    {
        Debug.Log("[Puzzle Caja Fuerte] Secuencia completada. Abriendo compartimento...");
        OnSolved?.Invoke();
    }
}
