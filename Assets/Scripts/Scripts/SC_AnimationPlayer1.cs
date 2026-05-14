using UnityEngine;
using FMODUnity;

public class SC_AnimationPlayer1 : MonoBehaviour
{
    [Header("Animation")]
    public string animationClipName = "";
    public bool play = false;
    public bool reverse = false;

    public Animator animator;

    public float normalizedTime = 0.0f;
    private float time = 0.0f;

    [Header("Audio (FMOD Emitters)")]
    [Tooltip("Emitter used when playing the animation forward (e.g. DOOR OPEN).")]
    [SerializeField] private StudioEventEmitter forwardEmitter;

    [Tooltip("Emitter used when playing the animation in reverse (e.g. DOOR CLOSE).")]
    [SerializeField] private StudioEventEmitter reverseEmitter;

    // Internal state so we only trigger emitter when starting / changing direction
    private bool wasPlayingLastFrame = false;
    private bool lastDirectionReverse = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        time = 0.0f;
        normalizedTime = 0.0f;
    }

    void Update()
    {
        // If we're not playing, reset state and bail
        if (!play || animator == null)
        {
            wasPlayingLastFrame = false;
            return;
        }

        // Just started playing OR direction changed (open <-> close)
        if (!wasPlayingLastFrame || lastDirectionReverse != reverse)
        {
            PlayDirectionEmitter();
        }

        wasPlayingLastFrame = true;
        lastDirectionReverse = reverse;

        // Advance time based on direction
        if (reverse)
        {
            time -= Time.deltaTime;
        }
        else
        {
            time += Time.deltaTime;
        }

        float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
        if (clipLength <= 0.0f)
            return;

        time = Mathf.Clamp(time, 0.0f, clipLength);

        normalizedTime = time / clipLength;
        normalizedTime = Mathf.Clamp01(normalizedTime);

        animator.SetFloat("Time", normalizedTime);

        // Optional: auto-stop when reaching the end
        // if (normalizedTime <= 0.0f || normalizedTime >= 1.0f)
        //     play = false;
    }

    private void PlayDirectionEmitter()
    {
        // reverse == false → forward (OPEN)
        // reverse == true  → backward (CLOSE)
        if (!reverse)
        {
            if (forwardEmitter != null)
            {
                forwardEmitter.Play();
            }
        }
        else
        {
            if (reverseEmitter != null)
            {
                reverseEmitter.Play();
            }
        }
    }
}
