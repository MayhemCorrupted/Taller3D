using UnityEngine;
using UnityEngine.UI;
public class CameraSettingsUI : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Toggle invertXToggle;
    [SerializeField] Toggle invertYToggle;

    [Header("Default Values")]
    [SerializeField] float defaultSensitivity = 100f;
    [SerializeField] bool defaultInvertX = false;
    [SerializeField] bool defaultInvertY = false;

    void Start()
    {
        AssignListeners();
        ApplyDefaultValues();
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
    }
    private void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("[Camera UI] Cambio detectado. Configuraciones de cámara actualizadas y guardadas.");
    }
}