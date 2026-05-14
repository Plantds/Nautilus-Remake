using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OpacityTrigger : MonoBehaviour
{
    public enum ColliderType
    {
        Player,
        Submarine
    }
    [SerializeField] private ColliderType colliderType;

    [SerializeField] public List<Material> materials = new List<Material>();

    [SerializeField] public List<VisualEffect> effects = new List<VisualEffect>();

    [SerializeField] private string propertyName = "_Alpha_Power";

    public float maxOpasity = 1;
    public float minOpasity = 0;
    public bool turnOff = true;


    private void OnValidate()
    {
        if (!gameObject.GetComponent<Collider>())
            Debug.LogError("CameraShake Gameobject Is Missing A Collider");

    }
    void Start()
    {
        foreach (var mat in materials)
        {
            if (mat == null) continue;
            if (!mat.HasProperty(propertyName)) continue;

            if (turnOff)
            {
                mat.SetFloat(propertyName, maxOpasity);
            }
            else
            {
                mat.SetFloat(propertyName, minOpasity);
            }
        }
    }

    private void UpdateTrigger(bool _entering)
    {        
        if (_entering)
        {
            foreach (var effect in effects)
            {

                if (turnOff)
                {
                    // Destroy(effect.visualEffectAsset);
                    effect.visualEffectAsset = null;
                    // Destroy(effect);
                }
            }
            foreach (var mat in materials)
            {
                if (mat == null) continue;
                if (!mat.HasProperty(propertyName)) continue;

                if (turnOff)
                {
                    mat.SetFloat(propertyName, minOpasity);
                }
                else
                {
                    mat.SetFloat(propertyName, maxOpasity);
                }
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