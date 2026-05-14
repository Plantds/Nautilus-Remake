using UnityEngine;

public class CS_ChangeLightColor : MonoBehaviour
{
    [SerializeField] private CS_SubmarineHazardChecker _shc;
    [SerializeField] private Color warningColor;
    private Color storedColor;

    Renderer _r;
    private void Start()
    {
        _r = GetComponent<Renderer>();
        storedColor = _r.material.GetColor("_Emissive_Tint");
    }

    private void Update()
    {
        if(_shc.ElIsOff)
        {
            _r.material.SetColor("_Emissive_Tint", warningColor);
        }
        else
        {
            _r.material.SetColor("_Emissive_Tint", storedColor);
        }
    }
}
