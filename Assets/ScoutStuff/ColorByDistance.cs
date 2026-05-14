using UnityEngine;

[ExecuteAlways]                      // <-- run in edit mode too
[RequireComponent(typeof(ParticleSystem))]
public class ColorByDistance : MonoBehaviour
{
    public Transform center;
    public float maxDistance = 20f;
    public Gradient gradient;
    public bool previewInEditMode = true;

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private int cachedMax;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        cachedMax = ps.main.maxParticles;
        particles = new ParticleSystem.Particle[cachedMax];
        if (!center) center = transform;
    }

    void LateUpdate() { ApplyColors(); }           // play mode path
    void Update() { if (!Application.isPlaying)    // edit mode path
        { if (previewInEditMode) ApplyColors(); } }

    void ApplyColors()
    {
        // keep buffer size in sync
        if (ps.main.maxParticles != cachedMax)
        {
            cachedMax = ps.main.maxParticles;
            particles = new ParticleSystem.Particle[cachedMax];
        }

        int alive = ps.GetParticles(particles);
        if (alive == 0) return;

        Vector3 c = center ? center.position : transform.position;
        float inv = maxDistance > 0f ? 1f / maxDistance : 0f;

        for (int i = 0; i < alive; i++)
        {
            var current = particles[i].GetCurrentColor(ps);   // keeps alpha fade
            float d = Vector3.Distance(particles[i].position, c);
            Color col = gradient.Evaluate(Mathf.Clamp01(d * inv));
            col.a = current.a;
            particles[i].startColor = col;
        }
        ps.SetParticles(particles, alive);
    }
}