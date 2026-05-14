using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SC_AudioManager : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference eventReference;

    [SerializeField] private FMODUnity.StudioEventEmitter emitter;
    [SerializeField] private FMODUnity.StudioGlobalParameterTrigger trigger;

    [SerializeField] private ClampedFloatParameter cutoff = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

    void Start()
    {
        emitter.Play();
    }

    // Update is called once per frame
    void Update()
    {
        trigger.Value = cutoff.value;
        trigger.TriggerParameters();
    }
}
