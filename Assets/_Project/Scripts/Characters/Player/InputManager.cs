using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Movement Keys")]
    public KeyCode ForwardKey { get; private set; }
    public KeyCode BackwardKey { get; private set; }
    public KeyCode LeftKey { get; private set; }
    public KeyCode RightKey { get; private set; }
    public KeyCode FlyKey { get; private set; }

    [Header("Action Keys")]
    public KeyCode InteractKey { get; private set; }
    public KeyCode PauseKey { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadAllKeybinds();
        }
        else Destroy(gameObject);
    }
    public void LoadAllKeybinds()
    {
        ForwardKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Forward", "W"));
        BackwardKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Backward", "S"));
        LeftKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Left", "A"));
        RightKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Right", "D"));

        FlyKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Fly", "Space"));
        InteractKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Interact", "E"));
        PauseKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Pause", "Escape"));
    }
}
