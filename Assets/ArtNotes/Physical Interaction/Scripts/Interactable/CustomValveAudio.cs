using ArtNotes.PhysicalInteraction;
using UnityEngine;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class CustomValveAudio : MonoBehaviour
{
    float lastAngle;
    [SerializeField] CustomValve valve;
    [Header("Audio")]
    FMODUnity.StudioEventEmitter audioSource;
    [SerializeField] float minDeltaAngle = 0.2f;

    bool isPaused;
    bool fadingOut, fadingIn;
    float fadeOutTimer, fadeInTimer;
    public float fadeOutTime, fadeInTime;
    float deltaAngle;

    void Start()
    {
        audioSource = GetComponent<FMODUnity.StudioEventEmitter>();
    }
    void FixedUpdate()
    {
        audioSource.EventInstance.getPaused(out isPaused);
        deltaAngle = Mathf.Abs(Mathf.DeltaAngle(valve._currentAngle, lastAngle));


        if (deltaAngle > minDeltaAngle && !fadingIn)
        {
            //Start
            fadingIn = true;
            fadingOut = false;
            fadeOutTimer = 0;
        }
        else if (deltaAngle <= minDeltaAngle && !fadingOut && !isPaused)
        {
            //Stop
            fadingOut = true;
            fadingIn = false;
            fadeInTimer = 0;
        }
        

        if (fadingIn)
        {
            fadeInTimer += Time.fixedDeltaTime;

            audioSource.EventInstance.setParameterByName("FadeIn", fadeInTimer/fadeInTime);

            if (audioSource.IsPlaying())
            {
                audioSource.EventInstance.setPaused(false);
            }
            else
            {
                audioSource.Play();
            }
            if (fadeInTimer / fadeInTime >= 1)
            {
                fadingIn = false;
            }
        }

        if (fadingOut)
        {
            fadeOutTimer += Time.fixedDeltaTime;

            audioSource.EventInstance.setParameterByName("FadeOut", fadeOutTimer / fadeOutTime);

            if (fadeOutTimer / fadeOutTime >= 1)
            {
                audioSource.EventInstance.setPaused(true);
                fadingOut = false;
            }
        }

        lastAngle = valve._currentAngle;
    }
}
