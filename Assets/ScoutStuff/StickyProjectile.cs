using UnityEngine;
using FMODUnity;

public class StickyProjectile : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifeTime = 5f;              // How long the projectile exists

    [Header("Stick To")]
    public LayerMask stickLayers;            // Set to only "Outside" in Inspector

    [Header("Effects")]
    public ParticleSystem stickParticles;    // stickyBurst
    public EventReference impactEvent;       // FMOD impact / explosion event

    private Rigidbody rb;
    private Collider col;
    private bool hasStuck;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (stickParticles == null)
            stickParticles = GetComponentInChildren<ParticleSystem>();

        if (stickParticles != null && Application.isPlaying)
            stickParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Start()
    {
        if (lifeTime > 0f)
            Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasStuck)
            return;

        int hitLayer = collision.gameObject.layer;

        // Only stick to layers selected in stickLayers (e.g. Outside)
        if ((stickLayers.value & (1 << hitLayer)) == 0)
            return;

        StickToSurface(collision.transform);
    }

    void StickToSurface(Transform hitTransform)
    {
        hasStuck = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (col != null)
            col.isTrigger = true;

        transform.SetParent(hitTransform);

        if (stickParticles != null)
            stickParticles.Play();

        // Play FMOD impact / explosion sound
        if (!impactEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(impactEvent, transform.position);
        }
    }
}