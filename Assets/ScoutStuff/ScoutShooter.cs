using UnityEngine;
using FMODUnity;

public class ScoutShooter : MonoBehaviour
{
    [Header("Projectile Setup")]
    public GameObject projectilePrefab;    // Light prefab
    public Transform firePoint;            // Spawn position and forward direction

    [Header("Launch Settings")]
    public float launchForce = 20f;
    public float launchAngle = 0f;
    public Vector3 rotationOffset;

    [Header("Cooldown")]
    public float relaunchCooldown = 2f;    // Time in seconds before you can fire again

    [Header("Relaunch Fade Out")]
    public float relaunchFadeOutDuration = 0.5f; // Fade duration for OLD projectile when relaunching

    [Header("Cooldown Audio Timing")]
    public float cooldownStartDelay = 0.1f; // Delay before cooldown start sound/loop plays

    [Header("Audio Emitters")]
    [SerializeField] private StudioEventEmitter fireEmitter;             // Plays when firing
    [SerializeField] private StudioEventEmitter cooldownPressEmitter;    // Plays when pressing during cooldown
    [SerializeField] private StudioEventEmitter readyEmitter;            // Plays when cooldown finishes
    [SerializeField] private StudioEventEmitter cooldownStartEmitter;    // Looping sound while on cooldown

    [Header("Cooldown Needle")]
    [SerializeField] private Transform needle;          // Needle object to rotate
    [SerializeField] private Vector3 needleFromEuler;   // Start rotation (deg) when cooldown begins
    [SerializeField] private Vector3 needleToEuler;     // End rotation (deg) when cooldown ends
    [SerializeField] private bool needleRotateX;        // Rotate around X?
    [SerializeField] private bool needleRotateY;        // Rotate around Y?
    [SerializeField] private bool needleRotateZ = true; // Rotate around Z?
    [SerializeField] private float needleShootLerpDuration = 0.1f; // Time to lerp to start angle on shoot

    private GameObject activeProjectile;   // Last spawned projectile
    private float nextFireTime = 0f;
    public bool isOnCooldown = false;
    private bool readySoundPlayed = false;

    // Cooldown start loop timing
    private bool cooldownStartPlayed = false;
    private float cooldownStartPlayTime = 0f;
    private bool cooldownLoopActive = false;

    // Needle timing
    private bool needleActive = false;         // true when doing the cooldown sweep
    private float cooldownStartTime = 0f;      // time when we fired (cooldown began)
    private Coroutine needleKickCoroutine;     // for the shoot → start lerp

    void Update()
    {
        if (isOnCooldown)
        {
            // --- Needle rotation based on cooldown progress (after the initial kick) ---
            if (needleActive && needle != null && relaunchCooldown > 0f)
            {
                float elapsed = Time.time - cooldownStartTime;
                float denom = Mathf.Max(0.0001f, relaunchCooldown - needleShootLerpDuration);
                float t = Mathf.Clamp01((elapsed - needleShootLerpDuration) / denom);

                Vector3 current = needle.localEulerAngles;
                Vector3 newEuler = current;

                if (needleRotateX)
                    newEuler.x = Mathf.Lerp(needleFromEuler.x, needleToEuler.x, t);
                if (needleRotateY)
                    newEuler.y = Mathf.Lerp(needleFromEuler.y, needleToEuler.y, t);
                if (needleRotateZ)
                    newEuler.z = Mathf.Lerp(needleFromEuler.z, needleToEuler.z, t);

                needle.localEulerAngles = newEuler;
            }

            // --- Start the cooldown loop after a short delay ---
            if (!cooldownStartPlayed &&
                Time.time >= cooldownStartPlayTime &&
                Time.time < nextFireTime)
            {
                if (cooldownStartEmitter != null)
                {
                    cooldownStartEmitter.Play(); // FMOD event should be set to loop
                    cooldownLoopActive = true;
                }

                cooldownStartPlayed = true;
            }

            // --- Check if cooldown finished ---
            if (Time.time >= nextFireTime && !readySoundPlayed)
            {
                // Stop cooldown loop
                if (cooldownStartEmitter != null && cooldownLoopActive)
                {
                    cooldownStartEmitter.Stop(); // AllowFadeout recommended
                    cooldownLoopActive = false;
                }

                // Snap needle to final rotation
                if (needle != null)
                {
                    Vector3 newEuler = needle.localEulerAngles;

                    if (needleRotateX)
                        newEuler.x = needleToEuler.x;
                    if (needleRotateY)
                        newEuler.y = needleToEuler.y;
                    if (needleRotateZ)
                        newEuler.z = needleToEuler.z;

                    needle.localEulerAngles = newEuler;
                    needleActive = false;
                }

                // Play "ready" sound
                if (readyEmitter != null)
                {
                    readyEmitter.Play();
                }

                readySoundPlayed = true;
                isOnCooldown = false;
            }
        }
    }

