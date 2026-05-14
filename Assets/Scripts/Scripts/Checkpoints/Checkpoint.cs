using AASave;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public enum CheckpintType
    {
        Player,
        Submarine
    }

    public CheckpintType checkpintType;
    SaveSystem saveSystem;
    CharacterControllerComponent player;
    SC_SubmarineMovement sub;
    LevelStreamingManager levelManager;
    SubmarineDamageSystem damageSystem;
    GameManager gameManager;



    string tag = "NoTag";
    void Start()
    {
        saveSystem = FindAnyObjectByType<SaveSystem>();
        if (saveSystem == null)
            Debug.Log("Checkpoint_Error: No SaveSystem found");

        player = FindAnyObjectByType<CharacterControllerComponent>();
        if (player == null)
            Debug.Log("Checkpoint_Error: No Player found");

        sub = FindAnyObjectByType<SC_SubmarineMovement>();
        if (sub == null)
            Debug.Log("Checkpoint_Error: No Submarine found");

        levelManager = FindAnyObjectByType<LevelStreamingManager>();
        if (levelManager == null)
            Debug.Log("Checkpoint_Error: No LevelStreamingManager found");

        damageSystem = FindAnyObjectByType<SubmarineDamageSystem>();
        if (damageSystem == null)
            Debug.Log("Checkpoint_Error: No SubmarineDamageSystem found");

        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
            Debug.Log("Checkpoint_Error: No gameManager found");
    }

    void Update()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        if (checkpintType == CheckpintType.Player && GameManager.GetInstance().GetPlayerState() == PlayerState.OUTSIDE_SUBMARINE)
        {
            tag = "Player";
        }
        else if(checkpintType == CheckpintType.Submarine && GameManager.GetInstance().GetPlayerState() == PlayerState.INSIDE_SUBMARINE)
        {
            tag = "SubmarineCollider";
        }
        if (!other.gameObject.CompareTag(tag) || saveSystem == null)
            return;
        
        Vector3     playerPosition = player.transform.position;
        Quaternion  playerRotation = player.transform.rotation;

        Vector3     subPosition = sub.transform.position;
        Quaternion  subRotation = sub.transform.rotation;

        bool airlockOpen = false;
        if (gameManager.currentPlayerState == PlayerState.OUTSIDE_SUBMARINE)
        {
            airlockOpen = true;
        }

        saveSystem.Save("PlayerPos", playerPosition);
        saveSystem.Save("PlayerRot", playerRotation);
        saveSystem.Save("SubPos", subPosition);
        saveSystem.Save("SubRot", subRotation);
        saveSystem.Save("CurrentLvl", gameObject.scene.name);
        saveSystem.Save("SubHp", damageSystem.shipHp);
        saveSystem.Save("AirlockOpen", airlockOpen);
        saveSystem.Save("TriggerIds", saveSystem.LoadArray("TriggerTempIds"));
        saveSystem.Delete("TriggerTempIds");


        GetComponent<Collider>().enabled = false;
    }
}
