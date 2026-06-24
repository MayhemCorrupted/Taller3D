using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    readonly Dictionary<AudioLibrary.AudioType, float> categoryVolumes = new();
    readonly List<AudioLibrary> activeAudios = new();
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (AudioLibrary.AudioType type in System.Enum.GetValues(typeof(AudioLibrary.AudioType)))
        {
            categoryVolumes[type] = 1;
        }
    }
    public void SubscribeAudio(AudioLibrary audio)
    {
        if (!activeAudios.Contains(audio)) activeAudios.Add(audio);
    }
    public void UnsubscribeAudio(AudioLibrary audio)
    {
        if (activeAudios.Contains(audio)) activeAudios.Remove(audio);
    }
    public void SetCategoryVolume(AudioLibrary.AudioType category, float volume)
    {
        categoryVolumes[category] = Mathf.Clamp01(volume);
        NotifyAudiosToRecalculate();
    }
    public void NotifyMasterVolumeChanged()
    {
        NotifyAudiosToRecalculate();
    }
    private void NotifyAudiosToRecalculate()
    {
        foreach (var audio in activeAudios)
        {
            audio.RecalculateLoopVolume();
        }
    }
    public float GetCategoryVolume(AudioLibrary.AudioType category)
    {
        float catVol = categoryVolumes.GetValueOrDefault(category, 1f);
        return catVol * SettingsDataManager.MasterVolume;
    }
    public void PauseGlobalAudio()
    {
        AudioListener.pause = true;
    }
    public void ResumeGlobalAudio()
    {
        AudioListener.pause = false;
    }
}
