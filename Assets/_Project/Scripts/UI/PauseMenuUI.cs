using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] string sceneName = "MainMenuScene";

    [Header("Principal Buttons")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button quitButton;

    [Header("Settings Buttons")]
    [SerializeField] Button backFromSettingsButton;

    void Awake()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
        if (backFromSettingsButton != null) backFromSettingsButton.onClick.AddListener(CloseSettings);

        if (UserInterfaceManager.Instance != null)
            UserInterfaceManager.Instance.RegisterPanel(
                UserInterfaceManager.PanelType.Pause, () => TogglePause(true));
    }

    void Update()
    {
        if (InputManager.Instance == null) return;
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        bool anyOtherOpen = UserInterfaceManager.Instance.IsAnyPanelOpen()
                            && !UserInterfaceManager.Instance.IsPauseOpen;
        if (anyOtherOpen) return;

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

        bool isOpening = !pausePanel.activeSelf;

        if (isOpening)
            UserInterfaceManager.Instance.RequestOpenPanel(UserInterfaceManager.PanelType.Pause);
        else
            UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Pause);

        TogglePause(isOpening);
    }

    public void TogglePause(bool state)
    {

        if (pausePanel != null) pausePanel.SetActive(state);
        Time.timeScale = state ? 0f : 1f;

        
        if (state) AudioManager.Instance?.PauseAll();
        else AudioManager.Instance?.ResumeAll();
    }

    void ResumeGame()
    {
        if (UserInterfaceManager.Instance != null)
            UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Pause);
        TogglePause(false);

        AudioManager.Instance?.ResumeAll();
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