using UnityEngine;

public class ValveStateAudio : MonoBehaviour
{
    [SerializeField] FMODUnity.StudioEventEmitter middleAudio;
    [SerializeField] FMODUnity.StudioEventEmitter endAudio;
    [SerializeField] Collider[] middleColliders;
    [SerializeField] Collider[] endColliders;
    bool firstTime = true;
    void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("ValveStateAudio"))
            return;

        foreach (var collider in middleColliders)
        {
            if (other == collider && !firstTime)
            {
                middleAudio.Play();
            }
        }
        foreach (var collider in endColliders)
        {
            if (other == collider && !firstTime)
            {
                endAudio.Play();
            }
        }
        firstTime = false;
    }
}
