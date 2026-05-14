using ArtNotes.PhysicalInteraction;
using System.Data.Common;
using UnityEngine;

public class CS_InspectInteraction : InteractableObject
{
    [Tooltip("needs the character component from the player specificaly")]
    [SerializeField] private CharacterControllerComponent _player;
    [Tooltip("the place the text will write to")]
    [SerializeField] private CS_TypeWriter _typeWriter;
    [SerializeField] private float _CancelDistance = 12.5f;
    [SerializeField] private AudioAndMessageForTypeWriter[] _messages;

    public bool _debug = false;
    private bool _isInteracted = false;
    private bool _flipFlop = false;
    public void AddMsgsToTypeWriter()
    {
        foreach (AudioAndMessageForTypeWriter msg in _messages)
        {
            _typeWriter.Add(msg.msg, msg.clip);
        }
    }
    void Start()
    {
        if (!_typeWriter)
        {
            _typeWriter = FindAnyObjectByType<CS_TypeWriter>();
        }
        if (!_player)
        {
            _player = FindAnyObjectByType<CharacterControllerComponent>();
        }
    }

    private void Update()
    {

        if(_isInteracted && Vector3.Distance(transform.position, _player.transform.position) < _CancelDistance)
        {
            if (_flipFlop && !_typeWriter._isActive)
            {
                AddMsgsToTypeWriter();
                _typeWriter.Activate();
                _flipFlop = false;
            }
            else if (_typeWriter._isActive && _flipFlop) {
                _typeWriter.ForceStop();
            }
        }
        else if (_isInteracted && Vector3.Distance(transform.position, _player.transform.position) > _CancelDistance)
        {
            Debug.LogWarning("If you were expecting to see your msg write out and it isnt, check how long your Cancel Distance is. Currently it is " + 
                _CancelDistance + 
                " try making it longer");

           if(_debug)
                Debug.DrawLine(transform.position, _player.transform.position, Color.red, 3.5f);

            _typeWriter.ForceStop();
            _isInteracted = false;
            InteractEnd();
        }
    }

    // -- Intectaction --
    public override void InteractStart(RaycastHit hit)
    {
        if (!_typeWriter._isActive)
        {
            base.InteractStart(hit);
            _isInteracted = _isInteracted ? false : true; ;
            _flipFlop = true;
        }
    }
    public override void InteractEnd()
    {
        base.InteractEnd();
    }
}
