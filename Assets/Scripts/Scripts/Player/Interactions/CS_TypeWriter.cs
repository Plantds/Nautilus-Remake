using FMODUnity;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct AudioAndMessageForTypeWriter
{
    public string msg;
    public FMODUnity.EventReference clip;
}

public class TypeWriterMessage
{
    public int _charIndex = 0;
    public float _timer = 0;
    public bool _endOfMsg = false;
    
    private float _timePerChar = 0.05f;
    private string _currentMsg = null;
    private string _displayMsg = null;

    private Action _onActionCallback = null;
    private FMODUnity.EventReference _currClip;

    public TypeWriterMessage(string msg, FMODUnity.EventReference audio, Action callback = null)
    {
        _currentMsg = msg;
        _onActionCallback = callback;
        _currClip = audio;
    }

    public void Callback()
    {
        if (_onActionCallback != null) _onActionCallback();
    }

    public FMODUnity.EventReference GetAudioClip()
    {
        return _currClip;
    }

    public string GetFullMsgAndCallback()
    {
        if (_onActionCallback != null) _onActionCallback();
        return _currentMsg;
    }

    public string GetFullMsg()
    {
        return _currentMsg;
    }

    public string GetMsg()
    {
        return _displayMsg;
    }

    public void Update()
    {
        if(string.IsNullOrEmpty(_currentMsg)) return;

        _timer -= Time.deltaTime;
        if(_timer < 0)
        {
            _timer += _timePerChar;
            _charIndex++;

            _displayMsg = _currentMsg.Substring(0, _charIndex);
            //_displayMsg += _currentMsg.Substring(_charIndex);

            if(_charIndex >= _currentMsg.Length)
            {
                Callback();
                _endOfMsg = true;
                _currentMsg = null;
            }
            else
            {
                _endOfMsg = false;
            }
        }
    }

    public bool IsActive()
    {
        if(string.IsNullOrEmpty(_currentMsg)) return false;
        return _charIndex < _currentMsg.Length;
    }
}

public class CS_TypeWriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textComponent;
    [SerializeField] private float _timeBetweenStrings = 1.5f;

    private static CS_TypeWriter _instance;
    private List<TypeWriterMessage> _messages = new List<TypeWriterMessage>();

    private TypeWriterMessage _currMsg = null;
    private int _msgIndex = 0;
    private bool _stop = false;
    private float _betweenMsgTimer = 0.0f;
    
    [HideInInspector] public bool _isActive = false;

    public void Add(string msg, FMODUnity.EventReference audio, Action callback = null)
    {
        TypeWriterMessage typeMsg = new TypeWriterMessage(msg, audio, callback);
        _instance._messages.Add(typeMsg);
    }

    public void Activate()
    {
        _stop = false;
        _textComponent.enabled = true;
        _instance._currMsg = _instance._messages[_msgIndex];
        _isActive = true;
        RuntimeManager.PlayOneShot(_currMsg.GetAudioClip());
    }

    private void Awake()
    {
        _instance = this;
        _textComponent.enabled = false;
    }

    private void Update()
    {
        if(_messages.Count > 0 && _currMsg != null && !_stop)
        {
            _currMsg.Update();
            _textComponent.SetText(_currMsg.GetMsg());
            if (_currMsg._endOfMsg)
            {
                _betweenMsgTimer -= Time.deltaTime;
                if (_betweenMsgTimer <= 0.0f)
                {
                    //_fmodEmitter.Stop();
                    if(_msgIndex + 1 != _messages.Count)
                        RuntimeManager.PlayOneShot(_messages[_msgIndex+1].GetAudioClip());
                    //_fmodEmitter.Play();
                    WriteNextMessageInQueue();
                }
            }
            else
            {
                _betweenMsgTimer = _timeBetweenStrings;
            }
        }
    }

    public void WriteNextMessageInQueue()
    {
        if (_msgIndex < _messages.Count-1)
        {
            _msgIndex++;
            _currMsg._endOfMsg = false;
            _instance._currMsg = _instance._messages[_msgIndex];
        }
        else
        {
            ForceStop();
        }
    }

    public void ClearQueue()
    {
        _messages.Clear();
    }

    public void StartOver()
    {
        _msgIndex = 0;
        foreach(TypeWriterMessage m in _messages)
        {
            m._charIndex = 0;
            m._timer = 0.0f;
        }
    }

    public void ForceStop()
    {
        _stop = true;
        _textComponent.SetText("");
        _textComponent.enabled = false;
        _msgIndex = 0;
        StartOver();
        ClearQueue();
        _isActive = false;
    }
}
