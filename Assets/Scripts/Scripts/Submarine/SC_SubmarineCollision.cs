using UnityEngine;

public class SC_SubmarineCollision : MonoBehaviour
{
    public SC_SubmarineMovement submarineMovement;
    [Tooltip("How much the submarine bounces after a collision\n\n1 = bounce back by equal force to the submarines current velocity\n2 = half of the submarines current velocity")]
    public float submarineCollisionAbsorb = 1;
    float subCollisionBounce = 1.0f;

    public SubmarineDamageSystem submarineDamageSystem;
    [SerializeField] FMODUnity.StudioEventEmitter audioSource;

    void Start()
    {
        if (submarineCollisionAbsorb == 0)
        {
            submarineCollisionAbsorb = 1;
        }
        //rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        subCollisionBounce = submarineMovement.LinearVelocity.magnitude / submarineCollisionAbsorb;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.tag.Contains("Player"))
        {
            submarineMovement.ApplyForceAtPoint(collision.contacts[0].normal * subCollisionBounce, collision.contacts[0].point);
            if (audioSource)
            {
                audioSource.Play();
            }
        }
    }
}
