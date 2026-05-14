using System.Collections.Generic;
using ArtNotes.PhysicalInteraction;
using UnityEngine;

public class SubmarineDamageSystem : MonoBehaviour
{
    public float shipHp = 200;
    [HideInInspector] public float startShipHp;
    public float alarmProcentage = 20;
    public float holeEveryXDmg = 20;
    public float holeTickingDmg = 0.001f;
    [SerializeField] Interactor interactor;
    SC_SubmarineMovement sub;
    public List<SubmarineDamageHole> damageHoleSpots;

    public bool ShowDebugValues = false;  

    [Header("Damage function, see milanote Instruments/Hull")]

    public float a = 1.8f;
    public float b = 10;
    public float maxDmg = 30;

    [HideInInspector]
    public float currentHoleDmg;
    bool alarmOn;
    bool looseConditionTriggerd;
    ReloadLoadSave loadSaveSystem;

    void Start()
    {
        sub = FindAnyObjectByType<SC_SubmarineMovement>();

        loadSaveSystem = FindAnyObjectByType<ReloadLoadSave>();

        startShipHp = shipHp;
        
        looseConditionTriggerd = false;
        currentHoleDmg = holeEveryXDmg;
        foreach (var damageHole in damageHoleSpots)
        {
            damageHole.GetComponent<Collider>().enabled = false;
        } 
    }

    void Update()
    {
        if (ShowDebugValues)
        {
            Debug.Log("TICK " + "Hp: " + shipHp + " CurDmg: " + currentHoleDmg);
        }
        while (currentHoleDmg <= 0)
        {
            currentHoleDmg += holeEveryXDmg;

            int rand = Random.Range(0, damageHoleSpots.Count);

            if (damageHoleSpots.Count > 0)
            {
                damageHoleSpots[rand].GetComponent<Collider>().enabled = true;
                damageHoleSpots[rand].Break();
                damageHoleSpots[rand].tickingDamage = holeTickingDmg;
                damageHoleSpots.RemoveAt(rand);
            }
        }
        if (shipHp <= alarmProcentage * shipHp / 100 && !alarmOn)
        {
            //Turn on alarm logic
            alarmOn = true;
        }
        else if (shipHp > alarmProcentage * shipHp / 100 && alarmOn)
        {
            //Turn off alarm logic
            alarmOn = false;
        }
        
        if (shipHp <= 0 && !looseConditionTriggerd)
        {
            //Loose condition
            loadSaveSystem.Death(ReloadLoadSave.deathType.submarine);
            looseConditionTriggerd = true;
        }
    }

    public void OnHit(float _magnitude)
    {
        if (!sub.isParking)
        {
            float damage = (Mathf.Pow(a, _magnitude) - 1) / b;
            damage = Mathf.Clamp(damage, 0, maxDmg);

            shipHp -= damage;
            currentHoleDmg -= damage;

            if (ShowDebugValues)
            {
                Debug.Log("HIT " + "Hp: " + shipHp + " CurDmg: " + currentHoleDmg + " Dmg: " + damage);
            }
        }   
    }
    
    public void DeactivateDamageHole(SubmarineDamageHole _submarineDamageHole)
    {
        _submarineDamageHole.GetComponent<Collider>().enabled = false;
        interactor.endInteraction();
    }
}
