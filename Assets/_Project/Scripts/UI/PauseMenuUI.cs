using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject settingsPanel;

    [Header("Principal Buttons")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button settingsButton;
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
        if (backFromSettingsButton != null) backFromSettingsButton.onClick.AddListener(CloseSettings);
        if (UserInterfaceManager.Instance != null)
        {
            UserInterfaceManager.Instance.RegisterPanel(UserInterfaceManager.PanelType.Pause, () => TogglePause(true));
        }
    }

    void Update()
    {
        if (InputManager.Instance != null && Input.GetKeyDown(InputManager.Instance.PauseKey))
        {
            if (UserInterfaceManager.Instance.IsAnyPanelOpen() && !UserInterfaceManager.Instance.IsPauseOpen) return;

            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                bool isOpening = !pausePanel.activeSelf;
                if (isOpening) UserInterfaceManager.Instance.RequestOpenPanel(UserInterfaceManager.PanelType.Pause);
                else UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Pause);

                TogglePause(isOpening);
            }
        }
    }
    public void TogglePause(bool state)
    {
        if (pausePanel != null) pausePanel.SetActive(state);
        Time.timeScale = state ? 0f : 1f;
    }
    private void ResumeGame()
    {
        if (UserInterfaceManager.Instance != null)
        {
            UserInterfaceManager.Instance.ReportClosedPanel(UserInterfaceManager.PanelType.Pause);
        }
        TogglePause(false);
    }

    private void OpenSettings()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);

        //if (InputManager.Instance != null) InputManager.Instance.LoadAllKeybinds();
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
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
