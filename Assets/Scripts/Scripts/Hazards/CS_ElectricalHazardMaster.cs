using System.Linq;
using UnityEngine;

public class CS_ElectricalHazardMaster : MonoBehaviour
{
    [HideInInspector] public CS_ElctricalBaseScript[] _electraicalObjects;
    [SerializeField] private CS_ElctricalBaseScript[] _ManualAddObjects;
    private void OnValidate()
    {
        _electraicalObjects = FindObjectsByType<CS_ElctricalBaseScript>(FindObjectsSortMode.None);

        foreach(CS_ElctricalBaseScript e in _ManualAddObjects)
        {
            _electraicalObjects.Append(e);
            Debug.Log("adding: " + e.name + " to ElectracalObjects");
        }

        Debug.Log("ElectracalObjects: " + _electraicalObjects.Length);
    }
}
