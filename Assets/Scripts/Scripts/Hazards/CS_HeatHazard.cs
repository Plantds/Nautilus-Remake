using FMODUnity;
using UnityEngine;

public class CS_HeatHazard : MonoBehaviour
{
    [SerializeField] private CS_SubmarineHazardChecker _shc;
    [SerializeField] private CS_HeatHazardMaster _hhm;
    [SerializeField] private FMODUnity.EventReference powerTurnOffClip;

    private bool _in = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SubmarineCollider" && !_in)
        {
            _in = true;
            Debug.Log("sub is in " + gameObject.name);
            _shc.Heat = true;
            RuntimeManager.PlayOneShot(powerTurnOffClip);
            _hhm.takeDmg = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SubmarineCollider" && _in)
        {
            _hhm.takeDmg = false;
            _shc.Heat = false;
            _in = false;
        }
    }
}
