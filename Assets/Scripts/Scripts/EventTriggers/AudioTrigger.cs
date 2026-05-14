using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public enum ZoneBehaviour
    {
        PLAY_ONCE,
        PLAY_WHILE_IN_ZONE,
    }
    [Tooltip("If play_while_in_zone the sound need to be looping in fmod")]
    [SerializeField] private ZoneBehaviour behaviour;
    public enum ColliderType
    {
        Player,
        Submarine
    }
    [SerializeField] private ColliderType colliderType;


    [SerializeField] public FMODUnity.StudioEventEmitter emitter;

    private void OnValidate()
    {
        if (!gameObject.GetComponent<Collider>())
            Debug.LogError("Audio Trigger Gameobject Is Missing A Collider");

        if (!emitter)
            Debug.LogError("Audio Trigger Gameobject Is Missing A Emitter");
    }

    private void UpdateTrigger(bool _entering)
    {
        switch (behaviour)
        {
            case ZoneBehaviour.PLAY_ONCE:
                {
                    if (_entering)
                    {
                        emitter.Play();
                    }
                    break;
                }
            case ZoneBehaviour.PLAY_WHILE_IN_ZONE:
                {
                    if (_entering)
                    {
                        emitter.Play();
                    }
                    else
                    {
                        emitter.Stop();
                    }
                    break;
                }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (colliderType == ColliderType.Player)
        {
            tag = "Player";
        }
        else
        {
            tag = "SubmarineCollider";
        }
        if (!other.gameObject.CompareTag(tag))
            return;

        UpdateTrigger(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (colliderType == ColliderType.Player)
        {
            tag = "Player";
        }
        else
        {
            tag = "SubmarineCollider";
        }
        if (!other.gameObject.CompareTag(tag))
            return;

        UpdateTrigger(false);
    }
}