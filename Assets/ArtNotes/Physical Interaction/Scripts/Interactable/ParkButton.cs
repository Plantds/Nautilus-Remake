using ArtNotes.PhysicalInteraction;
using UnityEngine;

public class ParkButton : Button
{
    bool lampOn = false;
    bool buttonInteractive = false;
    bool isParking;
    [SerializeField] EmissionAndLightLerp lampLerpOn;
    [SerializeField] EmissionAndLightLerp lampLerpOff;
    [SerializeField] EmissionAndLightLerp buttonLerp;
    [SerializeField] Light parkLight;
    [SerializeField] Color canParkColor;
    [SerializeField] Color isParkingColor;
    [SerializeField] Color buttonBlinkColor;

    [SerializeField] FMODUnity.StudioEventEmitter OnParkStartAudioSource;
    [SerializeField] FMODUnity.StudioEventEmitter ParkingCanceledAudioSource;


    // [SerializeField] Color isParkedColor;

    [SerializeField] SC_SubmarineMovement subMovement;

    void OnValidate()
    {
        buttonLerp.emissiveMaterials[0].SetColor("_Emissive_Tint", buttonBlinkColor);
    }
    void Update()
    {
        if (subMovement.canAutoPark && !isParking && !subMovement.isSubmarineParked)
        {
            //Can park
            buttonInteractiveOn();
            lampLerpOn.emissiveMaterials[0].SetColor("_Emissive_Tint", canParkColor);
            parkLight.color = canParkColor;
            lampLerpOn.looping = false;
            if (!lampOn)
            {
                turnOnLamp();
            } 
        }
        else if(isParking && !subMovement.isSubmarineParked)
        {
            //Is parking
            buttonInteractiveOff();
            lampLerpOn.emissiveMaterials[0].SetColor("_Emissive_Tint", isParkingColor);
            parkLight.color = isParkingColor;
            lampLerpOn.looping = false;
            if (!lampOn)
            {
                turnOnLamp();
            }
        }

        if (!subMovement.canAutoPark && !isParking)
        {
            //Normal
            buttonInteractiveOff();
            if (lampOn)
            {
                turnOffLamp();
            } 
        }

        if (subMovement.isSubmarineParked)
        {
            //Is parked
            buttonInteractiveOn();
            if (lampOn)
            {
                turnOffLamp();
            }
        }
    }
    void turnOnLamp()
    {
        lampLerpOff.Stop();
        lampLerpOn.Play();
        lampOn = true;
    }
    void turnOffLamp()
    {
        lampLerpOn.Stop();
        lampLerpOff.Play();
        lampOn = false;
    }
    void buttonInteractiveOn()
    {
        if (!buttonInteractive)
        {
            buttonLerp.Play();
            buttonInteractive = true;
        }
    }
    void buttonInteractiveOff()
    {
        if (buttonInteractive)
        {
            buttonLerp.Stop();
            buttonLerp.OnReset();
            buttonInteractive = false;
        }
    }
    public override void InteractStart(RaycastHit hit)
    {
        base.InteractStart(hit);

        if (subMovement.canAutoPark && !isParking)
        {
            subMovement.beginAutomaticMode = true;
            subMovement.TrySwitchState(SubmarineState.AUTOMATIC);
            isParking = true;
            OnParkStartAudioSource.Play();
        }
        else if (isParking)
        {
            subMovement.TrySwitchState(SubmarineState.MANUAL);
            isParking = false;
            ParkingCanceledAudioSource.Play();
        }
    }
}