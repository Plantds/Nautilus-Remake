using UnityEngine;

namespace ArtNotes.PhysicalInteraction
{
	[RequireComponent(typeof(Rigidbody))]
	public class Box : InteractableObject
    {
        [SerializeField] int _force = 30;
        [SerializeField] Collider _connectedTrigger;
        [SerializeField] bool _onlyOneTimeConnect;

        bool _oneTimeConnected, _avoidTriggerFix;
        bool _isMove;
        Rigidbody _rb;
        Transform _camTransform;
        float _targetDistanceToCam;

        Vector3 _targetPos => _camTransform.position + _camTransform.forward * _targetDistanceToCam;

        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _camTransform = Camera.main.transform;

            if (_connectedTrigger != null && !_connectedTrigger.isTrigger)
            	_connectedTrigger.isTrigger = true;
        }

        void FixedUpdate()
        {
            if (!_isMove) return;

            Vector3 force = _targetPos - HitPos;
            force *= _force;
            _rb.AddForceAtPosition(force, HitPos);
        }

        public override void InteractStart(RaycastHit hit)
        {
            base.InteractStart(hit);
            if (_onlyOneTimeConnect && _oneTimeConnected) return;
            
            if (_rb.isKinematic)
            {
                _avoidTriggerFix = true;
            	_rb.isKinematic = false; // OnTriggerEnter() after this line! It is reason to use _avoidTriggerFix
                if (_mainExecutor != null) _mainExecutor.Execute(0);
            }
            _targetDistanceToCam = hit.distance;
            _isMove = true;
        }
        public override void InteractEnd()
        {
            base.InteractEnd();
            _isMove = false;
        }
        
        void OnTriggerEnter(Collider other)
        {
        	if (other != _connectedTrigger) return;
            if (_avoidTriggerFix)
            {
                _avoidTriggerFix = false;
                return;
            }

        	_rb.isKinematic = true;
        	InteractEnd();

        	transform.position = other.transform.position;
            transform.rotation = other.transform.rotation;

            if (_mainExecutor != null) _mainExecutor.Execute(1);
        	if (_onlyOneTimeConnect) _oneTimeConnected = true;
        }
    }
}
