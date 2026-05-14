using ArtNotes.PhysicalInteraction;
using UnityEngine;

public class CS_LadderInteraction : InteractableObject
{
    [SerializeField] private GameObject _player;
    private PlayerComponent pc;

    private void Start()
    {
        pc = _player.GetComponent<PlayerComponent>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InteractEnd();
        }
    }

    public override void InteractStart(RaycastHit hit)
    {
        base.InteractStart(hit);
        pc.Character.TransitionToState(CharacterState.Climbing);
        pc.Character.SetCurrentClimbObject(transform);
    }

    public override void InteractEnd()
    {
        pc.Character.TransitionToState(CharacterState.Default);
        pc.Character.SetCurrentClimbObject(null);
        base.InteractEnd();
    }
}
