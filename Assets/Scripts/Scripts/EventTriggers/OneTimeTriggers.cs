using System.Collections.Generic;
using System.Linq;
using AASave;
using UnityEngine;

public class OneTimeTriggers : MonoBehaviour
{
    public string id;
    public enum CheckpintType
    {
        Player,
        Submarine
    }
    public CheckpintType checkpintType;
    SaveSystem saveSystem;
    void Start()
    {
        saveSystem = FindAnyObjectByType<SaveSystem>();
        if (saveSystem == null)
            Debug.Log("EventTrigger_Error: No SaveSystem found");
    }

    void OnTriggerEnter(Collider other)
    {
        if (checkpintType == CheckpintType.Player)
        {
            tag = "Player";
        }
        else
        {
            tag = "SubmarineCollider";
        }
        if (!other.gameObject.CompareTag(tag) || saveSystem == null)
            return;

        GetComponent<Collider>().enabled = false;

        string[] ids = saveSystem.LoadArray("TriggerIds");
        List<string> idsList = ids.ToList<string>();
        idsList.Add(id);
        ids = idsList.ToArray();
        saveSystem.Save("TriggerTempIds", ids);

        Debug.Log("Ids new amount: "+ids.Count()+" Id: " + id);
    }
}
