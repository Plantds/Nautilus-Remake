using UnityEngine;

//PlaySound
public class SC_CloseDoor : MonoBehaviour
{
    [SerializeField] Collider doorCollider;
    [SerializeField] Animation doorAnimation;
    [SerializeField] AnimationClip doorCloseClip;
    [SerializeField] bool animReverse;
    [Header("Sound")]
    [SerializeField] FMODUnity.StudioEventEmitter audioSource;
    [SerializeField] bool playSoundOnce = true;


    bool hasTriggerdAnim = false;
    bool animFinish = false;


    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("SubmarineCollider"))
            return;

        if (!hasTriggerdAnim)
        {
            doorCollider.enabled = true;

            if (animReverse)
            {
                doorAnimation[doorCloseClip.name].normalizedTime = 1.0f;
                doorAnimation[doorCloseClip.name].speed = -1;
            }
            doorAnimation.Play(doorCloseClip.name);

            if (playSoundOnce && audioSource)
            {
                audioSource.Play();
            }

            hasTriggerdAnim = true;
        }
    }

    void Update()
    {
        if (!playSoundOnce && hasTriggerdAnim && audioSource && !animFinish)
        {
            if (doorAnimation.isPlaying && !audioSource.IsPlaying()) audioSource.Play();
            else if (!doorAnimation.isPlaying && audioSource.IsPlaying())
            {
                audioSource.Stop();
                animFinish = true;
            } 
        }
    }
}
