using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    enum SettingsState { Main, Sound, Controls }
    SettingsState currentState = SettingsState.Main;

    [Header("General/Display Settings (Main)")]
    [SerializeField] TextMeshProUGUI bannerTMP;

    [Tooltip("Este es tu panel principal (el Display / Opciones base)")]
    [SerializeField] GameObject mainDisplayPanel;

    [Tooltip("Coloca aquí TODOS los botones de 'Regresar' o 'Atrás' que existan en tus paneles")]
    [SerializeField] Button[] returnButtons;

    [Header("Sub-Panels")]
    [SerializeField] GameObject soundPanel;
    [SerializeField] GameObject controlPanel;

    [Header("Menu Navigation Buttons")]
    [SerializeField] Button soundButton;
    [SerializeField] Button controlButton;

    void Awake()
    {
        if (returnButtons != null && returnButtons.Length > 0)
        {
            foreach (Button btn in returnButtons)
            {
                if (btn != null) btn.onClick.AddListener(ReturnToPreviousState);
            }
        }

        if (soundButton != null) soundButton.onClick.AddListener(() => ChangeState(SettingsState.Sound));
        if (controlButton != null) controlButton.onClick.AddListener(() => ChangeState(SettingsState.Controls));

        ChangeState(SettingsState.Main);
    }

    void ChangeState(SettingsState newState)
    {
        currentState = newState;

        if (bannerTMP != null)
        {
            bannerTMP.text = currentState == SettingsState.Main ? "Settings" :
                             currentState == SettingsState.Sound ? "Sound" : "Controls";
        }

        if (mainDisplayPanel != null) mainDisplayPanel.SetActive(false);
        if (soundPanel != null) soundPanel.SetActive(false);
        if (controlPanel != null) controlPanel.SetActive(false);

        switch (currentState)
        {
            case SettingsState.Main:
                if (mainDisplayPanel != null) mainDisplayPanel.SetActive(true);
                break;
            case SettingsState.Sound:
                if (soundPanel != null) soundPanel.SetActive(true);
                break;
            case SettingsState.Controls:
                if (controlPanel != null) controlPanel.SetActive(true);
                break;
        }
    }

    void ReturnToPreviousState()
    {
        if (currentState != SettingsState.Main) ChangeState(SettingsState.Main);
        else gameObject.SetActive(false);
    }
}
