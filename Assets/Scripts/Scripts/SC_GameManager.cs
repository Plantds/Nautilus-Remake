using System;
using KinematicCharacterController;
using UnityEngine;

public enum PlayerState
{
    INSIDE_SUBMARINE = 6,
    OUTSIDE_SUBMARINE = 7,
}

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;

    public static GameManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_GameManager").AddComponent<GameManager>();
        }

        return _instance;
    }
    #endregion

    #region Variables
    [Header("Player State")]
    [SerializeField] public PlayerState currentPlayerState = PlayerState.OUTSIDE_SUBMARINE;
    [NonSerialized] private PlayerState previousPlayerState = PlayerState.OUTSIDE_SUBMARINE;

    public PlayerState GetPlayerState() { return currentPlayerState; }
    public void SetPlayerState(PlayerState _playerState) { currentPlayerState = _playerState; submarineAirlockSequence.canSetStartState = true;}

    [Header("Player References")]
    [SerializeField] public PlayerComponent playerComponent;
    [SerializeField] public SC_HelmetStateSwitcher helmetStateSwitcher;

    [Header("Submarine References")]
    [SerializeField] public SubmarineAirlock submarineAirlock;
    [SerializeField] public SC_SubmarineAirlockSequence submarineAirlockSequence;
    [SerializeField] public SC_SubmarineMovement submarineMovement;

    [NonSerialized] private PlayerHandler playerStateHandler;
    [NonSerialized] private SubmarineHandler submarineStateHandler;
    [NonSerialized] private AudioHandler audioStateHandler;
    #endregion

    #region On Awake
    private void Awake()
    {
        _instance = this;

        ValidateHandlers();

        ChangeState(currentPlayerState);
    }
    #endregion

    void OnValidate()
    {
        ValidateHandlers();
        ChangeState(currentPlayerState);
    }

    void ValidateHandlers()
    {
        if (playerStateHandler == null)
        {
            playerStateHandler = new PlayerHandler();
            playerStateHandler.playerComponent = playerComponent;
            playerStateHandler.helmetStateSwitcher = helmetStateSwitcher;
        }
        if (submarineStateHandler == null)
        {
            submarineStateHandler = new SubmarineHandler();
            submarineStateHandler.submarineAirlock = submarineAirlock;
        }
        if (audioStateHandler == null)
        {
            audioStateHandler = new AudioHandler();
        }
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;

        KinematicCharacterSystem.PreSimulationInterpolationUpdate(deltaTime);
        KinematicCharacterSystem.Simulate(deltaTime, KinematicCharacterSystem.CharacterMotors, KinematicCharacterSystem.PhysicsMovers);
        KinematicCharacterSystem.PostSimulationInterpolationUpdate(deltaTime);

        if (currentPlayerState != previousPlayerState)
        {
            ChangeState(currentPlayerState);
        }

    }

    private void LateUpdate()
    {

    }

    void ChangeState(PlayerState _state)
    {
        playerStateHandler.ChangeState(_state);
        submarineStateHandler.ChangeState(_state);
        audioStateHandler.ChangeState(_state);

        previousPlayerState = _state;
    }
}

public class PlayerHandler : IStateChanger
{
    // References
    public PlayerComponent playerComponent;
    public SC_HelmetStateSwitcher helmetStateSwitcher;

    public void ChangeState(PlayerState _state)
    {
        playerComponent.Character.playerState = _state;
        helmetStateSwitcher.ChangeState(_state);
    }

    public void SetPlayerPositionAndRotation(Vector3 _position, Quaternion _rotation, bool _bypassInterpolation = true)
    {
        playerComponent.Character.Motor.SetPositionAndRotation(_position, _rotation, _bypassInterpolation);
    }
}

public class SubmarineHandler : IStateChanger
{
    // References
    public SubmarineAirlock submarineAirlock;
    public SC_SubmarineMovement submarineMovement;

    public void ChangeState(PlayerState _state)
    {
        submarineAirlock.ChangeLayer(_state);
    }

    public void SetSubmarinePositionAndRotation(Vector3 _position, Quaternion _rotation, bool _bypassInterpolation = true)
    {
        submarineMovement.mover.SetPositionAndRotation(_position, _rotation);
    }
}

public class AudioHandler : IStateChanger
{
    // References


    public void ChangeState(PlayerState _state)
    {

    }
}

public interface IStateChanger
{
    public void ChangeState(PlayerState _state);
}