using UnityEngine;
using System.Collections;
using VLB;

public class CS_LightSorceSwitch : CS_ElctricalBaseScript
{
    [SerializeField] private float _onEmison = 3.11f;
    [SerializeField] private float _offEmison = 0.0f;
    [SerializeField] private string _veriableName = "_Emissive_Strength";
    [SerializeField] GameObject _lightTextureObj;
    [SerializeField] private float _onPower = 2500.0f;
    [SerializeField] private float _offPower = 0.0f;
    [Min(0.0f)]
    [SerializeField] private float _minFlickerTime = 0.4f;
    [Min(0.0f)]
    [SerializeField] private float _maxFlickerTime = 1.2f;

    private float _flickerTimer = 0.0f;
    private bool _isFlickering = false;
    private float _timeDelay = 0.0f;
    private bool _active = false;

    Light _L;
    private MeshRenderer _mr;

    private void Start()
    {
        _L = GetComponent<Light>();
        _mr = _lightTextureObj.GetComponent<MeshRenderer>();
    }

    public override void TurnOn(bool flash)
    {
        if (flash)
        {
            _flickerTimer = Random.Range(_minFlickerTime, _maxFlickerTime);
            _active = false;
        }
        else
        {
            _L.intensity = _onPower;
        }
    }

    public override void TurnOff(bool flash)
    {
        if (flash)
        {
            _active = true;
            _flickerTimer = Random.Range(_minFlickerTime, _maxFlickerTime);
        }
        else
        {
            _L.intensity = _offPower;
        }
    }

    private void Update()
    {
        _flickerTimer -= Time.deltaTime;
        if (_active)
        {
            if (_isFlickering == false && _flickerTimer > 0.0f)
            {
                StartCoroutine(FlickerOff());
            }
        }
        else
        {
            if (_isFlickering == false && _flickerTimer > 0.0f)
            {
                StartCoroutine(FlickerOn());
            }
        }
    }

    private IEnumerator FlickerOff()
    {
        _isFlickering = true;
        _L.intensity = _onPower;
        if(_mr != null)
            _mr.material.SetFloat(_veriableName, _onEmison);
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        if(_mr != null)
            _mr.material.SetFloat(_veriableName, _offEmison);
        _L.intensity = _offPower;
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _isFlickering = false;
    }
    private IEnumerator FlickerOn()
    {
        _isFlickering = true;
        _L.intensity = _offPower;
        if (_mr != null)
            _mr.material.SetFloat(_veriableName, _offEmison);
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        if (_mr != null) 
            _mr.material.SetFloat(_veriableName, _onEmison);
        _L.intensity = _onPower;
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _isFlickering = false;
    }
}
