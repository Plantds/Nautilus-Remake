using KinematicCharacterController;
using UnityEngine;

public class SC_DeathBox : MonoBehaviour
{
    [SerializeField] bool outSideDeathBox = true;
    GameObject respawnPoint;

    ReloadLoadSave loadSaveSystem;

    void Start()
    {
        
    }

    void findObjects()
    {
        if (FindAnyObjectByType<SC_RepawnPoint>())
        {  
            respawnPoint = FindAnyObjectByType<SC_RepawnPoint>().gameObject;
        }
        else Debug.Log("DeathBox_Error: No respawnPoint found");

        loadSaveSystem = FindAnyObjectByType<ReloadLoadSave>();
        if (!loadSaveSystem)
            Debug.Log("DeathBox_Error: No ReloadLoadSave component found");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        findObjects();

        if (other.gameObject.GetComponent<CharacterControllerComponent>().playerState == PlayerState.OUTSIDE_SUBMARINE && outSideDeathBox)
        {
            loadSaveSystem.Death(ReloadLoadSave.deathType.player);
        }
        else if (other.gameObject.GetComponent<CharacterControllerComponent>().playerState == PlayerState.INSIDE_SUBMARINE && !outSideDeathBox)
        {
            loadSaveSystem.Death(ReloadLoadSave.deathType.player);
        }
    }
}
