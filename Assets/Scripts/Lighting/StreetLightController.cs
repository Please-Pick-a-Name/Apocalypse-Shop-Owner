using UnityEngine;

[RequireComponent(typeof(Light))]
public class StreetLightController : MonoBehaviour
{
    [Header("Light Settings")]
    public Color lightColor = new Color(1.0f, 0.95f, 0.7f);
    [Range(0f, 100f)]
    public float intensity = 8.0f;
    [Range(1f, 50f)]
    public float range = 15.0f;
 
    [Header("Flicker Effect")]
    public bool enableFlicker = false;
    public float minIntensity = 7.0f;
    public float maxIntensity = 9.0f;
    public float flickerSpeed = 0.2f;

    private Light streetLight;
    private float baseIntensity;

    void Awake()
    {
        streetLight = GetComponent<Light>();
        streetLight.type = LightType.Point;
    }

    void Start()
    {
        ApplyLightSettings();
        baseIntensity = intensity;
    }

    void Update()
    {
        if (enableFlicker)
        {
            ApplyFlicker();
        }
        else
        {
            // Base intensity if disabled
            streetLight.intensity = baseIntensity;
        }
    }

    private void ApplyLightSettings()
    {
        streetLight.color = lightColor;
        streetLight.intensity = intensity;
        streetLight.range = range;
    }

    private void ApplyFlicker()
    {
        float flickerValue = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        streetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, flickerValue);
    }

    void OnValidate()
    {
        if (streetLight == null)
        {
            streetLight = GetComponent<Light>();
        }
        if (streetLight != null)
        {
            ApplyLightSettings();
        }
    }
}