using System.Collections;
using UnityEngine;

public class CS_LightSwitch : CS_ElctricalBaseScript
{
    [SerializeField] private float _onEmison = 3.11f;
    [SerializeField] private float _offEmison = 0.0f;
    [SerializeField] private string _veriableName = "_Emissive_Strength";
    [SerializeField] private bool _warningLight = false;
    [SerializeField] private Color warningColor;
    [Min(0.0f)]
    [SerializeField] private float _minFlickerTime = 0.4f;
    [Min(0.0f)]
    [SerializeField] private float _maxFlickerTime = 1.2f;
    [SerializeField] private CS_SubmarineHazardChecker _shc;

    private Color storedColor;
    private float _flickerTimer = 0.0f;
    private bool _isFlickering = false;
    private float _timeDelay = 0.0f;
    private MeshRenderer _mr;
    private bool _active = false;

    private void Start()
    {
        _mr = GetComponent<MeshRenderer>();
        if(!_shc)
            _shc = FindAnyObjectByType<CS_SubmarineHazardChecker>();
        storedColor = _mr.material.GetColor("_Emissive_Tint");
    }

    private void Update()
    {
        _flickerTimer -= Time.deltaTime;
        if (_active)
        {
            if (_isFlickering == false && _flickerTimer > 0.0f)
            {
                if (_warningLight)
                    _mr.material.SetColor("_Emissive_Tint", warningColor);
                StartCoroutine(FlickerOffLights());
            }
        }
        else
        {
            if(_isFlickering == false && _flickerTimer > 0.0f)
            {
                if(_warningLight)
                    _mr.material.SetColor("_Emissive_Tint", storedColor);
                StartCoroutine(FlickerOnLights());
            }
        }
        if (_shc.ElIsOff && _warningLight)
            StartCoroutine(FlickerOffLights());    
            
    }

    private IEnumerator FlickerOffLights()
    {
        _isFlickering = true;
        if(_warningLight)
            _mr.material.SetColor("_Emissive_Tint", warningColor);
        _mr.material.SetFloat(_veriableName, _onEmison);
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        if (_warningLight)
            _mr.material.SetColor("_Emissive_Tint", storedColor);
        _mr.material.SetFloat(_veriableName, _offEmison);
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _isFlickering = false;
        if (_warningLight)
        {
            _mr.material.SetColor("_Emissive_Tint", warningColor);
            _mr.material.SetFloat(_veriableName, _onEmison);
        }
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


    public override void TurnOff(bool flash)
    {
        if (flash)
        {
            _active = true;
            _flickerTimer = Random.Range(_minFlickerTime, _maxFlickerTime);
        }else
        {
            _mr.material.SetFloat(_veriableName, _offEmison);
        }
    }
    public override void TurnOn(bool flash)
    {
        if (flash)
        {
            _flickerTimer = Random.Range(_minFlickerTime, _maxFlickerTime);
            _active = false;
        }else
        {
            _mr.material.SetFloat(_veriableName, _onEmison);
        }
    }
}
