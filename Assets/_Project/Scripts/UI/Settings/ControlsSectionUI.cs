using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[Serializable]
public class KeybindMapping
{
    public string actionName;           
    public string playerPrefsKey;       
    public KeyCode defaultKey;          
    public Button rebindButton;         
    public TextMeshProUGUI buttonText;  
}

public class ControlsSectionUI : MonoBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField] Slider mouseSensibilitySlider;
    [SerializeField] Toggle invertY;
    [SerializeField] Toggle invertX;

    [Header("Keybind Rebinders")]
    [Tooltip("Añade aquí todas las acciones del juego (+). Asigna su llave de PlayerPrefs, botón y texto individual.")]
    [SerializeField] KeybindMapping[] keybindMappings;

    [Header("Action Buttons")]
    [SerializeField] Button resetInputButton;
    [SerializeField] Button saveInputButton;
    private bool isRebinding = false;

    void Start()
    {
        LoadVisuals();
        AssignListeners();
    }

    private void LoadVisuals()
    {
        if (mouseSensibilitySlider != null) mouseSensibilitySlider.value = SettingsDataManager.MouseSensibility;
        if (invertY != null) invertY.isOn = SettingsDataManager.InvertY;
        if (invertX != null) invertX.isOn = SettingsDataManager.InvertX;

        foreach (KeybindMapping mapping in keybindMappings)
        {
            if (mapping.buttonText != null)
            {
                string savedKey = PlayerPrefs.GetString(mapping.playerPrefsKey, mapping.defaultKey.ToString());
                mapping.buttonText.text = savedKey;
            }
        }
    }

    private void AssignListeners()
    {
        if (mouseSensibilitySlider != null) mouseSensibilitySlider.onValueChanged.AddListener(v => SettingsDataManager.MouseSensibility = v);
        if (invertY != null) invertY.onValueChanged.AddListener(v => SettingsDataManager.InvertY = v);
        if (invertX != null) invertX.onValueChanged.AddListener(v => SettingsDataManager.InvertX = v);

        foreach (KeybindMapping mapping in keybindMappings)
        {
            if (mapping.rebindButton != null)
            {
                KeybindMapping currentMapping = mapping;

                currentMapping.rebindButton.onClick.AddListener(() => StartRebinding(currentMapping.playerPrefsKey, currentMapping.buttonText));
            }
        }
        // 1. Conexión de los nuevos botones
        if (resetInputButton != null) resetInputButton.onClick.AddListener(ResetToDefaults);
        if (saveInputButton != null) saveInputButton.onClick.AddListener(SaveAndApply);
    }

    // --- MÉTODOS DE ACCIÓN ---

    private void ResetToDefaults()
    {
        // 2. DATO IMPORTANTE: Restauración del Mouse
        // Al modificar el '.value' o '.isOn', se disparan automáticamente los eventos 'onValueChanged'
        // que asignamos arriba. Esto significa que el 'SettingsDataManager' se actualizará solo.
        if (mouseSensibilitySlider != null) mouseSensibilitySlider.value = 100; // Asumiendo 50 como valor por defecto
        if (invertY != null) invertY.isOn = false;
        if (invertX != null) invertX.isOn = false;

        foreach (KeybindMapping mapping in keybindMappings)
        {
            PlayerPrefs.SetString(mapping.playerPrefsKey, mapping.defaultKey.ToString());

            if (mapping.buttonText != null)
            {
                mapping.buttonText.text = mapping.defaultKey.ToString();
            }
        }

        SaveAndApply();
        Debug.Log("[Controls UI] Se han restaurado los valores por defecto.");
    }

    private void SaveAndApply()
    {
        PlayerPrefs.Save();

        if (InputManager.Instance != null)
        {
            InputManager.Instance.LoadAllKeybinds();
        }

        Debug.Log("[Controls UI] Configuraciones guardadas y aplicadas.");
    }

    private void StartRebinding(string prefKey, TextMeshProUGUI buttonText)
    {
        if (isRebinding) return;
        StartCoroutine(WaitForKeyPress(prefKey, buttonText));
    }

    private IEnumerator WaitForKeyPress(string prefKey, TextMeshProUGUI buttonText)
    {
        isRebinding = true;
        buttonText.text = "...";

        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                PlayerPrefs.SetString(prefKey, keyCode.ToString());
                buttonText.text = keyCode.ToString();
                break;
            }
        }

        WaitForSecondsRealtime waitForSecondsRealtime = new(0.1f);
        yield return waitForSecondsRealtime;
        isRebinding = false;
    }
}