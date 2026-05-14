using System.Collections;
using UnityEngine;

public class EnableColliderTrigger : MonoBehaviour
{
    public enum ColliderType
    {
        Player,
        Submarine
    }
    [SerializeField] private ColliderType colliderType;


    [SerializeField] private Collider[] colliders;

    [Tooltip("If false then disables it instead")]
    [SerializeField] private bool enableCollider = true;

    [SerializeField] private float delay = 0;

    private void OnValidate()
    {
        if (!gameObject.GetComponent<Collider>())
            Debug.LogError("Enable Collider Trigger Gameobject Is Missing Its Own Collider");
    }

    private void UpdateTrigger(bool _entering)
    {
              
        if (_entering)
        {
            foreach (var collider in colliders)
            {
                StartCoroutine(SpawnDespawnCollider(collider));
            }
        }              
        
    }

    IEnumerator SpawnDespawnCollider(Collider _collider)
    {
        yield return new WaitForSeconds(delay);

        if (!enableCollider)
        {
            var oldPos = _collider.gameObject.transform.position;

            _collider.gameObject.transform.position = new Vector3(10000, 10000, 10000);

            yield return new WaitForSeconds(2);

            _collider.enabled = enableCollider;
            _collider.gameObject.transform.position = oldPos;
        }
        else
        {
            _collider.enabled = enableCollider;
        }

        yield return null;
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
