using UnityEngine;

public class SC_SubmarineParkingTrigger : MonoBehaviour
{
    SC_SubmarineMovement submarine;
    public GameObject point;

    [SerializeField] FMODUnity.StudioEventEmitter CanParkAudioSorce;

    public bool forceAutoPark = false;

    void Awake()
    {
        submarine = GameObject.FindGameObjectWithTag("Submarine").GetComponent<SC_SubmarineMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("SubmarineCollider"))
        {
            submarine.targetPosition = point.transform.position;
            submarine.targetRotation = point.transform.rotation;

            submarine.forceAutoPark = forceAutoPark;
            
            submarine.canAutoPark = true;

            if(forceAutoPark)
            {
                gameObject.SetActive(false);
            }

            CanParkAudioSorce.Play();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SubmarineCollider"))
        {
            submarine.canAutoPark = false;
        }
    }
}
