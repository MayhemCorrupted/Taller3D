using UnityEngine;
using UnityEngine.UI;
public class CameraSettingsUI : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Toggle invertXToggle;
    [SerializeField] Toggle invertYToggle;
    [SerializeField] Button resetValuesButton;

    [Header("Default Values")]
    [SerializeField] float defaultSensitivity = 100f;
    [SerializeField] bool defaultInvertX = false;
    [SerializeField] bool defaultInvertY = false;

    private void Awake()
    {
        resetValuesButton.onClick.AddListener(() =>
        {
            ApplyDefaultValues();
            SaveSettings();
        });
    }
    void Start()
    {
        LoadSavedValues();
        AssignListeners();
        ApplyDefaultValues();
    }
    private void LoadSavedValues()
    {
        if (sensitivitySlider != null) sensitivitySlider.value = SettingsDataManager.MouseSensibility;

        if (invertXToggle != null) invertXToggle.isOn = SettingsDataManager.InvertX;

        if (invertYToggle != null) invertYToggle.isOn = SettingsDataManager.InvertY;
    }
    private void AssignListeners()
    {
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(v => { SettingsDataManager.MouseSensibility = v; SaveSettings(); });

        if (invertXToggle != null)
            invertXToggle.onValueChanged.AddListener(v => { SettingsDataManager.InvertX = v; SaveSettings(); });

        if (invertYToggle != null)
            invertYToggle.onValueChanged.AddListener(v => { SettingsDataManager.InvertY = v; SaveSettings(); });
    }

    private void ApplyDefaultValues()
    {
        if (sensitivitySlider != null) sensitivitySlider.value = defaultSensitivity;
        if (invertXToggle != null) invertXToggle.isOn = defaultInvertX;
        if (invertYToggle != null) invertYToggle.isOn = defaultInvertY;
        Debug.Log("[Camera UI] Se han restaurado los valores por defecto.");
    }
    private void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("[Camera UI] Cambio detectado. Configuraciones de cámara actualizadas y guardadas.");
    }
}