using FMODUnity;
using UnityEngine;
using UnityEngine.VFX;

public class CS_ElectirkSparkSoundSwtich : CS_ElctricalBaseScript
{
    private StudioEventEmitter Sound;

    private void Start()
    {
        Sound = GetComponent<StudioEventEmitter>();
    }

    public override void TurnOn(bool flash)
    {
        Sound.enabled = true;
    }

    public override void TurnOff(bool flash)
    {
        Sound.enabled = false;
    }
}
