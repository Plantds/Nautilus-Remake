using System.Collections;
using UnityEngine;
public class AnimationTrigger : MonoBehaviour
{
    public enum ColliderType
    {
        Player,
        Submarine
    }
    [SerializeField] private ColliderType colliderType;

    [SerializeField] private Animation anim;
    [SerializeField] private AnimationClip animClip;
    [SerializeField] private bool animReverse;

    [Header("Mega prototype not proper way of doing stuff")]
    [SerializeField] bool InvisibleAfterPlaying;
    [SerializeField] GameObject AnimObject;
    [SerializeField] Transform AnimTransform;
    [SerializeField] float TimeBeforeDisapear;


    private void OnValidate()
    {
        if (!gameObject.GetComponent<Collider>())
            Debug.LogError("AnimTrigger Gameobject Is Missing A Collider");

    }

    private void UpdateTrigger(bool _entering)
    {
        
        if (_entering)
        {
            if (InvisibleAfterPlaying)
            {
                AnimObject.transform.position = AnimTransform.position;
                AnimObject.transform.rotation = AnimTransform.rotation;
                StartCoroutine(timeForDisapear());
            }
            if (animReverse)
            {
                anim[animClip.name].normalizedTime = 1.0f;
                anim[animClip.name].speed = -1;
            }
            anim.Play(animClip.name);            
        }
    }
    IEnumerator timeForDisapear()
    {
        yield return new WaitForSeconds(TimeBeforeDisapear);

        AnimObject.transform.position = new Vector3(100000,100000,100000);

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
}