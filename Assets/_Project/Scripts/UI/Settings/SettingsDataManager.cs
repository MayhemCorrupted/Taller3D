using UnityEngine;

public static class SettingsDataManager
{
    public static float MasterVolume
    {
        get => PlayerPrefs.GetFloat("Settings_MasterVol", 1f);
        set { PlayerPrefs.SetFloat("Settings_MasterVol", value); PlayerPrefs.Save(); }
    }

    public static float SFXVolume
    {
        get => PlayerPrefs.GetFloat("Settings_SFXVol", 1f);
        set { PlayerPrefs.SetFloat("Settings_SFXVol", value); PlayerPrefs.Save(); }
    }

    public static float VoiceVolume
    {
        get => PlayerPrefs.GetFloat("Settings_VoiceVol", 1f);
        set { PlayerPrefs.SetFloat("Settings_VoiceVol", value); PlayerPrefs.Save(); }
    }

    public static float EnvironmentVolume
    {
        get => PlayerPrefs.GetFloat("Settings_EnvVol", 1f);
        set { PlayerPrefs.SetFloat("Settings_EnvVol", value); PlayerPrefs.Save(); }
    }

    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat("Settings_MusicVol", 1f);
        set { PlayerPrefs.SetFloat("Settings_MusicVol", value); PlayerPrefs.Save(); }
    }
    public static float MouseSensibility
    {
        get => PlayerPrefs.GetFloat("Settings_MouseSens", 50f);
        set { PlayerPrefs.SetFloat("Settings_MouseSens", value); PlayerPrefs.Save(); }
    }

    public static bool InvertY
    {
        get => PlayerPrefs.GetInt("Settings_InvertY", 0) == 1; 
        set { PlayerPrefs.SetInt("Settings_InvertY", value ? 1 : 0); PlayerPrefs.Save(); }
    }

    public static bool InvertX
    {
        get => PlayerPrefs.GetInt("Settings_InvertX", 0) == 1;
        set { PlayerPrefs.SetInt("Settings_InvertX", value ? 1 : 0); PlayerPrefs.Save(); }
    }

}
