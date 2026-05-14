using System.Collections.Generic;
using UnityEngine;

namespace ArtNotes.PhysicalInteraction
{
    [RequireComponent(typeof(HingeJoint))]
    public class CustomValve : InteractableObject
    {
        [SerializeField] int _fullRotations, _force;
        [SerializeField] bool _startFromFull = false;

        [SerializeField] bool _freezeWhenRelesed;
        [SerializeField] List<GameObject> _activeTriggers;

        [Header("Deadzone:")]
        [SerializeField] float _deadZone;

        [SerializeField] float _returnSpeed = 5;


        [Header("One use:")]

        [SerializeField] bool _oneUse;

        [SerializeField] float _maxLimitEndFraction = 0.5f;


        bool _interact = false;
        [HideInInspector]
        public float _currentAngle;
        float _targetDistance, _lastEulerZ, _lastEulerX, _lastEulerY;
        Transform _camTransform;
        Rigidbody _rb;
        HingeJoint _joint;

        float _currentRot => _currentAngle / 360;
        Vector3 _targetPos => _camTransform.position + _camTransform.forward * _targetDistance;
        Quaternion _startRot;

        float _lerpBackTimer;
        float _returnLenght;
        bool _lerpingBack;
        Quaternion _lerpStartRot;
        bool _doLerpOnce;
        bool _enabled = true;
        JointLimits jointLimits;
        //SC_ActivationManger _am;

        private void Start()
        {
            _camTransform = Camera.main.transform;
            _rb = GetComponent<Rigidbody>();
            _joint = GetComponent<HingeJoint>();
            jointLimits = _joint.limits;
            _startRot = _rb.transform.localRotation;

            if (_startFromFull)
            {
                _currentAngle = _fullRotations * 360;
                if (_mainExecutor != null) _mainExecutor.Execute(_currentRot);
            }
        }


        //Too sensitive
        private void FixedUpdate()
        {
            if (_enabled)
            {
                if (_oneUse)
                {
                    jointLimits.min = _currentAngle;
                    _joint.limits = jointLimits;
                    //Only works if max limit is the one you use
                    if (Mathf.Abs(_currentAngle) >= _joint.limits.max - _maxLimitEndFraction)
                    {
                        _rb.freezeRotation = true;
                        _enabled = false;
                        // if(_activeTriggers.Count > 0)
                        //     _am.ActiveTheseGameObjects(_activeTriggers);
                    }
                }

                if (_interact)
                {
                    Vector3 force = (_targetPos - HitPos).normalized * _force;
                    _rb.AddForceAtPosition(force, HitPos, ForceMode.Force);
                    _lerpBackTimer = 0;
                    _lerpingBack = false;
                    _doLerpOnce = true;
                }
                else
                {
                    if (_freezeWhenRelesed && !_lerpingBack)
                    {
                        _rb.transform.localEulerAngles = new Vector3(_lastEulerX, _lastEulerY, _lastEulerZ);
                    }
                }

                if (_joint.useLimits && _currentRot < _fullRotations - .1f && _currentRot > .1f)
                    _joint.useLimits = false;
                else if (!_joint.useLimits && (_currentRot > _fullRotations - .1f || _currentRot < .1f))
                    _joint.useLimits = true;

                float euler = transform.localEulerAngles.y;

                //custom
                float eulerX = transform.localEulerAngles.x;
                float eulerZ = transform.localEulerAngles.z;
                //

                float deltaRot = euler - _lastEulerY; // up 40 -> 50 ; -140 -> -130
                if (Mathf.Abs(deltaRot) < 300 && deltaRot != 0)
                {
                    _currentAngle += deltaRot;

                    //custom
                    if (_mainExecutor != null)
                    {
                        if (Mathf.Abs(_currentAngle) > _deadZone && !_lerpingBack) _mainExecutor.Execute(_currentRot);
                        else _mainExecutor.Execute(0.0f);
                    }
                    //

                }

                //custom
                if (Mathf.Abs(_currentAngle) <= _deadZone && !_interact && !_lerpingBack && _doLerpOnce)
                {
                    _lerpingBack = true;
                    _doLerpOnce = false;
                    _lerpStartRot = _rb.transform.localRotation;
                    _returnLenght = Mathf.Abs(_currentAngle);

                }
                if (_lerpingBack)
                {
                    _lerpBackTimer += Time.deltaTime * _returnSpeed / _returnLenght;
                    if (_lerpBackTimer >= 1)
                    {
                        _lerpBackTimer = 1;
                    }
                    _rb.transform.localRotation = Quaternion.Lerp(_lerpStartRot, _startRot, _lerpBackTimer);

                    if (_lerpBackTimer == 1)
                    {
                        _rb.transform.localRotation = _startRot;
                        _currentAngle = 0;
                        euler = transform.localEulerAngles.y;
                        eulerX = transform.localEulerAngles.x;
                        eulerZ = transform.localEulerAngles.z;
                        _lerpingBack = false;
                    }
                }

                _lastEulerY = euler;
                _lastEulerX = eulerX;
                _lastEulerZ = eulerZ;
                //
            }

        }

        public override void InteractStart(RaycastHit hit)
        {
            base.InteractStart(hit);
            _targetDistance = hit.distance;
            _interact = true;
        }
        public override void InteractEnd()
        {
            base.InteractEnd();
            _interact = false;
        }
    }
}
