using ArtNotes.PhysicalInteraction;
using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class HeadlightsButton : Button
{
    [SerializeField] bool startOn = true;
    bool lightsOn = false;

    [Header("Headlight Objects")]
    [SerializeField] private GameObject[] HighBeams;
    [SerializeField] private GameObject[] LowBeams;

    [SerializeField] private SC_SubmarineAirlockSequence airlockSequence;

    [Header("Emissive Material")]
    [SerializeField] private List<Material> emissiveMaterials = new List<Material>();
    [SerializeField] private string emissivePropertyName = "_Emissive_Strength";
    [SerializeField] private float emissiveOnStrength = 7.0f;
    [SerializeField] private float emissiveOffStrength = 0.0f;

    [Header("FMOD Emitters (drop GOs here)")]
    [SerializeField] private StudioEventEmitter headlightsOnEmitter;
    [SerializeField] private StudioEventEmitter headlightsOffEmitter;

    [SerializeField] private CS_SubmarineHazardChecker checker;

    public void Start()
    {
        base.Start();
        checker = FindAnyObjectByType<CS_SubmarineHazardChecker>();
        if (!checker.ElIsOff)
        {
            //lightsOn = startOn;
            //SetBeamsActive(lightsOn);
        }
    }

    public override void InteractStart(RaycastHit hit)
    {
        base.InteractStart(hit);
        if (!checker.ElIsOff)
            SetBeamsActive(!lightsOn);
    }

    public void SetBeamsActive(bool state)
    {
        lightsOn = state;

        airlockSequence.overrideLights = !state;

        // Toggle the correct set of beams
        GameObject[] targetArray = airlockSequence.isOutsideLights ? LowBeams : HighBeams;
        foreach (var g in targetArray)
        {
            if (g != null)
                g.SetActive(state);
        }

        // Set emissive strength
        float strength = state ? emissiveOnStrength : emissiveOffStrength;
        ApplyEmission(strength);

        // Play corresponding sound
        if (state)
            PlayEmitter(headlightsOnEmitter);
        else
            PlayEmitter(headlightsOffEmitter);
    }

    private void ApplyEmission(float strength)
    {
        foreach (var mat in emissiveMaterials)
        {
            if (mat == null || !mat.HasProperty(emissivePropertyName))
                continue;

            mat.SetFloat(emissivePropertyName, strength);
        }
    }

    private void PlayEmitter(StudioEventEmitter emitter)
    {
        if (emitter == null) return;
        emitter.Play();
    }
}
