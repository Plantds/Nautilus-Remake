using UnityEngine;

public class DEBUGSubmarine : MonoBehaviour
{
    private SC_SubmarineMovement submarineMovement;

    [SerializeField] private Vector3 linearVelocity;
    [SerializeField] private Vector3 angularVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        submarineMovement = GetComponent<SC_SubmarineMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // linearVelocity = submarineMovement.linearVelocity;
        // angularVelocity = submarineMovement.angularVelocity;
    }
}
