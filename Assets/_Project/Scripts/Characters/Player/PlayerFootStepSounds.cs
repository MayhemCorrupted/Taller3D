using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioLibrary))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(CharacterController))]

public class PlayerFootStepSounds : MonoBehaviour
{
    [System.Serializable] private struct SurfaceSettings
    {
        public string groundTag;
        public string soundName;
    }

    [Header("Configuration")]
    [SerializeField] private Transform feetPoint;
    [SerializeField] private float rayDistance = 1.2f;
    [SerializeField] private float stepCooldown = 0.35f;

    [Header("Sounds")]
    [SerializeField] private List<SurfaceSettings> surfaces = new();

    private CharacterController controller;
    private AudioLibrary audioLib;
    private PlayerMovement movement;
    private float stepTimer = 0f;

    void Awake()
    {
        audioLib = GetComponent<AudioLibrary>();
        movement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 horizontalVelocity = new (controller.velocity.x, 0f, controller.velocity.z);

        if (movement.Flying || horizontalVelocity.sqrMagnitude < 0.1f)
        {
            stepTimer = stepCooldown;
            return;
        }
        if (!movement.IsGrounded)
        {
            return;
        }

        UpdateFootsteps();
    }

    private void UpdateFootsteps()
    {
        stepTimer += Time.deltaTime;

        if (stepTimer >= stepCooldown)
        {
            string detectedTag = "Untagged";
            Vector3 rayOrigin = feetPoint != null ? feetPoint.position : transform.position;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance))
            {
                if (hit.collider.gameObject != this.gameObject)
                {
                    detectedTag = hit.collider.tag;
                }
            }

            string soundToPlay = "";
            bool found = false;

            foreach (var surface in surfaces)
            {
                if (surface.groundTag == detectedTag)
                {
                    soundToPlay = surface.soundName;
                    found = true;
                    break;
                }
            }

            if (found && !string.IsNullOrEmpty(soundToPlay))
            {
                audioLib.PlayOneShotSound(soundToPlay);
            }
            else
            {
                Debug.LogWarning($"El suelo tiene el tag {detectedTag} y ese no esta configurado en la lista.");
            }

            stepTimer = 0f;
        }
    }
}