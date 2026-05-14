using UnityEngine;

public class AirlockVolumeHandler : MonoBehaviour
{
    [SerializeField] private GameObject AirlockOutsideVolume01;
    [SerializeField] private GameObject AirlockOutsideVolume02;

    private GameManager instance;

    private PlayerState currentState = PlayerState.OUTSIDE_SUBMARINE;

    void Start()
    {
        instance = GameManager.GetInstance();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(instance.GetPlayerState() != currentState)
        {
            currentState = instance.GetPlayerState();
            AirlockOutsideVolume01.SetActive(currentState == PlayerState.OUTSIDE_SUBMARINE);
            AirlockOutsideVolume02.SetActive(currentState == PlayerState.OUTSIDE_SUBMARINE);
        }
    }
}
