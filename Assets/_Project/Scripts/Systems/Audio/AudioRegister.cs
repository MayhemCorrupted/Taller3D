using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioRegister : MonoBehaviour
{
    public enum AudioType { Music, Enviroment, VFX, Voices }
    [Header("Name")]
    [SerializeField] string clipName;
    [Header("Audio Settings")]
    [SerializeField] AudioClip clip;
    [SerializeField] AudioType category;
    [Header("Local Setting")]
    [SerializeField] [Range(0,1)] float localVolumeMultiplier = 1;
    AudioSource audioSource;
    public AudioType Category => category;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SubscribeAudio(this);
    }
    public void PlayOneShotSound()
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }
    public void PlayLoopSound()
    {
        if (clip == null || audioSource == null) return;
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
    public void StopSound()
    {
        if (audioSource != null) audioSource.Stop();
    }
    public void UpdateVolume(float globalVolume)
    {
        if (audioSource != null) audioSource.volume = globalVolume * localVolumeMultiplier;
    }
    void OnDestroy()
    {
        if(AudioManager.Instance != null)
           AudioManager.Instance.UnsubscribeAudio(this);
    }
}
