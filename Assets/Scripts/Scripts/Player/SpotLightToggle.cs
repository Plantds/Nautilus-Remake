using UnityEngine;

public class SpotLightToggle : MonoBehaviour
{
    public GameObject spotlight;
    public GameObject AreaLight;

    public GameObject Canvas;
    public FMODUnity.StudioEventEmitter Audio;
    public bool startOn = true;

    public ParticleSystem particle01;
    public ParticleSystem particle02;

    public FMODUnity.StudioEventEmitter emitter;
    public FMODUnity.StudioEventEmitter emitterInterior;

    // NEW: flashlight on/off sounds
    public FMODUnity.StudioEventEmitter flashlightOnEmitter;
    public FMODUnity.StudioEventEmitter flashlightOffEmitter;

    private float timer;
    public float interval = 3.0f;

    bool isPlayingSound = false;

    float delayTimer = 0.0f;
    public float delay = 0.8f;

    public bool isOutside = false;

    void Awake()
    {
        if (startOn)
        {
            ChangeToOutside();
        }
        else
        {
            ChangeToInside();
        }

        timer = 0.0f;

        particle01.Stop();
        particle02.Stop();
    }

    void Update()
    {
        if (isOutside)
        {
            if (!isPlayingSound)
            {
                timer += Time.deltaTime;
            }
            else
            {
                delayTimer += Time.deltaTime;
            }

            if (timer >= interval)
            {
                timer = 0.0f;
                emitter.Play();
                isPlayingSound = true;
            }

            if (delayTimer >= delay)
            {
                delayTimer = 0.0f;
                particle01.Play();
                particle02.Play();
                isPlayingSound = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            bool wasOn = spotlight.activeSelf || AreaLight.activeSelf;

            spotlight.SetActive(!spotlight.activeSelf);
            AreaLight.SetActive(!AreaLight.activeSelf);

            bool isOnNow = spotlight.activeSelf || AreaLight.activeSelf;

            if (isOnNow && flashlightOnEmitter != null)
            {
                flashlightOnEmitter.Play();
            }
            else if (!isOnNow && flashlightOffEmitter != null)
            {
                flashlightOffEmitter.Play();
            }
        }
    }

    public void ChangeToOutside()
    {
        emitterInterior.Stop();
        isOutside = true;
        Audio.Play();
        Canvas.SetActive(true);
    }

    public void ChangeToInside()
    {
        emitterInterior.Play();
        isOutside = false;
        Audio.Stop();
        Canvas.SetActive(false);
    }
}
