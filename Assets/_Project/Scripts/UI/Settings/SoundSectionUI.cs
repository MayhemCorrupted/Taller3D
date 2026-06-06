using UnityEngine;
using UnityEngine.UI;

public class SoundSectionUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider voiceSlider;
    [SerializeField] Slider environmentSlider;
    [SerializeField] Slider musicSlider;

    void Start()
    {
        AssignListeners();
        LoadVisuals();
    }

    private void AssignListeners()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(v => {
                SettingsDataManager.MasterVolume = v;
                AudioManager.Instance.NotifyMasterVolumeChanged();
            });

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(v => {
                SettingsDataManager.SFXVolume = v;
                AudioManager.Instance.SetCategoryVolume(AudioLibrary.AudioType.SFX, v);
            });

        if (voiceSlider != null)
            voiceSlider.onValueChanged.AddListener(v => {
                SettingsDataManager.VoiceVolume = v;
                AudioManager.Instance.SetCategoryVolume(AudioLibrary.AudioType.Voice, v);
            });

        if (environmentSlider != null)
            environmentSlider.onValueChanged.AddListener(v => {
                SettingsDataManager.EnvironmentVolume = v;
                AudioManager.Instance.SetCategoryVolume(AudioLibrary.AudioType.Environment, v);
            });

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(v => {
                SettingsDataManager.MusicVolume = v;
                AudioManager.Instance.SetCategoryVolume(AudioLibrary.AudioType.Music, v);
            });
    }

    private void LoadVisuals()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.value = SettingsDataManager.MasterVolume;
        if (sfxSlider != null) sfxSlider.value = SettingsDataManager.SFXVolume;
        if (voiceSlider != null) voiceSlider.value = SettingsDataManager.VoiceVolume;
        if (environmentSlider != null) environmentSlider.value = SettingsDataManager.EnvironmentVolume;
        if (musicSlider != null) musicSlider.value = SettingsDataManager.MusicVolume;
    }
}
