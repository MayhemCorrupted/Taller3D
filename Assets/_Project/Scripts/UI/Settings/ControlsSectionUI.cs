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
    [Header("Keybind Rebinders")]
    [Tooltip("Añade aquí todas las acciones del juego (+). Asigna su llave de PlayerPrefs, botón y texto individual.")]
    [SerializeField] KeybindMapping[] keybindMappings;

    [Header("Action Buttons")]
    [SerializeField] Button resetInputButton;
    private bool isRebinding = false;

    void Start()
    {
        LoadVisuals();
        AssignListeners();
    }

    private void LoadVisuals()
    {
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
        foreach (KeybindMapping mapping in keybindMappings)
        {
            if (mapping.rebindButton != null)
            {
                KeybindMapping currentMapping = mapping;

                currentMapping.rebindButton.onClick.AddListener(() => StartRebinding(currentMapping.playerPrefsKey, currentMapping.buttonText));
            }
        }
        if (resetInputButton != null) resetInputButton.onClick.AddListener(ResetToDefaults);
    }
    private void ResetToDefaults()
    {
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

        yield return null;

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

        SaveAndApply();
        WaitForSecondsRealtime waitForSecondsRealtime = new(0.1f);
        yield return waitForSecondsRealtime;
        isRebinding = false;
    }
}