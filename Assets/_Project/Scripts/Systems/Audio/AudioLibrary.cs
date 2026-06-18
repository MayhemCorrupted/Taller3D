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

    private AudioSource loopSource;
    private AudioSource sfxSource;

    private AudioTrack currentLoopingTrack;
    private bool hasLoopingTrack = false;
    private bool wasLoopingBeforePause = false;

    private bool sfxWasPlaying = false;

    void Awake()
    {
        loopSource = GetComponent<AudioSource>();
        loopSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        foreach (AudioTrack track in tracks)
        {
            if (!string.IsNullOrEmpty(track.trackName) && !trackDictionary.ContainsKey(track.trackName))
                trackDictionary.Add(track.trackName, track);
            else
                Debug.LogWarning($"[AudioRegister] Nombre duplicado o vacío: '{track.trackName}' en {gameObject.name}");
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
            if (track.clip == null) return;

            float globalVolume = AudioManager.Instance != null ? AudioManager.Instance.GetCategoryVolume(track.category) : 1f;

            sfxSource.clip = track.clip;
            sfxSource.loop = false;
            sfxSource.volume = globalVolume * track.localVolume;
            sfxSource.Play();
        }
        else Debug.LogWarning($"[AudioRegister] Pista no encontrada: '{name}'");
    }

    public void PlayLoopSound(string name)
    {
        if (trackDictionary.TryGetValue(name, out AudioTrack track))
        {
            if (track.clip == null) return;

            currentLoopingTrack = track;
            hasLoopingTrack = true;

            float globalVolume = AudioManager.Instance != null ? AudioManager.Instance.GetCategoryVolume(track.category) : 1f;

            loopSource.clip = track.clip;
            loopSource.loop = true;
            loopSource.volume = globalVolume * track.localVolume;
            loopSource.Play();
        }
        else Debug.LogWarning($"[AudioRegister] Pista no encontrada: '{name}'");
    }

    public void StopSound()
    {
        if (loopSource != null)
        {
            loopSource.Stop();
            hasLoopingTrack = false;
        }
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }

    public void RecalculateLoopVolume()
    {
        if (hasLoopingTrack && loopSource != null && loopSource.isPlaying)
        {
            float globalVolume = AudioManager.Instance != null ? AudioManager.Instance.GetCategoryVolume(currentLoopingTrack.category) : 1f;
            loopSource.volume = globalVolume * currentLoopingTrack.localVolume;
        }
    }

    void OnDestroy()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.UnsubscribeAudio(this);
    }

    public bool IsPlaying()
    {
        return (loopSource != null && loopSource.isPlaying) ||
               (sfxSource != null && sfxSource.isPlaying);
    }

    public bool IsLooping()
    {
        return hasLoopingTrack;
    }

    public void Pause()
    {
   
        if (hasLoopingTrack && loopSource != null && loopSource.isPlaying)
        {
            wasLoopingBeforePause = true;
            loopSource.Pause();
        }

  
        if (sfxSource != null && sfxSource.isPlaying)
        {
            sfxWasPlaying = true;
            sfxSource.Pause();
        }
    }

    public void Resume()
    {
     
        if (wasLoopingBeforePause && loopSource != null)
        {
            loopSource.UnPause();
            wasLoopingBeforePause = false;
        }

        if (sfxWasPlaying && sfxSource != null)
        {
            sfxSource.UnPause();
            sfxWasPlaying = false;
        }
    }
}