    public void TryFire()
    {
        if (projectilePrefab == null)
            return;

        // If still on cooldown, play cooldown-press sound
        if (Time.time < nextFireTime)
        {
            if (cooldownPressEmitter != null)
            {
                cooldownPressEmitter.Play();
            }
            return;
        }

        // Fire is allowed here
        FireProjectile();

        // Start cooldown
        nextFireTime = Time.time + relaunchCooldown;
        isOnCooldown = true;
        readySoundPlayed = false;

        // Setup delayed cooldown loop
        cooldownStartPlayed = false;
        cooldownStartPlayTime = Time.time + cooldownStartDelay;

        // Remember when cooldown began
        cooldownStartTime = Time.time;

        // Setup needle movement: first a kick to the start angle, then the sweep
        if (needle != null)
        {
            needleActive = false; // will be enabled after kick finishes

            if (needleKickCoroutine != null)
                StopCoroutine(needleKickCoroutine);

            needleKickCoroutine = StartCoroutine(NeedleShootKick());
        }
        else
        {
            needleActive = false;
        }
    }

    void FireProjectile()
    {
        Transform spawnPoint = firePoint != null ? firePoint : transform;

        // If a previous projectile exists, fade it out instead of hard-destroy
        if (activeProjectile != null)
        {
            ScoutLight light = activeProjectile.GetComponent<ScoutLight>();
            if (light != null)
            {
                light.StartFadeOutAndDie(relaunchFadeOutDuration);
            }
            else
            {
                Destroy(activeProjectile);
            }

            activeProjectile = null;
        }

        Quaternion angleRotation = Quaternion.AngleAxis(launchAngle, spawnPoint.right);
        Vector3 direction = angleRotation * spawnPoint.forward;

        GameObject instance = Instantiate(
            projectilePrefab,
            spawnPoint.position,
            Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset)
        );

        activeProjectile = instance;

        Rigidbody rb = instance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearVelocity = direction * launchForce;
        }

        // Play fire sound via emitter
        if (fireEmitter != null)
        {
            fireEmitter.Play();
        }
    }

    private System.Collections.IEnumerator NeedleShootKick()
    {
        Vector3 initialEuler = needle.localEulerAngles;
        Vector3 targetEuler = initialEuler;

        if (needleRotateX)
            targetEuler.x = needleFromEuler.x;
        if (needleRotateY)
            targetEuler.y = needleFromEuler.y;
        if (needleRotateZ)
            targetEuler.z = needleFromEuler.z;

        float duration = Mathf.Max(0.0001f, needleShootLerpDuration);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float f = Mathf.Clamp01(t / duration);

            Vector3 euler = initialEuler;

            if (needleRotateX)
                euler.x = Mathf.Lerp(initialEuler.x, targetEuler.x, f);
            if (needleRotateY)
                euler.y = Mathf.Lerp(initialEuler.y, targetEuler.y, f);
            if (needleRotateZ)
                euler.z = Mathf.Lerp(initialEuler.z, targetEuler.z, f);

            needle.localEulerAngles = euler;

            yield return null;
        }

        // Ensure exact target at the end of the kick
        needle.localEulerAngles = targetEuler;

        // Now start the normal cooldown sweep
        needleActive = true;
    }
}
