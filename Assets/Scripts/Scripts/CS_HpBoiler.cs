using UnityEngine;

[ExecuteInEditMode]
public class CS_HpBoiler : MonoBehaviour
{
    [Header("moving solid block")]
    [SerializeField] private bool _usePosAsStartPos = false;
    [SerializeField] private Vector3 _startPos = Vector3.zero;
    [SerializeField] private Vector3 _endPos = Vector3.zero;

    //[Space]
    //[Header("update Shader")]
    //[SerializeField] private bool _isShader = false;
    //[Range(0.0f, 1.0f)]
    //[SerializeField] private float _minFillAmount = 0.0f;
    //[Range(0.0f, 1.0f)]
    //[SerializeField] private float _maxFillAmount = 1.0f;

    [Header("Universal")]
    [SerializeField] private float _lerpSpeed = 8.0f;

    SubmarineDamageSystem _damageSystem;
    //CS_Liquid _liquid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if (!_isShader)
        //{
            if (_usePosAsStartPos)
                _startPos = transform.localPosition;
            transform.localPosition = _startPos;
      //  }
      //  else
      //  {
      //      _liquid = GetComponent<CS_Liquid>();
      //      _liquid._fillAmount = _minFillAmount;
      //  }
      //
        _damageSystem = FindAnyObjectByType<SubmarineDamageSystem>();
        if (_damageSystem == null)
            Debug.LogError("SubmarineDamageSystem is null in SC_LiquidInBoilerToShowHp on object " + this.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        // if (!_isShader)
        // {
            // 0 - 1 | 0 = 0 | 1 = _damageSystem.startShipHp | to get where we are now just take curr hp / max hp 
            float currHpTranslated = _damageSystem.shipHp / _damageSystem.startShipHp;

            Vector3 targetPos = Vector3.Lerp(_startPos, _endPos, currHpTranslated);

            Vector3 smoothPos = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * _lerpSpeed);

            transform.localPosition = smoothPos;
        //}
        //else
        //{
        //    float currHpTranslated = _damageSystem.shipHp / _damageSystem.startShipHp;
        //
        //    float targetFill = Mathf.Lerp(_minFillAmount, _maxFillAmount, currHpTranslated);
        //
        //    float smoothFill = Mathf.Lerp(_liquid._fillAmount, targetFill, Time.deltaTime * _lerpSpeed);
        //
        //    _liquid._fillAmount = smoothFill;
        //}
    }
}
