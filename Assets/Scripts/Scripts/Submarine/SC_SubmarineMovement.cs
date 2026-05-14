using System;
using System.Linq;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SubmarineState
{
    MANUAL = 0,
    AUTOMATIC,
}

public enum AutoParkState
{
    COMPLETE = 0,
    AUTOMATIC,
}

public class SC_SubmarineMovement : MonoBehaviour, IMoverController
{
    [SerializeField] public PhysicsMover mover;
    [SerializeField] public GameObject center;

    [Header("Movement Settings")]
    [SerializeField] public float forwardMaxSpeed = 10.0f;
    [SerializeField] private float turnMaxSpeed = 10.0f;
    [SerializeField] private float upMaxSpeed = 10.0f;

    [SerializeField] private float forwardVelocityResponseSpeed = 0.5f;
    [SerializeField] private float upVelocityResponseSpeed = 0.5f;
    [SerializeField] private float turnVelocityResponseSpeed = 0.5f;

    [SerializeField] public float stabilizeResponseSpeed = 0.5f;
    [SerializeField] private float stabilizeForceMultiplier = 10.0f;

    [SerializeField] public float forwardInput;
    [SerializeField] public float turnInput;
    [SerializeField] public float upInput;

    [Header("Collision")]
    [SerializeField] private float collisionMaxLinearVelocity = 20.0f;
    [SerializeField] private float collisionMaxAngularVelocity = 20.0f;

    [SerializeField] private float collisionExageration = 2.0f;

    [Header("Parking Settings")]
    [NonSerialized] private AnimationCurve parkingSpeedPosition;
    [NonSerialized] private AnimationCurve parkingSpeedRotation;

    [SerializeField] private AnimationCurve forceParkingSpeedPosition;
    [SerializeField] private AnimationCurve forceParkingSpeedRotation;
    [SerializeField] private AnimationCurve lastParkingSpeedPosition;
    [SerializeField] private AnimationCurve lastParkingSpeedRotation;
    [SerializeField] private AnimationCurve manualParkingSpeedPosition;
    [SerializeField] private AnimationCurve manualParkingSpeedRotation;

    private Vector3 currentStabilizeTorque = Vector3.zero;

    [SerializeField] private FMODUnity.StudioEventEmitter engineSoundEmitter;
    [SerializeField] private FMODUnity.StudioGlobalParameterTrigger engineSoundParameter;
    [SerializeField] private FMODUnity.StudioEventEmitter parkedEmitter;


    [NonSerialized] public SubmarineState state = SubmarineState.MANUAL;
    private Vector3 input;
    [NonSerialized] public bool lastPark = false;
    [NonSerialized] public bool subTeleport = false;


    /// <summary>
    /// The current linear velocity
    /// </summary>
    [ReadOnly]
    public Vector3 LinearVelocity { get; private set; }
    /// <summary>
    /// The current angular velocity
    /// </summary>
    [ReadOnly]
    public Vector3 AngularVelocity { get; private set; }
    /// <summary>
    /// The target position for the physics mover
    /// </summary>
    [ReadOnly]
    public Vector3 TransientPosition { get; private set; }
    /// <summary>
    /// The target rotation for the physics mover
    /// </summary>
    [ReadOnly]
    public Quaternion TransientRotation { get; private set; }
    /// <summary>
    /// The current physics mover transform
    /// </summary>
    [ReadOnly]
    public Transform Transform { get { return mover.Transform; } }

    [NonSerialized] public Vector3 startPosition;
    [NonSerialized] public Vector3 targetPosition;

    [NonSerialized] public Quaternion startRotation;
    [NonSerialized] public Quaternion targetRotation;

    [NonSerialized] public float currentAutomaticTime = 0.0f;

    [NonSerialized] public bool canAutoPark = false;
    [NonSerialized] public bool forceAutoPark = false;
    [NonSerialized] public bool automaticMode;
    [NonSerialized] public bool beginAutomaticMode = true;

    [NonSerialized] public bool isParking = false;
    [NonSerialized] public bool isSubmarineParked = false;

    [NonSerialized] public bool collisionOn = false;

    [Header("Parking Settings")]
    [SerializeField] private float autoParkDuration = 8f; // how many seconds the autopark should take

    void Awake()
    {
        mover.MoverController = this;

        TransientRotation = transform.rotation;
        TransientPosition = transform.position;
    }

    void Start()
    {
        if (GameManager.GetInstance().GetPlayerState() == PlayerState.OUTSIDE_SUBMARINE)
        {
            isSubmarineParked = true;
        }

        engineSoundEmitter.Play();
    }

