using System.Collections.Generic;
using UnityEngine;

public class EmiisionLerpTrigger : MonoBehaviour
{
    public enum ColliderType
    {
        Player,
        Submarine
    }
    [SerializeField] private ColliderType colliderType;

    [SerializeField] public List<EmissionAndLightLerp> emissionAndLightLerps = new List<EmissionAndLightLerp>();


    private void OnValidate()
    {
        if (!gameObject.GetComponent<Collider>())
            Debug.LogError("CameraShake Gameobject Is Missing A Collider");

    }

    private void UpdateTrigger(bool _entering)
    {        
        if (_entering)
        {
            foreach (var emissionAndLightLerp in emissionAndLightLerps)
            {
                if (!emissionAndLightLerp.enabled)
                {
                    emissionAndLightLerp.enabled = true;
                }
                // emissionAndLightLerp.Play();
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

    //     UpdateTrigger(false);
    // }
}