using System;
using ArtNotes.PhysicalInteraction;
using KinematicCharacterController;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class CharacterCameraComponent : MonoBehaviour
{
    [SerializeField] public SCOB_CameraSettings settings;

    public Transform Transform { get; private set; }
    public Transform FollowTransform { get; private set; }

    // Used to slow down camera speed when interacting (NOT IMPLEMENTED YET)
    [SerializeField] Interactor _interactor;

    public Vector3 PlanarDirection { get; set; }

    public bool isOnSubmarine = false;
    public PhysicsMover submarine;

    private float _currentDistance;
    private float _targetVerticalAngle;

    private Vector3 _currentFollowPosition;

    Quaternion ang = Quaternion.identity;
    Quaternion offset = Quaternion.identity;

    bool checkOffset = true;

    [SerializeField] private SC_HelmetOffset helmetOffset;
    [SerializeField] private FMODUnity.StudioEventEmitter emitter;
    [SerializeField] private VisualEffect dust;

    [Header("Global Bobbing Settings")]
    [SerializeField, Range(0.0f, 5.0f)] private float _startAndEndSmoothing = 3.0f;

    [Header("Camera Bobbing Settings")]
    [SerializeField] private bool _enableCameraBobbing = true;
    [SerializeField, Range(0.0f, 10.0f)] private float _frequencyCamera = 10.0f;
    [SerializeField, Range(0.0f, 4.0f)] private float _steepCamera = 3.0f;

    [SerializeField, Range(0.0f, 0.25f)] private float _verticalAmplitudeCamera = 0.015f;
    [SerializeField, Range(0.0f, 0.25f)] private float _horizontalAmplitudeCamera = 0.015f;
    [SerializeField, Range(0.0f, 10.0f)] private float _rotationalAmplitudeCamera = 1.0f;

    [Header("Helmet Bobbing Settings")]
    [SerializeField] private bool _enableHelmetBobbing = true;
    [SerializeField, Range(0.0f, 10.0f)] private float _frequencyHelmet = 10.0f;
    [SerializeField, Range(0.0f, 4.0f)] private float _steepHelmet = 3.0f;

    [SerializeField, Range(0.0f, 0.25f)] private float _verticalAmplitudeHelmet = 0.015f;
    [SerializeField, Range(0.0f, 0.25f)] private float _horizontalAmplitudeHelmet = 0.015f;
    [SerializeField, Range(0.0f, 10.0f)] private float _rotationalAmplitudeHelmet = 1.0f;

    [SerializeField] private bool _flipSteepness = false;

    [NonSerialized] private Vector3 cameraBobbingPositionOffset = Vector3.zero;
    [NonSerialized] private Vector3 helmetBobbingPositionOffset = Vector3.zero;

    [NonSerialized] private Quaternion cameraBobbingRotationOffset = Quaternion.identity;
    [NonSerialized] private Quaternion helmetBobbingRotationOffset = Quaternion.identity;

    Vector3 startPos;
    Vector3 shake = Vector3.zero;

    InputSystem_Actions inputActions;

    [SerializeField] private bool _cameraShakeEnabled = false;
    [SerializeField] private float _cameraShakeIntensity = 0.1f;
    [SerializeField] private bool _cameraShakeTest = false;
    float _cameraShakeTime = 0.0f;
    float _cameraShakeDuration = 0.0f;
    float funccameraShakeIntensity = 0.0f;
    bool _cameraShakeUseDuration = false;
    bool _cameraShakeUseIntensity = false;


    float bobbingTime = 0.0f;

    bool triggerSound = false;


    void OnValidate()
    {
        settings.DefaultDistance = Mathf.Clamp(settings.DefaultDistance, settings.MinDistance, settings.MaxDistance);
        settings.DefaultVerticalAngle = Mathf.Clamp(settings.DefaultVerticalAngle, settings.MinVerticalAngle, settings.MaxVerticalAngle);
    }

    void Awake()
    {
        Transform = this.transform;

        _currentDistance = settings.DefaultDistance;

        _targetVerticalAngle = 0f;

        PlanarDirection = Vector3.forward;

        startPos = this.transform.position;

        inputActions = new InputSystem_Actions();

        inputActions.Enable();
    }

    // Set the transform that the camera will orbit around
    public void SetFollowTransform(Transform t)
    {
        FollowTransform = t;
        PlanarDirection = FollowTransform.forward;
        _currentFollowPosition = FollowTransform.position;
    }

    public void UpdateWithInput(float deltaTime, Vector3 rotationInput)
    {
        if (FollowTransform)
        {
            if (settings.InvertX)
            {
                rotationInput.x *= -1f;
            }
            if (settings.InvertY)
            {
                rotationInput.y *= -1f;
            }

            if (isOnSubmarine)
            {
                if (GameManager.GetInstance().GetPlayerState() == PlayerState.INSIDE_SUBMARINE)
                {
                    if (checkOffset)
                    {
                        offset = Quaternion.Euler(new Vector3(0.0f, submarine.transform.rotation.eulerAngles.y, 0.0f));
                        checkOffset = false;
                    }

                    ang = Quaternion.Euler(new Vector3(0.0f, submarine.transform.rotation.eulerAngles.y, 0.0f)) * Quaternion.Inverse(offset);
                }
                
            }
            else
            {
                checkOffset = true;
            }

            // Process rotation input

            // Add this to the rotation input
            //_interactor.LookSpeedMultiply

            float interactorLookSpeed = 1.0f;

            if (_interactor)
            {
                if (_interactor.LookSpeedMultiply > 0.0f)
                {
                    interactorLookSpeed = _interactor.LookSpeedMultiply;
                }
            }

            if (inputActions.Player.KeyboardMove.ReadValue<Vector2>().SqrMagnitude() > 0.0f)
            {
                if (_enableCameraBobbing || _enableHelmetBobbing)
                {
                    bobbingTime += deltaTime;
                }
                if (_enableCameraBobbing)
                {
                    UpdateCameraBobbing();
                }
                if (_enableHelmetBobbing)
                {
                    UpdateHelmetBobbing();
                }
            }
            else
            {
                bobbingTime = 0.0f;
                cameraBobbingPositionOffset = CustomMath.Lerp.ApproxLerp(cameraBobbingPositionOffset, Vector3.zero, _startAndEndSmoothing, deltaTime);
                helmetBobbingPositionOffset = CustomMath.Lerp.ApproxLerp(helmetBobbingPositionOffset, Vector3.zero, _startAndEndSmoothing, deltaTime);

                cameraBobbingRotationOffset = CustomMath.Lerp.ApproxSlerp(cameraBobbingRotationOffset, Quaternion.identity, _startAndEndSmoothing, deltaTime);
                helmetBobbingRotationOffset = CustomMath.Lerp.ApproxSlerp(helmetBobbingRotationOffset, Quaternion.identity, _startAndEndSmoothing, deltaTime);
            }



            Quaternion rotationFromInput = Quaternion.Euler(FollowTransform.up * (rotationInput.x * settings.RotationSpeed * interactorLookSpeed));

            PlanarDirection = rotationFromInput * PlanarDirection;
            PlanarDirection = Vector3.Cross(FollowTransform.up, Vector3.Cross(PlanarDirection, FollowTransform.up));
            Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowTransform.up);

            _targetVerticalAngle -= (rotationInput.y * settings.RotationSpeed * interactorLookSpeed);
            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, settings.MinVerticalAngle, settings.MaxVerticalAngle);
            Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
            Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-settings.RotationSharpness * deltaTime));

            // Apply rotation
            Transform.rotation = ang * targetRotation * CustomMath.Lerp.ApproxSlerp(cameraBobbingRotationOffset, Quaternion.identity, 10.0f, deltaTime);

            // Find the smoothed follow position
            _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-settings.FollowingSharpness * deltaTime));

            // Find the smoothed camera orbit position
            Vector3 targetPosition = _currentFollowPosition - ((targetRotation * Vector3.forward) * _currentDistance);

            // Handle framing
            targetPosition += Transform.right * settings.FollowPointFraming.x;
            targetPosition += Transform.up * settings.FollowPointFraming.y;

            if (Mathf.Sin(bobbingTime * _frequencyHelmet) >= 0.98f || Mathf.Sin(bobbingTime * _frequencyHelmet) <= -0.98f && GameManager.GetInstance().GetPlayerState() == PlayerState.OUTSIDE_SUBMARINE)
            {
                triggerSound = true;
            }

            if (triggerSound)
            {
                triggerSound = false;
                emitter.Play();
                dust.Play();
            }

            if (_cameraShakeTest)
            {
                _cameraShakeTest = false;
                SetCameraShakeDurationAndIntensity(true, true, 1.0f, 0.01f);
            }

            shake = Vector3.zero;
            if (_cameraShakeEnabled)
            {
                float shakeINtensity = _cameraShakeIntensity;
                if (_cameraShakeUseDuration)
                {
                    _cameraShakeTime += deltaTime;

                    if (_cameraShakeTime >= _cameraShakeDuration)
                    {
                        _cameraShakeEnabled = false;
                        _cameraShakeUseDuration = false;
                        _cameraShakeTime = 0.0f;
                    }

                }

                if (_cameraShakeUseIntensity)
                {
                    shakeINtensity = funccameraShakeIntensity;
                }

                shake = UnityEngine.Random.insideUnitSphere * shakeINtensity;
            }
            else
            {
                _cameraShakeUseIntensity = false;
                _cameraShakeTime = 0.0f;
            }

            // Apply camera position
            Transform.position = targetPosition + cameraBobbingPositionOffset + shake;

            // Apply helmet position & rotation
            helmetOffset.ManualUpdate(targetPosition + helmetBobbingPositionOffset + shake * 0.8f, Transform.rotation * helmetBobbingRotationOffset);
        }
    }

    private void UpdateCameraBobbing()
    {
        float time = bobbingTime;
        Vector3 bobbing = new Vector3();
        bobbing.x = CalculateHorizontalBobbing(time, _frequencyCamera, _horizontalAmplitudeCamera, _steepCamera);
        bobbing.y = CalculateVerticalBobbing(time, _frequencyCamera, _verticalAmplitudeCamera, _steepCamera);

        cameraBobbingPositionOffset = CustomMath.Lerp.ApproxLerp(cameraBobbingPositionOffset, bobbing, _startAndEndSmoothing, Time.deltaTime);

        cameraBobbingRotationOffset = CalculateRotationalBobbing(time, _frequencyCamera, _rotationalAmplitudeCamera);
    }

    private void UpdateHelmetBobbing()
    {
        float time = bobbingTime;
        Vector3 bobbing = new Vector3();
        bobbing.x = CalculateHorizontalBobbing(time, _frequencyHelmet, _horizontalAmplitudeHelmet, _steepHelmet);
        bobbing.y = CalculateVerticalBobbing(time, _frequencyHelmet, _verticalAmplitudeHelmet, _steepHelmet);

        helmetBobbingPositionOffset = CustomMath.Lerp.ApproxLerp(helmetBobbingPositionOffset, bobbing, _startAndEndSmoothing, Time.deltaTime);


        helmetBobbingRotationOffset = CalculateRotationalBobbing(time, _frequencyHelmet, _rotationalAmplitudeHelmet);
    }

    private float CalculateVerticalBobbing(float _time, float _frequency, float _amplitude, float _steep)
    {
        float sample = Mathf.Pow(Mathf.Sin(_time * _frequency), 2.0f * _steep);
        sample *= _flipSteepness ? -2.0f : 2.0f;
        sample += _flipSteepness ? 1.0f : -1.0f;
        return sample * _amplitude;
    }

    private float CalculateHorizontalBobbing(float _time, float _frequency, float _amplitude, float _steep)
    {
        float sample = Mathf.Pow(Mathf.Cos(_time * _frequency / 2.0f), 2.0f * _steep);
        sample *= _flipSteepness ? -2.0f : 2.0f;
        sample += _flipSteepness ? 1.0f : -1.0f;
        return sample * _amplitude;
    }

    private Quaternion CalculateRotationalBobbing(float _time, float _frequency, float _amplitude)
    {
        float sample = Mathf.Cos(Time.time * _frequency - 0.4f * Mathf.Pow(Mathf.Cos(_time * _frequency), 2.0f));
        return Quaternion.Euler(new Vector3(0.0f, 0.0f, sample * _amplitude));
    }

    float Shake()
    {
        return Mathf.PerlinNoise(Time.time / 1.0f, 0f);
    }

    public void SetCameraShake(bool _enable, bool _useDuration = false, float _duration = 0.0f)
    {
        _cameraShakeEnabled = _enable;
        _cameraShakeUseDuration = _useDuration;
        _cameraShakeDuration = _duration;
    }

    public void SetCameraShakeDurationAndIntensity(bool _enable, bool _useDuration, float _duration, float _inputCameraShakeIntensity)
    {
        _cameraShakeEnabled = _enable;
        _cameraShakeUseDuration = _useDuration;
        _cameraShakeDuration = _duration;
        _cameraShakeUseIntensity = _enable;
        funccameraShakeIntensity = _inputCameraShakeIntensity;
    }
}