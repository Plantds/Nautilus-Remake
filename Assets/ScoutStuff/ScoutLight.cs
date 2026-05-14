using System.Collections;
using UnityEngine;
using FMODUnity;

public class ScoutLight : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;              // Light to control
    public float fadeOutDuration = 4f;     // Time to fade from current intensity -> 0 on impact

    [Header("Flicker Settings")]
    public float flickerAmplitude = 2f;    // How much it wiggles around base intensity
    public float flickerSpeed = 20f;       // Speed of small flickers

    [Header("Fuse-like Glitch Flicks")]
    public bool useBursts = true;
    public float burstChancePerSecond = 0.4f;
    public float burstDuration = 0.08f;
    public float burstIntensityMultiplier = 0.1f; // Multiplier during glitch (e.g. 0.1 = almost off)

    [Header("Impact Audio")]
    [SerializeField] private StudioEventEmitter impactEmitter; // Plays once when the scout lands

    private Rigidbody rb;
    private Collider col;
    private bool hasHit;

    // Flicker state
    private bool inBurst = false;
    private float burstEndTime = 0f;

    // Fade state
    private float fadeFactor = 0f;     // 0 = off, 1 = full
    private float baseIntensity = 0f;  // Captured from current light intensity when fade starts
    private bool isFading = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (targetLight == null)
            targetLight = GetComponentInChildren<Light>();

        // If you want it dark while flying, uncomment:
        // if (targetLight != null)
        //     targetLight.intensity = 0f;
    }

    void Update()
    {
        if (!isFading || targetLight == null)
            return;

        if (fadeFactor <= 0f)
        {
            targetLight.intensity = 0f;
            return;
        }

        float time = Time.time;

        // Perlin-based small flicker
        float noise = Mathf.PerlinNoise(time * flickerSpeed, 0f);
        float centered = (noise - 0.5f) * 2f;
        float smallFlicker = centered * flickerAmplitude;

        float intensity = (baseIntensity + smallFlicker) * fadeFactor;

        // Burst logic
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
                    intensity *= burstIntensityMultiplier;
                }
                else
                {
                    inBurst = false;
                }
            }
        }

        targetLight.intensity = Mathf.Max(0f, intensity);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
            return;

        hasHit = true;

        // Stop movement and physics
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (col != null)
            col.isTrigger = true;

        // Snap to first contact point and stick to the surface
        if (collision.contactCount > 0)
            transform.position = collision.GetContact(0).point;

        transform.SetParent(collision.transform);

        // 🔊 Play impact sound via emitter (if assigned)
        if (impactEmitter != null)
        {
            // ensure it's at the right spot if it's not already a child
            impactEmitter.transform.position = transform.position;
            impactEmitter.Play();
        }

        // Start fade out from whatever intensity it has right now
        if (targetLight != null)
            StartFadeOutAndDie(fadeOutDuration);
        else
            Destroy(gameObject, fadeOutDuration);
    }

    /// <summary>
    /// Called by shooter OR collision: starts a fade from current intensity -> 0, then destroys the projectile.
    /// </summary>
    public void StartFadeOutAndDie(float duration)
    {
        if (targetLight == null)
        {
            Destroy(gameObject);
            return;
        }

        StopAllCoroutines();

        baseIntensity = targetLight.intensity; // Keep current brightness
        fadeFactor = 1f;
        isFading = true;

        if (duration <= 0f)
        {
            fadeFactor = 0f;
            targetLight.intensity = 0f;
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }
    }

    IEnumerator FadeOutCoroutine(float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            fadeFactor = 1f - normalized; // 1 -> 0 over "duration"
            yield return null;
        }

        fadeFactor = 0f;
        if (targetLight != null)
            targetLight.intensity = 0f;

        Destroy(gameObject);
    }
}
