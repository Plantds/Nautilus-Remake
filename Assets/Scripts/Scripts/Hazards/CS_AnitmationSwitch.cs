using System.Collections;
using UnityEngine;

public class CS_AnitmationSwitch : CS_ElctricalBaseScript
{
    [SerializeField] private bool isGenarator = false;
    [Min(0.0f)]
    [SerializeField] private float _minFlickerTime = 0.4f;
    [Min(0.0f)]
    [SerializeField] private float _maxFlickerTime = 1.2f;

    private Animation an;
    private EmissiveBulbFlicker ebf;
    private bool _active = false;
    private float _flickerTimer = 0.0f;
    private bool _isFlickering = false;
    private float _timeDelay = 0.0f;

    private void Start()
    {
        an = GetComponent<Animation>();
        if (isGenarator)
        {
            ebf = GetComponent<EmissiveBulbFlicker>();
        }
    }

    private void Update()
    {
        _flickerTimer -= Time.deltaTime;
        if (_active)
        {
            if (_isFlickering == false && _flickerTimer > 0.0f)
            {
                StartCoroutine(FlickerOffLights());
            }
        }
        else
        {
            if (_isFlickering == false && _flickerTimer > 0.0f)
            {
                StartCoroutine(FlickerOnLights());
            }
        }
    }

    private IEnumerator FlickerOffLights()
    {
        _isFlickering = true;
        an.Play();
        if (isGenarator)
            ebf.Play();
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        an.Stop();
        if (isGenarator)
            ebf.Stop();
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _isFlickering = false;
    }
    private IEnumerator FlickerOnLights()
    {
        _isFlickering = true;
        an.Stop();
        if (isGenarator)
            ebf.Stop();
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        an.Play();
        if (isGenarator)
            ebf.Play();
        _timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(_timeDelay);
        _isFlickering = false;
    }

    public override void TurnOff(bool flash)
    {
        _active = true;
        _flickerTimer = Random.Range(_minFlickerTime, _maxFlickerTime);
    }
    public override void TurnOn(bool flash)
    {
        _flickerTimer = Random.Range(_minFlickerTime, _maxFlickerTime);
        _active = false;
    }
}
