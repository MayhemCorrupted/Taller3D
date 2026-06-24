using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioLibrary : MonoBehaviour
{
    public enum AudioType { Music, Environment, SFX, Voice }
    [System.Serializable]
    public struct AudioTrack
    {
        public string trackName;
        public AudioClip clip;
        public AudioType category;
        [Range(0, 1)] public float localVolume;
    }

    [Header("Audio Library")]
    [SerializeField] List<AudioTrack> tracks = new();
    readonly private Dictionary<string, AudioTrack> trackDictionary = new();
    private AudioSource audioSource;

    private AudioTrack currentLoopingTrack;
    private bool hasLoopingTrack = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        foreach (AudioTrack track in tracks)
        {
            if (!string.IsNullOrEmpty(track.trackName) && !trackDictionary.ContainsKey(track.trackName)) trackDictionary.Add(track.trackName, track);
            else Debug.LogWarning($"[AudioRegister] Nombre duplicado o vacío: '{track.trackName}' en {gameObject.name}");
        }
    }
    void Start()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SubscribeAudio(this);
    }
    public void PlayOneShotSound(string name)
    {
        if (trackDictionary.TryGetValue(name, out AudioTrack track))
        {
            if (track.clip == null || audioSource == null) return;

            float globalVolume = AudioManager.Instance != null ? AudioManager.Instance.GetCategoryVolume(track.category) : 1f;

            audioSource.volume = globalVolume * track.localVolume;
            audioSource.PlayOneShot(track.clip);
        }
        else Debug.LogWarning($"[AudioRegister] Pista no encontrada: '{name}'");
    }
    public void PlayLoopSound(string name)
    {
        if (trackDictionary.TryGetValue(name, out AudioTrack track))
        {
            if (track.clip == null || audioSource == null) return;

            currentLoopingTrack = track;
            hasLoopingTrack = true;

            float globalVolume = AudioManager.Instance != null ? AudioManager.Instance.GetCategoryVolume(track.category) : 1f;

            audioSource.clip = track.clip;
            audioSource.loop = true;
            audioSource.volume = globalVolume * track.localVolume;
            audioSource.Play();
        }
        else Debug.LogWarning($"[AudioRegister] Pista no encontrada: '{name}'");
    }
    public void StopSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            hasLoopingTrack = false;
        }
    }
    public void RecalculateLoopVolume()
    {
        if (hasLoopingTrack && audioSource != null && audioSource.isPlaying)
        {
            float globalVolume = AudioManager.Instance != null ? AudioManager.Instance.GetCategoryVolume(currentLoopingTrack.category) : 1f;
            audioSource.volume = globalVolume * currentLoopingTrack.localVolume;
        }
    }
    void OnDestroy()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.UnsubscribeAudio(this);
    }
}
