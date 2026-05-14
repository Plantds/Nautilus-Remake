using ArtNotes.PhysicalInteraction;
using UnityEngine;
using KinematicCharacterController;
using FMODUnity;

public class CS_LadderClickInteraction : InteractableObject
{
    [Header("Teleport")]
    [SerializeField] private Transform target;                 // Where to teleport
    [SerializeField] private KinematicCharacterMotor motor;    // Player motor

    [Header("FMOD")]
    [SerializeField] private EventReference interactEvent;

    private void Awake()
    {
        // Auto-find player motor if not assigned
        if (motor == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                motor = player.GetComponent<KinematicCharacterMotor>();
            }
        }
    }

    public override void InteractStart(RaycastHit hit)
    {
        base.InteractStart(hit);

        if (motor == null)
        {
            Debug.LogError($"[{name}] No KinematicCharacterMotor assigned/found.");
            return;
        }

        if (target == null)
        {
            Debug.LogError($"[{name}] No teleport target assigned.");
            return;
        }

        // === Your line, but on the player motor ===
        motor.SetPosition(target.position);

        // Optional sound
        if (!interactEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(interactEvent, target.position);
        }
    }

    public override void InteractEnd()
    {
        base.InteractEnd();
    }
}
