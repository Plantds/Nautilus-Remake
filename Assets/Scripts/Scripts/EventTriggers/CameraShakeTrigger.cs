using UnityEngine;

public class CameraShakeTrigger : MonoBehaviour
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

    [SerializeField] public float intesity = 0.1f;
    [Tooltip("Only if PLAY_ONCE")]
    [SerializeField] private float duration = 0.3f;


    private CharacterCameraComponent cameraComponent;

    private void OnValidate()
    {
        if (!gameObject.GetComponent<Collider>())
            Debug.LogError("CameraShake Gameobject Is Missing A Collider");

    }

    private void Start()
    {
        cameraComponent = FindAnyObjectByType<CharacterCameraComponent>();
    }

    private void UpdateTrigger(bool _entering)
    {
        switch (behaviour)
        {
            case ZoneBehaviour.PLAY_ONCE:
                {
                    if (_entering)
                    {
                        cameraComponent.SetCameraShakeDurationAndIntensity(true, true, duration, intesity);
                    }
                    break;
                }
            case ZoneBehaviour.PLAY_WHILE_IN_ZONE:
                {
                    if (_entering)
                    {
                        cameraComponent.SetCameraShakeDurationAndIntensity(true, false, 0.0f, intesity);
                    }
                    else
                    {
                        cameraComponent.SetCameraShakeDurationAndIntensity(false, false, 0.0f, 0.0f);
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