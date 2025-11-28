using UnityEngine;

public class EmissiveMaterialController : MonoBehaviour
{
    // Reference to the actual Light component
    [Tooltip("Point Light")]
    public Light targetLight;

    // Multiplier to make the glow visually brighter than the actual light intensity
    [Tooltip("How much brighter the material's glow should be.")]
    public float emissionMultiplier = 0.5f;

    private Renderer lightRenderer;
    private Material lightMaterial;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissiveColor");

    void Start()
    {
        lightRenderer = GetComponent<Renderer>();
        if (lightRenderer == null)
        {
            Debug.LogError("EmissiveMaterialController requires a Renderer component.");
            enabled = false;
            return;
        }

        if (targetLight == null)
        {
            Debug.LogError("Target Light is not assigned.");
            enabled = false;
            return;
        }

        lightMaterial = lightRenderer.material;
        lightMaterial.EnableKeyword("_EMISSION"); 
    }

    void Update()
    {
        if (targetLight == null || lightMaterial == null) return;

        Color baseColor = targetLight.color;
        float finalIntensity = targetLight.intensity * emissionMultiplier;

        Color finalEmissionColor = baseColor * finalIntensity;
        lightMaterial.SetColor(EmissionColor, finalEmissionColor);
    }
}