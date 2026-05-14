using UnityEngine;
using FMODUnity;
using ArtNotes.PhysicalInteraction;

public class PianoInteraction : InteractableObject
{
    [Header("FMOD")]
    [SerializeField] private EventReference pianoEvent;   // Drag your FMOD event here

    [Header("Debug")]
    [SerializeField] private bool logOnInteract = false;

    // Called when the interactor "clicks" on this object
    public override void InteractStart(RaycastHit hit)
    {
        // If base has logic you care about, you can call it.
        // Button doesn't, so we keep it like this.
        // base.InteractStart(hit);

        if (pianoEvent.IsNull)
        {
            Debug.LogWarning("[PianoInteraction] No FMOD event assigned!", this);
            return;
        }

        if (logOnInteract)
            Debug.Log("[PianoInteraction] Playing piano FMOD event", this);

        // Play the FMOD event at the piano's position (or hit.point if you prefer)
        RuntimeManager.PlayOneShot(pianoEvent, transform.position);
    }

    // Optional override, only if you need something when interaction ends
    public override void InteractEnd()
    {
        base.InteractEnd();
        // Nothing for now – piano just plays on click.
    }
}
