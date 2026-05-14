using UnityEngine;
using UnityEngine.VFX;
using ArtNotes.PhysicalInteraction;
using System;
using VLB;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.UI;
using UnityEditor.Playables;

public enum AirlockSequenceState
{
    CLOSING,
    PRECLOSING,
    NEUTRAL,
    PREOPENING,
    OPENING,
}

public class SC_SubmarineAirlockSequence : MonoBehaviour
{
    public enum LeverState
    {
        DECREASING,
        NEUTRAL,
        INCREASING,
    }

    [Header("References")]
    [SerializeField] private GameObject startPos;
    [SerializeField] private GameObject endPos;

    [SerializeField] private GameObject water;

    

    [SerializeField] private SubmarineAirlockExecutor executor;
    [SerializeField] private SpotLightToggle playertoggle;

    [Header("Animators")]
    [SerializeField] public SC_AnimationPlayer door;
    [SerializeField] public SC_AnimationPlayer ramp;

    [Header("VFX")]
    [SerializeField] private VisualEffect[] visualEffects;

    [Header("Lights")]
    [SerializeField] private GameObject[] HighBeams;
    [SerializeField] private GameObject[] LowBeams;

    [Header("Light Settings (When Inside)")]
    [SerializeField] private float lightIntensityInside = 25000.0f;
    [SerializeField] private float lightRangeInside = 17.0f;
    [SerializeField] private Color volumetricColorInside = new Color(46, 39, 31);

    [Header("Light Settings (When Outside)")]
    [SerializeField] private float lightIntensityOutside = 100.0f;
    [SerializeField] private float lightRangeOutside = 34.0f;
    [SerializeField] private Color volumetricColorOutside = new Color(27, 23, 18);

    // [Header("Fade Settings")]
    [NonSerialized] private FadeOutImg fadeOut;
    [NonSerialized] private Image fadeOutImg;

    [Header("Sequence Settings")]
    [SerializeField] private float sequenceLength = 5.0f;

    [NonSerialized] public LeverState currentWaterState = LeverState.NEUTRAL;
    [NonSerialized] public AirlockSequenceState currentAirlockState = AirlockSequenceState.NEUTRAL;

    [SerializeField] public FMODUnity.StudioEventEmitter floodSound;
    private bool playedSound;

    public bool overrideLights = false;
    public bool isOutsideLights = false;

    private bool isWaterPlaneOn = false;
    private bool isWaterVFXOn = false;
    private bool isWaterVFXUnderWater = false;

    private float timer;

    public bool isReadyForSwitch = true;
    public bool canSetStartState = false;
    void Start()
    {
        StartCoroutine(setStartState());
        findObjects();
    }

    void findObjects()
    {
        fadeOut = FindAnyObjectByType<FadeOutImg>();
        if (fadeOut == null)
            Debug.Log("PlayerSubDeath_Error: No FadeOutImg found");
        else
        {
            fadeOutImg = fadeOut.GetComponent<Image>();
            if (!fadeOutImg.enabled)
            {
                fadeOutImg.enabled = true;
                fadeOutImg.CrossFadeAlpha(0.0f, 0.0f, false);
            }
        }
    }

    IEnumerator setStartState()
    {
        //Can be optimized
        yield return new WaitUntil(() => canSetStartState);
        if (GameManager.GetInstance().GetPlayerState() == PlayerState.OUTSIDE_SUBMARINE)
        {
            ramp.normalizedTime = 1;
            ramp.time = ramp.animator.GetCurrentAnimatorStateInfo(0).length;
            door.normalizedTime = 0;
            door.time = 0;

            currentWaterState = LeverState.INCREASING;
            currentAirlockState = AirlockSequenceState.OPENING;
            playertoggle.ChangeToOutside();
            ChangeLightsToOutside();
            timer = sequenceLength;
            // isWaterVFXUnderWater = true;
        }
        else
        {
            ramp.normalizedTime = 0;
            ramp.time = 0;
            door.normalizedTime = 1;
            door.time = 8.16f;

            currentWaterState = LeverState.DECREASING;
            currentAirlockState = AirlockSequenceState.CLOSING;
            playertoggle.ChangeToInside();
            ChangeLightsToInside();
            timer = 0.0f;
        }
        yield return null;
    }

