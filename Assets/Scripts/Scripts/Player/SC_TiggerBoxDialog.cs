using UnityEngine;

public class SC_TiggerBoxDialog : MonoBehaviour
{
    public enum ColliderType
    {
        Player,
        Submarine
    }
    [SerializeField] private ColliderType colliderType;

    [Tooltip("the place the text will write to")]
    [SerializeField] private CS_TypeWriter _typeWriter;
    [SerializeField] private AudioAndMessageForTypeWriter[] _messages;
    private bool _triggered = false;
    private bool _once = true;

    private void Start() 
    {
        if (!_typeWriter)
        {
            _typeWriter = FindAnyObjectByType<CS_TypeWriter>();
        }
    }

    public void AddMsgsToTypeWriter()
    {
        foreach (AudioAndMessageForTypeWriter msg in _messages)
        {
            _typeWriter.Add(msg.msg, msg.clip);
        }
    }

    private void Update()
    {
        if (_triggered && _once) {
            AddMsgsToTypeWriter();
            _typeWriter.Activate();
            _once = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (colliderType == ColliderType.Player)
        {
            tag = "Player";
        }
        else
        {
            tag = "SubmarineCollider";
        }
        if (!other.gameObject.CompareTag(tag))
            return;

        _triggered = true;
    }
}
