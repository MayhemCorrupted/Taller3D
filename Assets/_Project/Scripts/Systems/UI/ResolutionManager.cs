using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;


    private Resolution[] availableResolutions;
    private int currentResolutionIndex = 0;

    void Awake()
    {

        ResolutionManager[] managers = FindObjectsByType<ResolutionManager>(FindObjectsSortMode.None);

        if (managers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
        availableResolutions = Screen.resolutions;


        resolutionDropdown.ClearOptions();

     
        var options = new System.Collections.Generic.List<string>();
        for (int i = 0; i < availableResolutions.Length; i++)
        {

            string option = availableResolutions[i].width + " x " +
                           availableResolutions[i].height + " @ ";
                           
            options.Add(option);

        
            if (availableResolutions[i].width == Screen.currentResolution.width &&
                availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

      
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

      
        fullscreenToggle.isOn = Screen.fullScreen;

        
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = availableResolutions[resolutionIndex];

        
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        Debug.Log($"Resolución cambiada a: {resolution.width}x{resolution.height}");
    }


    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log($"Pantalla completa: {isFullscreen}");
    }

 
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionWidth", Screen.width);
        PlayerPrefs.SetInt("ResolutionHeight", Screen.height);
        PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.Save();
    }

    
    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int savedIndex = PlayerPrefs.GetInt("ResolutionIndex");
            bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;

            resolutionDropdown.value = savedIndex;
            fullscreenToggle.isOn = savedFullscreen;

            SetResolution(savedIndex);
            SetFullscreen(savedFullscreen);
        }
    }

    void OnDestroy()
    {
        
        resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
    }
}
