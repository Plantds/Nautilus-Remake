using ArtNotes.PhysicalInteraction;
using System.Collections;
using UnityEngine;

public class ScoutButton : Button
{
    [SerializeField] EmissionAndLightLerp lampEmissionOn;
    [SerializeField] EmissionAndLightLerp lampEmissionOff;
    [SerializeField] CS_SubmarineHazardChecker _shc;
    [SerializeField] Color canShoot;

    //flicker
    [SerializeField] private float _onEmison = 3.11f;
    [SerializeField] private float _offEmison = 0.0f;
    [SerializeField] private string _veriableName = "_Emissive_Strength";
    [Min(0.0f)]
    [SerializeField] private float _minFlickerTime = 0.4f;
    [Min(0.0f)]
    [SerializeField] private float _maxFlickerTime = 1.2f;

    ScoutShooter scout;
    bool isOn = false;

    // flicker
    private float _flickerTimer = 0.0f;
    private bool _isFlickering = false;
    private float _timeDelay = 0.0f;
    private MeshRenderer _mr;
    private bool _active = false;   

    void OnValidate()
    {
        lampEmissionOn.emissiveMaterials[0].SetColor("_Emissive_Tint", canShoot);
    }

    public void Start()
    {
        base.Start();
        _mr = GetComponent<MeshRenderer>();
        _mr.material.SetFloat(_veriableName, _onEmison);
        _shc = FindAnyObjectByType<CS_SubmarineHazardChecker>();
        scout = FindAnyObjectByType<ScoutShooter>();
    }
    void Update()
    {
        if (!_shc.ElIsOff)
        {
            if (!scout.isOnCooldown)
            {
                turnOn();
            }
            else
            {
                turnOff();
            }
        }
    }
    void turnOn()
    {
        if (!isOn)
        {
            _mr.material.SetFloat(_veriableName, _onEmison);
            isOn = true;
        }
    }
    void turnOff()
    {
        if (isOn)
        {
            _mr.material.SetFloat(_veriableName, _offEmison);
            isOn = false;
        }
        
    }
    public override void InteractStart(RaycastHit hit)
    {
        if (!scout.isOnCooldown && !_shc.ElIsOff)
        {
            base.InteractStart(hit);
        }

        if (scout && !_shc.ElIsOff)
        {
            scout.TryFire();
        }
    }
    private IEnumerator FlickerOffLights()
    {
        _isFlickering = true;
        _mr.material.SetFloat(_veriableName, _onEmison);
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _mr.material.SetFloat(_veriableName, _offEmison);
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _isFlickering = false;
    }
    private IEnumerator FlickerOnLights()
    {
        _isFlickering = true;
        _mr.material.SetFloat(_veriableName, _offEmison);
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _mr.material.SetFloat(_veriableName, _onEmison);
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _isFlickering = false;
    }
}
