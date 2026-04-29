using UnityEngine;
using System;
using System.Collections;
using Unity.Cinemachine;

public class CinematicController : MonoBehaviour
{
    public static CinematicController Instance { get; private set; }

    [Header("References")]
    [SerializeField] CinemachineCamera playerCamera;

    [Header("Cinematic Cameras")]
    [SerializeField] CinematicEntry[] cinematics;

    [Header("Settings")]
    [SerializeField] float defaultBlendDuration = 0.8f;

    public event Action<string> OnCinematicEnd;

    public event Action<string> OnCinematicStart;

    bool isPlaying;
    CinemachineCamera activeCinematicCam;
    Coroutine currentCinematic;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        foreach (var entry in cinematics)
            if (entry.camera != null)
                entry.camera.Priority = 0;
    }

    public void PlayCinematic(string id)
    {
        if (isPlaying)
        {
            Debug.LogWarning($"[CinematicController] Already playing a cinematic. Skipping '{id}'.");
            return;
        }

        CinematicEntry entry = GetEntry(id);
        if (entry == null)
        {
            Debug.LogError($"[CinematicController] No cinematic found with ID '{id}'.");
            return;
        }

        currentCinematic = StartCoroutine(RunCinematic(entry));
    }

    public void StopCinematic()
    {
        if (currentCinematic != null)
            StopCoroutine(currentCinematic);

        EndCinematic(activeCinematicCam, string.Empty);
    }

    public bool IsPlaying => isPlaying;

    IEnumerator RunCinematic(CinematicEntry entry)
    {
        isPlaying = true;
        activeCinematicCam = entry.camera;

        OnCinematicStart?.Invoke(entry.id);
        entry.onStart?.Invoke();

        SetPlayerInputEnabled(false);

        entry.camera.Priority = 20;

        yield return new WaitForSeconds(defaultBlendDuration);

        yield return new WaitForSeconds(entry.duration);

        entry.onEnd?.Invoke();

        EndCinematic(entry.camera, entry.id);
    }

    void EndCinematic(CinemachineCamera cam, string id)
    {
        if (cam != null) cam.Priority = 0;

        isPlaying = false;
        activeCinematicCam = null;

        SetPlayerInputEnabled(true);
        OnCinematicEnd?.Invoke(id);
    }

    void SetPlayerInputEnabled(bool enabled)
    {
        var movement = FindFirstObjectByType<Player_Movement>();
        if (movement != null) movement.enabled = enabled;

        var cam = FindFirstObjectByType<Player_Camera>();
        if (cam != null) cam.enabled = enabled;
    }

    CinematicEntry GetEntry(string id)
    {
        foreach (var e in cinematics)
            if (e.id == id) return e;
        return null;
    }

    [Serializable]
    public class CinematicEntry
    {
        [Tooltip("Unique identifier used to call this cinematic from code.")]
        public string id;

        [Tooltip("The Cinemachine camera for this fixed shot.")]
        public CinemachineCamera camera;

        [Tooltip("How long the shot holds before returning to the player camera.")]
        public float duration = 3f;

        [Tooltip("Called when this cinematic begins.")]
        public UnityEngine.Events.UnityEvent onStart;

        [Tooltip("Called when this cinematic ends (before returning to player camera).")]
        public UnityEngine.Events.UnityEvent onEnd;
    }
}
