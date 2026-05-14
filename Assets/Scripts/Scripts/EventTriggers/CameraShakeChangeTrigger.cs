using UnityEngine;

public class CameraShakeChangeTrigger : MonoBehaviour
{
    public enum ColliderType
    {
        Player,
        Submarine
    }
    [SerializeField] private ColliderType colliderType;

    [SerializeField] private float intesity = 0.1f;

    [SerializeField] CameraShakeTrigger cameraShakeChangeTrigger ;

    private void OnValidate()
    {
        if (!gameObject.GetComponent<Collider>())
            Debug.LogError("CameraShake Gameobject Is Missing A Collider");

    }

    private void UpdateTrigger(bool _entering)
    {        
        if (_entering)
        {
            cameraShakeChangeTrigger.intesity = intesity;
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