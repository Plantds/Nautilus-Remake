using UnityEngine;
using UnityEngine.VFX;

public class VfxTrigger : MonoBehaviour
{
    public enum ZoneBehaviour
    {
        PLAY_ONCE,
        PLAY_WHILE_IN_ZONE,
    }
    [SerializeField] private ZoneBehaviour behaviour;
    public enum ColliderType
    {
        Player,
        Submarine
    }
    [SerializeField] private ColliderType colliderType;

    [SerializeField] private VisualEffect[] visualEffects;

    [SerializeField] private bool enable = true;


    private void OnValidate()
    {
        if (!gameObject.GetComponent<Collider>())
            Debug.LogError("VfxTrigger Gameobject Is Missing A Collider");

    }

    void Start()
    {
        foreach (var vfx in visualEffects)
        {
            if (!vfx.enabled)
            {
                vfx.enabled = true;
                vfx.Stop();
            }
        }
    }

    private void UpdateTrigger(bool _entering)
    {
        switch (behaviour)
        {
            case ZoneBehaviour.PLAY_ONCE:
                {
                    if (_entering)
                    {
                        foreach (var vfx in visualEffects)
                        {
                            if (enable)
                            {
                                vfx.Play();
                            }
                            else
                            {
                                vfx.Stop();
                            }
                        }
                    }
                    break;
                }
            case ZoneBehaviour.PLAY_WHILE_IN_ZONE:
                {
                    if (_entering)
                    {
                        foreach (var vfx in visualEffects)
                        {
                            if (enable)
                            {
                                vfx.Play();
                            }
                            else
                            {
                                vfx.Stop();
                            }
                        }
                    }
                    else
                    {
                        foreach (var vfx in visualEffects)
                        {
                            if (enable)
                            {
                                vfx.Stop();
                            }
                            else
                            {
                                vfx.Play();
                            }
                        }
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