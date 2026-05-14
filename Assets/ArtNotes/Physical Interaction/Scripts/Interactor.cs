using UnityEngine;
using UnityEngine.InputSystem;

namespace ArtNotes.PhysicalInteraction
{
	public class Interactor : MonoBehaviour
    {
        #region delcarations
        [Space]
        [SerializeField] int _distanceMax = 3;
        [SerializeField] HandUI _hand;
        [SerializeField] private InputActionAsset _actionAsset;
        [SerializeField] private CharacterControllerComponent _player;
        [SerializeField] private string _interactionActionMapName;
        [SerializeField] private string _interactionActionName;

        InteractableObject _interactable;
        RectTransform _handRect;
        public Camera _camera;
        RaycastHit _hit;
        private bool _down;
        private bool _up;

        private InputAction _interactAction;

        public float LookSpeedMultiply { get; private set; } = 1;
		#endregion
		
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked; // TODO displace to GameManager

            if (_hand)
            {
                _hand.SetEnableImage(false);
                _handRect = _hand.GetComponent<RectTransform>();
                if (_handRect == null) { Debug.LogError("handRect is Null"); return; }
                _handRect.position = new Vector3(Screen.width / 2, Screen.height / 2);
            }

            _interactAction = _actionAsset.FindActionMap(_interactionActionMapName).FindAction(_interactionActionName);
            if (_interactAction == null)
                Debug.LogError("_actionAsset is Null; Check [_interactionActionName] or [_interactionActionMapName] for spelling mistakes or if the action does exist");


            _interactAction.Enable();

            _interactAction.started += PressedButton;
            _interactAction.canceled += RelasedButton;
        }

        void PressedButton(InputAction.CallbackContext context)
        {
            _up = false;
            _down = true;
        }

        void RelasedButton(InputAction.CallbackContext context)
        {
            _down = false;
            _up = true;
        }

        public void UpdateInteractor()
        {
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hit, _distanceMax))
            {
                var target = _hit.collider.gameObject.GetComponent<InteractableObject>();
                if (target)
                {
                    if (_hand) _hand.SetEnableImage(true);
                    if (!_interactable && _hand) _hand.SetTexture(PlayerLooking.HandMode.canUse);

                    if (Input.GetButtonDown("Fire1"))
                    {
                        _interactable = target;
                        _interactable.InteractStart(_hit);
                        LookSpeedMultiply = _interactable.LookingSpeed; 

                        if (_hand) _hand.SetTexture(_interactable.Hand);
                        _down = false;
                    }
                }
                else if (!_interactable && _hand) _hand.SetEnableImage(false);
            }
            else if (!_interactable && _hand) _hand.SetEnableImage(false);

            if (_interactable == null) return;

            //We might change this later (0.55 is just a temp value)
            if(Vector3.Distance(_interactable.transform.position, _player.transform.position) > _distanceMax + 0.55f) endInteraction();

            if (_hand) _handRect.position =
                    _camera.WorldToScreenPoint(_interactable.HitPos);

            if (Input.GetButtonUp("Fire1")) //  || _interactCurerntDistance < _maxInteractDistance
            {
                endInteraction();
                _up = false;
            }

            if(_down)
                _down = false;
            if(_up)
                _up = false;
        }

        public void endInteraction()
        {
            _interactable.InteractEnd();
                _interactable = null;
                LookSpeedMultiply = 1;
                if (_hand != null)
                {
                    _hand.SetEnableImage(false);
                    _handRect.position = new Vector3(Screen.width / 2, Screen.height / 2);
                }
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            if (_interactable == null)
                Gizmos.color = Color.white;
            else
            {
            	Gizmos.color = Color.red;
            	Gizmos.DrawSphere(_interactable.HitPos, .1f);
            }
            Gizmos.DrawLine(_camera.transform.position,
                            _camera.transform.position + _camera.transform.forward * _distanceMax);
        }
    }
}
