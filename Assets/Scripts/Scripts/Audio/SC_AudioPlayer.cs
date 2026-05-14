using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // [Moved code to audioTrigger]


    // public enum ZoneBehaviour
    // {
    //     PLAY_ONCE,
    //     PLAY_WHILE_IN_ZONE,
    // }
    // [SerializeField] private ZoneBehaviour behaviour;
    // public enum ColliderType
    // {
    //     Player,
    //     Submarine
    // }
    // [SerializeField] private ColliderType colliderType;


    // [SerializeField] private FMODUnity.StudioEventEmitter emitter;

    // private void OnValidate() {
    //     if(!gameObject.GetComponent<Collider>())
    //         Debug.LogError("Audio Player Gameobject Is Missing A Collider");

    //     if(!emitter)
    //         Debug.LogError("Audio Player Gameobject Is Missing A Emitter");
    // }

    // private void UpdateAudio(bool _entering)
    // {
    //     switch (behaviour)
    //     {
    //         case ZoneBehaviour.PLAY_ONCE:
    //             {
    //                 if(_entering)
    //                 {
    //                     emitter.Play();
    //                 }
    //                 break;
    //             }
    //         case ZoneBehaviour.PLAY_WHILE_IN_ZONE:
    //             {
    //                 if(_entering)
    //                 {
    //                     emitter.Play();
    //                 }
    //                 else
    //                 {
    //                     emitter.Stop();
    //                 }
    //                 break;
    //             }
    //     }
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (colliderType == ColliderType.Player)
    //     {
    //         tag = "Player";
    //     }
    //     else
    //     {
    //         tag = "SubmarineCollider";
    //     }
    //     if (!other.gameObject.CompareTag(tag))
    //         return;

    //     UpdateAudio(true);
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (colliderType == ColliderType.Player)
    //     {
    //         tag = "Player";
    //     }
    //     else
    //     {
    //         tag = "SubmarineCollider";
    //     }
    //     if (!other.gameObject.CompareTag(tag))
    //         return;

    //     UpdateAudio(false);
    // }
}
