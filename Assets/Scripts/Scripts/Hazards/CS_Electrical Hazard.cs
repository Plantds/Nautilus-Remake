using FMODUnity;
using UnityEngine;

public class CS_ElectricalHazard : MonoBehaviour
{
    [SerializeField] private CS_ElectricalHazardMaster _ehm;
    [SerializeField] private CS_SubmarineHazardChecker _shc;
    [SerializeField] private FMODUnity.EventReference enteredHeatClip;

    private bool _in = false;
    private void Start()
    {
        if (!_ehm)
            _ehm = FindAnyObjectByType<CS_ElectricalHazardMaster>();
        if (!_shc)
            _shc = FindAnyObjectByType<CS_SubmarineHazardChecker>();
    }

    private void Update()
    {
        if(_in)
        for (int i = 0; i < _ehm._electraicalObjects.Length; i++)
        {
            _ehm._electraicalObjects[i].TurnOff(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SubmarineCollider" && !_in)
        {
            _in = true;
            Debug.Log("sub is in " + gameObject.name);
            _shc.Electric = true;
            _shc.ElIsOff = true;
            RuntimeManager.PlayOneShot(enteredHeatClip);
            //for (int i = 0; i < _ehm._electraicalObjects.Length; i++)
            //{
            //        _ehm._electraicalObjects[i].TurnOff(true);
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SubmarineCollider" && _in)
        {
            _shc.Electric = false;
            _in = false;
        }
    }
}
