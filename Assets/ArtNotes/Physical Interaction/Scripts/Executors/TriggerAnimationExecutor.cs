using UnityEngine;

namespace ArtNotes.PhysicalInteraction
{
    public class TriggerAnimationExecutor : Executor
    {
        [SerializeField] Animation anim;

        [SerializeField] AnimationClip animClip;

        [SerializeField] EmissionAndLightLerp emissionAndLightLerp;

        [SerializeField] float triggerWhenAbove;
        [Header("Sound")]
        [SerializeField] FMODUnity.StudioEventEmitter audioSource;
        [SerializeField] bool playSoundOnce = true;

        bool hasTriggerdAnim = false;
        bool animFinish = false;

        void Awake()
        {
        }

        public override void Execute(float signal)
        {
            if (!hasTriggerdAnim && signal >= triggerWhenAbove)
            {
                anim.Play(animClip.name);
                hasTriggerdAnim = true;

                if (playSoundOnce && audioSource)
                {
                    audioSource.Play();
                }

                if (emissionAndLightLerp)
                {
                    emissionAndLightLerp.Play();
                }
            }
        }

        void Update()
        {
            if (!playSoundOnce && hasTriggerdAnim && audioSource && !animFinish)
            {
                if (anim.isPlaying && !audioSource.IsPlaying()) audioSource.Play();
                else if (!anim.isPlaying && audioSource.IsPlaying())
                {
                    audioSource.Stop();
                    animFinish = true;
                } 
            }
        }
    }
}
