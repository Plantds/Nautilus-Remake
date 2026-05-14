using UnityEngine;
using System.Collections.Generic;

public class EmissiveBulbFlicker : MonoBehaviour
{
    [Header("Emissive Materials (Shader Graphs/Basic_ORM)")]
    [SerializeField] private List<Material> emissiveMaterials = new List<Material>();
    [SerializeField] private string emissivePropertyName = "_Emissive_Strength";

    [Header("Base Glow")]
    [SerializeField] private float baseEmission = 150f;
    [SerializeField] private float flickerAmplitude = 60f;
    [SerializeField] private float flickerSpeed = 20f;

    [Header("Fuse-like Glitch Flicks")]
    [SerializeField] private bool useBursts = true;
    [SerializeField] private float burstChancePerSecond = 0.4f;
    [SerializeField] private float burstDuration = 0.08f;
    [SerializeField] private float burstEmissionMultiplier = 0.1f;

    [Header("Control")]
    [SerializeField] private bool playOnStart = true;

    bool isPlaying;
    bool inBurst = false;
    float burstEndTime = 0f;

    Dictionary<Material, float> originalEmission = new Dictionary<Material, float>();

    void Awake()
    {
        CacheOriginalEmission();
    }

    void OnEnable()
    {
        isPlaying = playOnStart;
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

    void Update()
    {
        if (!isPlaying)
            return;

        float emissionValue = CalculateEmission();
        ApplyEmission(emissionValue);
    }

    float CalculateEmission()
    {
        float time = Time.time;

        float noise = Mathf.PerlinNoise(time * flickerSpeed, 0f);
        float centered = (noise - 0.5f) * 2f;
        float smallFlicker = centered * flickerAmplitude;

        float emission = baseEmission + smallFlicker;

        if (useBursts)
        {
            if (!inBurst)
            {
                if (Random.value < burstChancePerSecond * Time.deltaTime)
                {
                    inBurst = true;
                    burstEndTime = time + burstDuration;
                }
            }

            if (inBurst)
            {
                if (time < burstEndTime)
                {
                    emission *= burstEmissionMultiplier;
                }
                else
                {
                    inBurst = false;
                }
            }
        }

        return Mathf.Max(0f, emission);
    }

    void ApplyEmission(float emissionValue)
    {
        foreach (var mat in emissiveMaterials)
        {
            if (mat == null) continue;
            if (!mat.HasProperty(emissivePropertyName)) continue;

            mat.SetFloat(emissivePropertyName, emissionValue);
        }
    }

    public void Play()  => isPlaying = true;
    public void Stop()  => isPlaying = false;
}
