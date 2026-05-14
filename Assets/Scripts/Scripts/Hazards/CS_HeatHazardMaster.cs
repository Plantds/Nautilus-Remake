using UnityEngine;

public class CS_HeatHazardMaster : MonoBehaviour
{
    [SerializeField] private SubmarineDamageSystem sds;
    [SerializeField] private float damgMulti = 10.0f;
    [HideInInspector] public bool takeDmg = false;

    private void Start()
    {
        if(sds  == null)
            sds = FindAnyObjectByType<SubmarineDamageSystem>();
    }

    private void Update()
    {
        if (takeDmg) {
            sds.shipHp -= Time.deltaTime * damgMulti;
        }
    }
}
