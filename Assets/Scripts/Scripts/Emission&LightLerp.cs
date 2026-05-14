using UnityEngine;
using System.Collections.Generic;

public class EmissionAndLightLerp : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private List<Light> targetLights = new List<Light>();
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 3f;

    [Header("Emissive Materials (Shader Graphs/Basic_ORM)")]
    [SerializeField] public List<Material> emissiveMaterials = new List<Material>();
    [SerializeField] private string emissivePropertyName = "_Emissive_Strength";
    [SerializeField] private float minEmission = 0f;
    [SerializeField] private float maxEmission = 300f;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] public bool looping = true;
    [SerializeField] private bool useExponentialFunc = false;

    [Header("Exponential function values (y = (a^x - 1) / b)")]
    [SerializeField] private float a = 100f;
    [SerializeField] private float b = 100f;

    private bool isPlaying;
    private bool fadingUp = false;
    private float t = 0f;

    private Dictionary<Material, float> originalEmission = new Dictionary<Material, float>();

    void Awake()
    {
        CacheOriginalEmission();
    }

    void OnDisable()
    {
        RestoreEmission();
    }

    void CacheOriginalEmission()
    {
        originalEmission.Clear();

        foreach (var mat in emissiveMaterials)
        {
            if (mat == null) continue;
            if (!mat.HasProperty(emissivePropertyName)) continue;
            if (originalEmission.ContainsKey(mat)) continue;

            originalEmission[mat] = mat.GetFloat(emissivePropertyName);
        }
    }

    void RestoreEmission()
    {
        foreach (var kvp in originalEmission)
        {
            var mat = kvp.Key;
            if (mat == null) continue;
            if (!mat.HasProperty(emissivePropertyName)) continue;

            mat.SetFloat(emissivePropertyName, kvp.Value);
        }
    }

    private void Start()
    {
        isPlaying = playOnStart;

        foreach (var l in targetLights)
        {
            if (l != null)
                l.intensity = 0;
        }

        foreach (var mat in emissiveMaterials)
        {
            if (mat == null) continue;
            if (mat.HasProperty(emissivePropertyName))
                mat.SetFloat(emissivePropertyName, 0);
        }
    }

    private void Update()
    {
        if (!isPlaying || fadeDuration <= 0f)
            return;

        float dir = 1f;
        if (looping)
            dir = fadingUp ? 1f : -1f;

        t += (Time.deltaTime / fadeDuration) * dir;
        t = Mathf.Clamp01(t);

        float tExp = t;
        if (useExponentialFunc)
            tExp = (Mathf.Pow(a, t) - 1f) / b;

        if (looping)
        {
            if (t >= 1f) fadingUp = false;
            else if (t <= 0f) fadingUp = true;
        }

        float intensityValue = Mathf.Lerp(minIntensity, maxIntensity, tExp);
        float emissionValue  = Mathf.Lerp(minEmission,  maxEmission,  tExp);

        foreach (var l in targetLights)
        {
            if (l != null)
                l.intensity = intensityValue;
        }

        foreach (var mat in emissiveMaterials)
        {
            if (mat == null) continue;
            if (mat.HasProperty(emissivePropertyName))
                mat.SetFloat(emissivePropertyName, emissionValue);
        }
    }

    public void Play()
    {
        t = 0f;
        isPlaying = true;
    }

    public void Stop()
    {
        t = 0f;
        isPlaying = false;
    }

    public void OnReset()
    {
        t = 0f;
        isPlaying = playOnStart;

        foreach (var l in targetLights)
        {
            if (l != null)
                l.intensity = 0;
        }

        foreach (var mat in emissiveMaterials)
        {
            if (mat == null) continue;
            if (mat.HasProperty(emissivePropertyName))
                mat.SetFloat(emissivePropertyName, 0);
        }
    }
}
