using UnityEngine;
using KinematicCharacterController;
using ArtNotes.PhysicalInteraction;

public class PlayerComponent : MonoBehaviour
{
    public CharacterControllerComponent Character;
    public CharacterCameraComponent CharacterCamera;
    [SerializeField] private Interactor interactor;

    InputSystem_Actions inputActions;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        inputActions = new InputSystem_Actions();

        inputActions.Enable();

        // Tell camera to follow transform
        CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        CharacterCamera.settings.IgnoredColliders.Clear();
        CharacterCamera.settings.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
    }

    private void Update()
    {
        HandleCharacterInput();
    }

    private void LateUpdate()
    {
        // Handle rotating the camera along with physics movers
        if (CharacterCamera.settings.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
        {
            CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
            CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
        }

        HandleCameraInput();
    }

    private void HandleCameraInput()
    {
        // Create the look input vector for the camera
        Vector2 lookAxis = inputActions.Player.Look.ReadValue<Vector2>();
        // float mouseLookAxisRight = inputActions.Player.Look.ReadValue<Vector2>().x;
        Vector3 lookInputVector = new Vector3(lookAxis.x, lookAxis.y, 0f);

        //lookInputVector = Vector3.ClampMagnitude(lookInputVector, 1.0f);

        //Debug.Log(lookInputVector);

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        // Apply inputs to the camera
        CharacterCamera.UpdateWithInput(Time.deltaTime, lookInputVector *= 0.1f);
        interactor.UpdateInteractor();
    }

    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        Vector2 moveAxis = new Vector2();
        moveAxis += inputActions.Player.KeyboardMove.ReadValue<Vector2>();
        moveAxis += inputActions.Player.ControllerMove.ReadValue<Vector2>();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = moveAxis.y;
        characterInputs.MoveAxisRight = moveAxis.x;
        characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
        characterInputs.JumpDown = inputActions.Player.Jump.IsPressed();

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }
}