    /// <summary>
    /// Sets the character's position directly
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        TransientPosition = position;
    }

    /// <summary>
    /// Sets the character's rotation directly
    /// </summary>
    public void SetRotation(Quaternion rotation)
    {
        TransientRotation = rotation;
    }

    /// <summary>
    /// Sets the character's position and rotation directly
    /// </summary>
    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        TransientPosition = position;
        TransientRotation = rotation;
    }

    public void TrySwitchState(SubmarineState _state)
    {
        if (lastPark)
        {
            parkingSpeedPosition = lastParkingSpeedPosition;
            parkingSpeedRotation = lastParkingSpeedRotation;
        }
        else if (forceAutoPark)
        {
            parkingSpeedPosition = forceParkingSpeedPosition;
            parkingSpeedRotation = forceParkingSpeedRotation;
        }
        else
        {
            parkingSpeedPosition = manualParkingSpeedPosition;
            parkingSpeedRotation = manualParkingSpeedRotation;
        }

        switch (_state)
        {
            case SubmarineState.MANUAL:
                {
                    isSubmarineParked = false;
                    state = _state;
                    isParking = false;
                    break;
                }
            case SubmarineState.AUTOMATIC:
                {
                    if (canAutoPark || forceAutoPark)
                    {
                        isSubmarineParked = false;
                        // forceAutoPark = false;
                        isParking = true;
                        state = _state;
                    }
                    break;
                }
        }
    }

    public void UpdateMovement(out Vector3 _goalPosition, out Quaternion _goalRotation, float _deltaTime)
    {
        //Collison fix
        if (GameManager.GetInstance().GetPlayerState() == PlayerState.OUTSIDE_SUBMARINE)
        {
            collisionOn = false;
        }
        else
        {
            collisionOn = true;
        }
        ////

        input = new Vector3(
            Mathf.LerpUnclamped(0, forwardMaxSpeed, forwardInput),
            Mathf.LerpUnclamped(0, upMaxSpeed, upInput),
            Mathf.LerpUnclamped(0, turnMaxSpeed, turnInput)
        );

        switch (state)
        {
            case SubmarineState.MANUAL:
                {
                    ManualControl(_deltaTime);
                    break;
                }
            case SubmarineState.AUTOMATIC:
                {
                    AutomaticControl(_deltaTime);
                    break;
                }
        }

        UpdateRotation(out _goalRotation);
        UpdatePosition(out _goalPosition);

        UpdateSound();
    }

    #region Automatic
    // Automatic
    private void AutomaticControl(float _deltaTime)
    {
        if (beginAutomaticMode)
        {
            startPosition = TransientPosition;
            startRotation = TransientRotation;

            currentAutomaticTime = 0.0f;

            beginAutomaticMode = false;
        }

        currentAutomaticTime += _deltaTime;

        TransientPosition = Vector3.LerpUnclamped(startPosition, targetPosition, parkingSpeedPosition.Evaluate(Mathf.Clamp(currentAutomaticTime, 0.0f, parkingSpeedPosition.keys.Last().time)));
        TransientRotation = Quaternion.SlerpUnclamped(startRotation, targetRotation, parkingSpeedRotation.Evaluate(Mathf.Clamp(currentAutomaticTime, 0.0f, parkingSpeedRotation.keys.Last().time))).normalized;

        if (currentAutomaticTime >= parkingSpeedRotation.keys.Last().time && currentAutomaticTime >= parkingSpeedPosition.keys.Last().time)
        {
            if (!isSubmarineParked)
            {
                parkedEmitter.Play();
                Debug.Log("ParkSound debug");
            }
            isSubmarineParked = true;

            if (lastPark)
            {
                FindAnyObjectByType<ParkButton>().enabled = false;
            }
            else if (subTeleport)
            {
                subTeleport = false;
                forceAutoPark = false;
            }
            else if (forceAutoPark)
            {
                forceAutoPark = false;
                TrySwitchState(SubmarineState.MANUAL);
            }
        }

        // if (beginAutomaticMode)
        // {
        //     startPosition = TransientPosition;
        //     startRotation = TransientRotation;

        //     currentAutomaticTime = 0.0f;

        //     beginAutomaticMode = false;
        // }

        // currentAutomaticTime += _deltaTime;

        // // t is normilized from 0 to 1
        // // 0 → 1 over autoParkDuration seconds
        // float t01 = Mathf.Clamp01(currentAutomaticTime / autoParkDuration);

        // //curves that go from time 0→1 on X
        // float posT = parkingSpeedPosition.Evaluate(t01);
        // float rotT = parkingSpeedRotation.Evaluate(t01);

        // TransientPosition = Vector3.LerpUnclamped(startPosition, targetPosition, posT);
        // TransientRotation = Quaternion.SlerpUnclamped(startRotation, targetRotation, rotT).normalized;

        // // Done when t01 reaches 1
        // if (t01 >= 1.0f)
        // {
        //     isSubmarineParked = true;

        //     if (forceAutoPark)
        //     {
        //         forceAutoPark = false;
        //         TrySwitchState(SubmarineState.MANUAL);
        //     }
        // }
    }
    // Automatic
    #endregion Automatic

    #region Manual
    // Manual
    private void ManualControl(float _deltaTime)
    {
        UpdateAngularVelocity(_deltaTime);
        UpdateLinearVelocity(_deltaTime);
    }

    private void UpdateLinearVelocity(float _deltaTime)
    {
        Vector3 currentLinearVelocity = LinearVelocity;

        // Altitude
        Vector3 currentUpLinearVelocity = new Vector3(0.0f, currentLinearVelocity.y, 0.0f);

        Vector3 targetUpVelocity = input.y * transform.up.normalized;

        Vector3 upVelocity = Vector3.Lerp(
            a: currentUpLinearVelocity,
            b: targetUpVelocity,
            t: 1.0f - Mathf.Exp(-upVelocityResponseSpeed * _deltaTime)
            );
        // Altitude

        // Latitude
        Vector3 currentForwardLinearVelocity = new Vector3(currentLinearVelocity.x, 0.0f, currentLinearVelocity.z);

        Vector3 targetForwardVelocity = input.x * transform.forward.normalized;

        Vector3 forwardVelocity = Vector3.Lerp(
            a: currentForwardLinearVelocity,
            b: targetForwardVelocity,
            t: 1.0f - Mathf.Exp(-forwardVelocityResponseSpeed * _deltaTime)
            );
        // Latitude

        LinearVelocity = upVelocity + forwardVelocity;

        TransientPosition += LinearVelocity * _deltaTime;

    }

    private void UpdateAngularVelocity(float _deltaTime)
    {
        Vector3 currentAngularVelocity = AngularVelocity;

        Vector3 currentWorldTurnVelocity = new Vector3(0.0f, currentAngularVelocity.y, 0.0f);

        Quaternion current = TransientRotation;
        Quaternion target = Quaternion.Euler(new Vector3(0.0f, 1.0f, 0.0f));

        Quaternion delta = target * Quaternion.Inverse(current);

        float angle; Vector3 axis;
        delta.ToAngleAxis(out angle, out axis);

        if (angle > 180f)
            angle -= 360f;

        Vector3 stabilizeForce = Mathf.Deg2Rad * angle * axis.normalized;

        Vector3 upStabilizeForce = new Vector3(0.0f, stabilizeForce.y, 0.0f);

        Vector3 stabilizeForceXZ = stabilizeForce - upStabilizeForce;

        Vector3 finalStabilizeTorque = Vector3.Lerp(
            a: currentStabilizeTorque,
            b: stabilizeForceXZ,
            t: 1.0f - Mathf.Exp(-stabilizeResponseSpeed * _deltaTime)
            );

        Vector3 finalTurnTorque = Vector3.Lerp(
            a: currentWorldTurnVelocity,
            b: new Vector3(0.0f, input.z, 0.0f),
            t: 1.0f - Mathf.Exp(-turnVelocityResponseSpeed * _deltaTime)
            );

        AngularVelocity = finalStabilizeTorque + finalTurnTorque;
        currentStabilizeTorque = finalStabilizeTorque;

        TransientRotation = Quaternion.Euler(AngularVelocity * _deltaTime) * TransientRotation;
    }

    void UpdatePosition(out Vector3 _goalPosition)
    {
        _goalPosition = TransientPosition;
    }

    void UpdateRotation(out Quaternion _goalRotation)
    {
        _goalRotation = TransientRotation;
    }
    // Manual
    #endregion Manual

    private void UpdateSound()
    {
        float normalizedInput = Mathf.Clamp(forwardInput, -1.0f, 1.0f);

        if (forwardInput < 0.0f)
            normalizedInput *= -1.0f;

        engineSoundParameter.Value = normalizedInput;
        engineSoundParameter.TriggerParameters();
    }

    public void OnCollisionCallBack(Collision collision)
    {
        float subCollisionBounce = LinearVelocity.magnitude / 1.0f;
        ApplyForceAtPoint(collision.contacts[0].normal * subCollisionBounce, (collision.contacts[0].point - center.transform.position) * collisionExageration);
        LinearVelocity = Vector3.ClampMagnitude(LinearVelocity, collisionMaxLinearVelocity);
        AngularVelocity = Vector3.ClampMagnitude(AngularVelocity, collisionMaxAngularVelocity);
    }

    public void ApplyForceAtPoint(Vector3 force, Vector3 point)
    {
        LinearVelocity += force;
        AngularVelocity += Vector3.Cross(point - center.transform.position, force);
    }
}
