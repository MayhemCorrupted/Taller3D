using UnityEngine;

public class InteractiveObjectLight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Renderer targetRenderer;

    [Header("Glow Settings")]
    [SerializeField] Color glowColor = Color.yellow;
    [SerializeField] float blinkSpeed = 3f;
    [SerializeField] float glowStrength = 1.5f;

    Material[] materials;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        materials = targetRenderer.materials;

        foreach (Material mat in materials)
        {
            if (mat.HasProperty("_EmissionColor"))
                mat.EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        float blink = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f;
        Color finalGlow = glowColor * Mathf.Lerp(0f, glowStrength, blink);

        foreach (Material mat in materials)
        {
            if (mat.HasProperty("_EmissionColor"))
                mat.SetColor("_EmissionColor", finalGlow);
        }
    }

    void OnDisable()
    {
        if (materials == null) return;
        foreach (Material mat in materials)
        {
            if (mat.HasProperty("_EmissionColor"))
                mat.SetColor("_EmissionColor", Color.black);
        }
    }
}