using UnityEngine;

public class SC_ForceAutoParkTrigger : MonoBehaviour
{
    [SerializeField] FMODUnity.StudioEventEmitter audioSource;
    SC_SubmarineMovement submarine;
    public GameObject point;
    public bool lastPark = false;

    void Awake()
    {
        submarine = GameObject.FindGameObjectWithTag("Submarine").GetComponent<SC_SubmarineMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("SubmarineCollider"))
            return;

        if (lastPark)
        {
            submarine.lastPark = true;
        }

        submarine.forceAutoPark = true;
        submarine.TrySwitchState(SubmarineState.AUTOMATIC);

        submarine.targetPosition = point.transform.position;
        submarine.targetRotation = point.transform.rotation;

        

        audioSource.Play();

        // submarine.canAutoPark = true;
    }
}