    // Update is called once per frame
    public void Update()
    {
        float delta = Time.deltaTime;

        WaterStateUpdate();
        AirlockStateUpdate(delta);
    }

    void WaterStateUpdate()
    {
        float currentSignal = executor.Signal;

        if (currentSignal >= 1.0f)
        {
            currentWaterState = LeverState.INCREASING;
        }
        else if (currentSignal <= -1.0f)
        {
            currentWaterState = LeverState.DECREASING;
        }
        else
        {
            // currentWaterState = LeverState.NEUTRAL;
        }
    }

    void AirlockStateUpdate(float _delta)
    {
        switch (currentAirlockState)
        {
            case AirlockSequenceState.CLOSING:
                {
                    switch (currentWaterState)
                    {
                        case LeverState.DECREASING:
                            {
                                // Make Sure Door Closed
                                
                                door.play = true;
                                door.reverse = false;
                                

                                break;
                            }
                        case LeverState.NEUTRAL:
                            {
                                break;
                            }
                        case LeverState.INCREASING:
                            {
                                
                                door.play = true;
                                door.reverse = true;
                                if (door.normalizedTime <= 0.0f)
                                {
                                    currentAirlockState = AirlockSequenceState.PRECLOSING;
                                }
                                
                                break;
                            }
                    }
                    break;
                }
            case AirlockSequenceState.PRECLOSING:
                {
                    switch (currentWaterState)
                    {
                        case LeverState.DECREASING:
                            {
                                currentAirlockState = AirlockSequenceState.CLOSING;
                                break;
                            }
                        case LeverState.NEUTRAL:
                            {
                                break;
                            }
                        case LeverState.INCREASING:
                            {
                                currentAirlockState = AirlockSequenceState.NEUTRAL;
                                break;
                            }
                    }
                    break;
                }
            case AirlockSequenceState.NEUTRAL:
                {
                    switch (currentWaterState)
                    {
                        case LeverState.DECREASING:
                            {
                                currentAirlockState = AirlockSequenceState.PRECLOSING;

                                ChangeLightsToInside();
                                Debug.Log("CHANGING TO INSIDE");
                                break;
                            }
                        case LeverState.NEUTRAL:
                            {
                                break;
                            }
                        case LeverState.INCREASING:
                            {
                                currentAirlockState = AirlockSequenceState.PREOPENING;

                                ChangeLightsToOutside();
                                Debug.Log("CHANGING TO OUTSIDE");

                                break;
                            }
                    }
                    break;
                }
            case AirlockSequenceState.PREOPENING:
                {
                    switch (currentWaterState)
                    {
                        case LeverState.DECREASING:
                            {
                                if (!isWaterPlaneOn)
                                {
                                    isWaterPlaneOn = true;
                                    water.GetComponent<MeshRenderer>().enabled = true;
                                }

                                timer -= _delta;
                                timer = Mathf.Clamp(timer, 0.0f, sequenceLength);

                                if (isWaterVFXOn)
                                {
                                    VisualEffectStop();
                                }

                                if ((timer / sequenceLength) < 0.7f)
                                {
                                    if (playertoggle.isOutside)
                                    {
                                        playertoggle.ChangeToInside();
                                        GameManager.GetInstance().SetPlayerState(PlayerState.INSIDE_SUBMARINE);
                                    }
                                }

                                if ((timer / sequenceLength) < 0.85f)
                                {
                                    if (playertoggle.isOutside)
                                    {
                                        StartCoroutine(fadeInOut());
                                    }
                                }

                                if (timer <= 0.0f)
                                {
                                    currentAirlockState = AirlockSequenceState.NEUTRAL;
                                    isWaterPlaneOn = false;
                                    water.GetComponent<MeshRenderer>().enabled = false;
                                }

                                water.transform.position = Vector3.Lerp(startPos.transform.position, endPos.transform.position, timer / sequenceLength);
                                AirlockRendererFeature.targetHeight = water.transform.position.y;
                                break;
                            }
                        case LeverState.NEUTRAL:
                            {
                                break;
                            }
                        case LeverState.INCREASING:
                            {
                                if (!playedSound)
                                {
                                    floodSound.Play();
                                    playedSound = true;
                                }
                                if (!isWaterPlaneOn)
                                {
                                    isWaterPlaneOn = true;
                                    water.GetComponent<MeshRenderer>().enabled = true;
                                }

                                timer += _delta;
                                timer = Mathf.Clamp(timer, 0.0f, sequenceLength);

                                if (!isWaterVFXOn && !isWaterVFXUnderWater)
                                {
                                    VisualEffectPlay();
                                }

                                if ((timer / sequenceLength) > 0.4f)
                                {
                                    if (!playertoggle.isOutside)
                                    {
                                        StartCoroutine(fadeInOut());
                                    }
                                }

                                if ((timer / sequenceLength) > 0.7f)
                                {
                                    if (!playertoggle.isOutside)
                                    {
                                        playertoggle.ChangeToOutside();
                                        GameManager.GetInstance().SetPlayerState(PlayerState.OUTSIDE_SUBMARINE);
                                    }

                                    isWaterVFXUnderWater = true;
                                    VisualEffectStop();
                                }
                                else
                                {
                                    isWaterVFXUnderWater = false;
                                }

                                if (timer >= sequenceLength)
                                {
                                    currentAirlockState = AirlockSequenceState.OPENING;
                                    isWaterPlaneOn = false;
                                    water.GetComponent<MeshRenderer>().enabled = false;
                                    playedSound = false;
                                }

                                water.transform.position = Vector3.Lerp(startPos.transform.position, endPos.transform.position, timer / sequenceLength);
                                AirlockRendererFeature.targetHeight = water.transform.position.y;
                                break;
                            }
                    }
                    break;
                }
            case AirlockSequenceState.OPENING:
                {
                    switch (currentWaterState)
                    {
                        case LeverState.DECREASING:
                            {
                                ramp.play = true;
                                ramp.reverse = true;
                                if (ramp.normalizedTime <= 0.0f)
                                {
                                    currentAirlockState = AirlockSequenceState.PREOPENING;
                                }

                                break;
                            }
                        case LeverState.NEUTRAL:
                            {
                                break;
                            }
                        case LeverState.INCREASING:
                            {
                                ramp.play = true;
                                ramp.reverse = false;

                                break;
                            }
                    }
                    break;
                }
        }
    }

    public void ChangeLightsToOutside()
    {
        isOutsideLights = true;

        if (!overrideLights)
        {
            foreach (var itr in HighBeams)
            {
                itr.SetActive(false);
            }

            foreach (var itr in LowBeams)
            {
                itr.SetActive(true);
            }
        }


    }

    public void ChangeLightsToInside()
    {
        isOutsideLights = false;
        if (!overrideLights)
        {
            foreach (var itr in HighBeams)
            {
                itr.SetActive(true);
            }

            foreach (var itr in LowBeams)
            {
                itr.SetActive(false);
            }
        }
    }

    IEnumerator fadeInOut()
    {
        findObjects();
        fadeOutImg.CrossFadeAlpha(1, 0.15f, false);
        yield return new WaitForSeconds(2);
        fadeOutImg.CrossFadeAlpha(0, 0.5f, false);
        yield return null;
    }

    void VisualEffectPlay()
    {
        isWaterVFXOn = true;
        foreach (var effect in visualEffects)
        {
            effect.Play();
        }
    }

    void VisualEffectStop()
    {
        isWaterVFXOn = false;
        foreach (var effect in visualEffects)
        {
            effect.Stop();
        }
    }
}
