using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    readonly Dictionary<AudioRegister.AudioType, float> categoryVolumes = new();
    readonly List<AudioRegister> activeAudios = new();
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (AudioRegister.AudioType type in System.Enum.GetValues(typeof(AudioRegister.AudioType)))
        {
            categoryVolumes[type] = 1;
        }
    }
    public void SubscribeAudio(AudioRegister audio)
    {
        if (!activeAudios.Contains(audio))
        {
            activeAudios.Add(audio);
            audio.UpdateVolume(categoryVolumes[audio.Category]);
        }
    }
    public void UnsubscribeAudio(AudioRegister audio)
    {
        if (activeAudios.Contains(audio)) activeAudios.Remove(audio);
    }
    public void SetCategoryVolume(AudioRegister.AudioType category, float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        categoryVolumes[category] = clampedVolume;

        foreach (var audio in activeAudios) audio.UpdateVolume(clampedVolume);
    }
    public float GetCateVolume(AudioRegister.AudioType category)
    {
        return categoryVolumes.GetValueOrDefault(category, 1);
    }
}
