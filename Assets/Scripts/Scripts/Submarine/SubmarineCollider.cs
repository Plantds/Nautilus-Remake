using UnityEngine;

public class SubmarineCollider : MonoBehaviour
{
    public SC_SubmarineMovement submarineMovement;
    public SubmarineDamageSystem submarineDamageSystem;
    CharacterCameraComponent cameraComponent;
    public float cameraShakeIntensityMultiplier = 0.1f;
    public float cameraShakeDurationMultiplier = 0.2f;
    public FMODUnity.StudioEventEmitter audioSource;
    private void Start() 
    {
        cameraComponent = FindAnyObjectByType<CharacterCameraComponent>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.tag.Contains("Player") && submarineMovement.collisionOn && !submarineMovement.isParking)
        {
            //submarineDamageSystem.OnHit(submarineMovement.LinearVelocity.magnitude);
            
            cameraComponent.SetCameraShakeDurationAndIntensity(true, true , /* submarineMovement.LinearVelocity.magnitude * */ cameraShakeDurationMultiplier, submarineMovement.LinearVelocity.magnitude * cameraShakeIntensityMultiplier);

            audioSource.Play();

            submarineMovement.OnCollisionCallBack(collision);
        }
    }

}
