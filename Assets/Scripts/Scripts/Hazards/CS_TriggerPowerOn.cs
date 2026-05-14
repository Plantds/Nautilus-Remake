using Unity.VisualScripting;
using UnityEngine;

public class CS_TriggerPowerOn : MonoBehaviour
{
    [SerializeField] private CS_ElectricalHazardMaster _ehm;
    [SerializeField] private CS_SubmarineHazardChecker _shc;
    private bool once = false;

    private void Start()
    {
        if (!_ehm)
            _ehm = FindAnyObjectByType<CS_ElectricalHazardMaster>();
        if (!_shc)
            _shc = FindAnyObjectByType<CS_SubmarineHazardChecker>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!once)
        {
            if (other.gameObject.tag == "Player")
            {
                _shc.ElIsOff = false;
                for (int i = 0; i < _ehm._electraicalObjects.Length; i++)
                {
                    _ehm._electraicalObjects[i].TurnOn();
                }
                once = true;
            }
        }
    }
}
