using UnityEngine;
public class TeleportSubTrigger : MonoBehaviour
{
    public enum ColliderType
    {
        Player,
        Submarine
    }
    /* [SerializeField] */ private ColliderType colliderType = ColliderType.Player;

    [SerializeField] private Transform newTransform;
    // private KinematicCharacterMotor player;

    [Header("Only to see if reference breaks")]
    [SerializeField] private SC_SubmarineMovement submarine;


    private void OnValidate()
    {
        if (!gameObject.GetComponent<Collider>())
            Debug.LogError("TeleportSubTrigger Gameobject Is Missing A Collider");
    }

    void Start()
    {
        // player = FindAnyObjectByType<KinematicCharacterMotor>();
        // if (player == null)
        //     Debug.Log("TeleportSubTrigger_Error: No Player found");
        findObjects();
    }
    void findObjects()
    {
        submarine = FindAnyObjectByType<SC_SubmarineMovement>();
        if (submarine == null)
            Debug.Log("TeleportSubTrigger_Error: No Submarine found");
    }

    private void UpdateTrigger(bool _entering)
    {

        switch (colliderType)
        {
            case ColliderType.Player:
                {
                    if (_entering)
                    {
                        bool teleportOnManual = true;
                        if (submarine.state != SubmarineState.MANUAL)
                        {
                            submarine.TrySwitchState(SubmarineState.MANUAL);
                            teleportOnManual = false;
                        }
                        submarine.SetPositionAndRotation(newTransform.position, newTransform.rotation);
                        if (!teleportOnManual)
                        {
                            submarine.forceAutoPark = true;
                            submarine.TrySwitchState(SubmarineState.AUTOMATIC);
                            submarine.targetPosition = newTransform.position;
                            submarine.targetRotation = newTransform.rotation;
                            submarine.subTeleport = true;
                        }
                        

                    }
                    break;
                }
            // case ColliderType.Submarine:
            //     {
            //         if (_entering)
            //         {
            //             Vector3 subTransformDiffPos = newTransform.position - submarine.transform.position;
            //             Vector3 subTransformDiffRot = newTransform.rotation.eulerAngles - submarine.transform.rotation.eulerAngles;
            //             submarine.SetPositionAndRotation(newTransform.position, newTransform.rotation);

            //             player.SetPositionAndRotation(player.GetComponent<CharacterControllerComponent>().transform.position + subTransformDiffPos, /* Add quaternions together */)

            //         }
            //         break;
            //         // player.GetComponent<CharacterControllerComponent>().transform.position;
            //     }
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

        findObjects();
        UpdateTrigger(true);
    }
}