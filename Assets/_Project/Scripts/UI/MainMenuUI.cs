using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Settings")]
    [SerializeField] string playSceneName;
    [SerializeField] Button playButton;
    [SerializeField] Button exitButton;
    void Awake()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (playButton != null) playButton.onClick.AddListener(() => ChangePlayScene(playSceneName));
        if (exitButton != null) exitButton.onClick.AddListener(() => ExitGame());
    }
    void ChangePlayScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }
}
