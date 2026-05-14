using ArtNotes.PhysicalInteraction;
using UnityEngine;

public class EndingOrbInteract : InteractableObject
{
    Interactor interactor;
    [SerializeField] AudioTrigger ambienceAudio;
    void Start()
    {
        interactor = FindAnyObjectByType<Interactor>();
    }
    void Update()
    {
    }

    public override void InteractStart(RaycastHit hit)
    {
        base.InteractStart(hit);

        _mainExecutor.Execute(1);
        ambienceAudio.emitter.Stop();

        interactor.endInteraction();
        enabled = false;
    }
}
