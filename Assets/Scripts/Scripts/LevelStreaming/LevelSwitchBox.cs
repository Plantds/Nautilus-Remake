using UnityEngine;

public class LevelSwitchBox : MonoBehaviour
{
    public enum ColliderType
    {
        Player,
        Submarine
    }
    [SerializeField] private ColliderType colliderType;
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
        
        EnteredVan();
    }
    void EnteredVan()
    {
        LevelStreamingManager.GetInstance().EnteredCart(this);
    }
}