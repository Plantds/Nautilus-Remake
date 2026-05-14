using UnityEngine;
using FMODUnity;

public class FMODTriggerControl : MonoBehaviour
{
    public enum TriggerMode { Play, Stop }

    public TriggerMode mode = TriggerMode.Play;
    public StudioEventEmitter emitter;

    private void OnTriggerEnter(Collider other)
    {
        // Only react to the Player (optional – remove if you want anything to trigger it)
        if (!other.CompareTag("Player")) return;

        if (mode == TriggerMode.Play)
        {
            if (!emitter.IsPlaying())
            {
                emitter.Play();
            }
        }
        else if (mode == TriggerMode.Stop)
        {
            emitter.Stop(); // This will fade out because of "Allow Fadeout"
        }
    }
}
