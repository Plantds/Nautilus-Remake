using UnityEngine;

public class CS_PowerOffStart : MonoBehaviour
{
    public bool on = true;
    bool once = false;

    private void Update()
    {
        if (on)
            if (!once)
            {
                CS_ElectricalHazardMaster _ehm = FindAnyObjectByType<CS_ElectricalHazardMaster>();
                CS_SubmarineHazardChecker _shc = FindAnyObjectByType<CS_SubmarineHazardChecker>();

                if (_ehm != null && _shc != null)
                {
                    _shc.ElIsOff = true;
                    for (int i = 0; i < _ehm._electraicalObjects.Length; i++)
                    {
                        _ehm._electraicalObjects[i].TurnOff(false);
                    }
                }
                else
                {
                    if (_ehm == null)
                        Debug.LogError("Couldnt Find Object: CS_ElectricalHazardMaster");
                    if (_shc == null)
                        Debug.LogError("Couldnt Find Object: CS_SubmarineHazardChecker");
                }
                once = true;
            }
    }
}
