using ArtNotes.PhysicalInteraction;
using FMODUnity;
using UnityEngine;

public class CS_FixElectric : InteractableObject
{
    [SerializeField] private CS_ElectricalHazardMaster _ehm;
    [SerializeField] private CS_SubmarineHazardChecker _shc;
    [SerializeField] private FMODUnity.EventReference SuccsededInFixingSound;
    [SerializeField] private FMODUnity.EventReference FailedInFixingSound;
    [SerializeField] private FMODUnity.EventReference IsFixingSound;

    [SerializeField] private float _fixTime = 0.3f;
    private float _fixTimer = 0.0f;
    private bool _isFixing = false;
    private bool _fixed = true;

    private void Start()
    {
        _fixTimer = _fixTime;

        if (!_ehm)
            _ehm = FindAnyObjectByType<CS_ElectricalHazardMaster>();
        if (!_shc)
            _shc = FindAnyObjectByType<CS_SubmarineHazardChecker>();
    }

    private void Update()
    {
        if (!_shc.Electric && _shc.ElIsOff && _isFixing)
        {
            _fixed = false;
            RuntimeManager.PlayOneShot(IsFixingSound);
            _fixTimer -= Time.deltaTime;
            if (_fixTimer <= 0.0f)
            {
                for (int i = 0; i < _ehm._electraicalObjects.Length; i++)
                {
                    _ehm._electraicalObjects[i].TurnOn();
                }
                _shc.ElIsOff = false;
            }
            else
            {
                for (int i = 0; i < _ehm._electraicalObjects.Length; i++)
                {
                    _ehm._electraicalObjects[i].TurnOff();

                }
            }
        }
        else if (_isFixing && _shc.Electric) {
            RuntimeManager.PlayOneShot(IsFixingSound);
            for (int i = 0; i < _ehm._electraicalObjects.Length; i++)
            {
                _ehm._electraicalObjects[i].TurnOff();

            }
        }
    }

    public override void InteractStart(RaycastHit hit)
    {
        _isFixing = true;
    }

    public override void InteractEnd()
    {
        _isFixing = false;
        if (_fixTime > 0.0f && _shc.ElIsOff) 
            RuntimeManager.PlayOneShot(FailedInFixingSound);
        else if (!_fixed)
        {
            RuntimeManager.PlayOneShot(SuccsededInFixingSound);
            _fixed = true;
        }
        _fixTimer = _fixTime;
    }
}
