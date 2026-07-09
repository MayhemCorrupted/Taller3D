using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
public class InteractiveObjectLight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer targetRenderer;

    [Header("Light Settings")]
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float blinkSpeed = 3f;
    [SerializeField] private float glowStrength = 1.5f;

    private Material[] materials;

    void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<MeshRenderer>();
        }

        materials = targetRenderer.materials;

        foreach (Material material in materials)
        {
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
            }
        }
    }

    void Update()
    {
        float blinkValue = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f;

        Color finalGlow = glowColor * Mathf.Lerp(0f, glowStrength, blinkValue);

        foreach (Material material in materials)
        {
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", finalGlow);
            }
        }
    }
    void OnDisable()
    {
        foreach (Material material in materials)
        {
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}