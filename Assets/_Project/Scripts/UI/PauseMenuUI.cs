using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] string sceneName = "MainMenuScene";
    [SerializeField] KeyCode menuKey = KeyCode.Escape;

    [Header("Principal Buttons")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button quitButton;

    [Header("Settings Buttons")]
    [SerializeField] Button backFromSettingsButton;
    public bool CanPause { get; set; } = true;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
        if (backFromSettingsButton != null) backFromSettingsButton.onClick.AddListener(CloseSettings);
       UserInterfaceManager.Instance.RegisterPanel(
            UserInterfaceManager.PanelType.Pause,
            () => TogglePause(true),
            () => TogglePause(false)
        );
    }

    void Update()
    {
        if (InputManager.Instance == null) return;
        if (!Input.GetKeyDown(menuKey)) return;
        if (!CanPause) return;

        UserInterfaceManager.Instance.TogglePanel(UserInterfaceManager.PanelType.Pause);

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }
    }

    public void TogglePause(bool isPaused)
    {
        if (pausePanel != null) pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (AudioManager.Instance != null)
        {
            if (isPaused) AudioManager.Instance.PauseGlobalAudio();
            else AudioManager.Instance.ResumeGlobalAudio();
        }

    }

    void ResumeGame()
    {
        UserInterfaceManager.Instance.ClosePanel(UserInterfaceManager.PanelType.Pause);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (InputManager.Instance != null) InputManager.Instance.LoadAllKeybinds();
    }

    void QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
    void OnDestroy()
    {
        if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (quitButton != null) quitButton.onClick.RemoveAllListeners();
        if (backFromSettingsButton != null) backFromSettingsButton.onClick.RemoveAllListeners();
    }
}