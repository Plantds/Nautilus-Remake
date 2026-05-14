using UnityEngine;
using UnityEngine.VFX;

public class SubmarineDamageHole : RepairableObject
{
    [SerializeField] SubmarineDamageSystem submarineDamageSystem;
    [SerializeField] FMODUnity.StudioEventEmitter audioSource;
    [SerializeField] VisualEffect waterVfx;

    [HideInInspector]
    public float tickingDamage = 0.01f;
    MeshRenderer meshRenderer;

    void Start()
    {
        //No hole
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        waterVfx.enabled = false;
    }
    void Update()
    {
        RepairUpdate();
        if (broken)
        {
            submarineDamageSystem.shipHp -= tickingDamage;
            submarineDamageSystem.currentHoleDmg -= tickingDamage;
        }
    }

    public override void OnRepared()
    {
        base.OnRepared();
        //Close hole
        submarineDamageSystem.damageHoleSpots.Add(this);
        meshRenderer.enabled = false;
        waterVfx.enabled = false;
        submarineDamageSystem.DeactivateDamageHole(this);

        submarineDamageSystem.shipHp += submarineDamageSystem.holeEveryXDmg;
        //Add or set to "holeEveryXDmg"
        submarineDamageSystem.currentHoleDmg = submarineDamageSystem.holeEveryXDmg;

        if (audioSource)
        {
            audioSource.Stop();
        }
    }
    public override void Break()
    {
        base.Break();
        //New hole
        meshRenderer.enabled = true;
        waterVfx.enabled = true;

        if (audioSource)
        {
            audioSource.Play();
        }
    }
}
