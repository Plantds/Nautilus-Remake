using UnityEngine;
using UnityEngine.VFX;

public class CS_ElectricSparkSwitch : CS_ElctricalBaseScript
{
    private VisualEffect effect;

    private void Start()
    {
        effect = GetComponent<VisualEffect>();
    }

    public override void TurnOn(bool flash)
    {
        effect.enabled = true;
    }

    public override void TurnOff(bool flash)  
    {
        effect.enabled = false;
    }
}
