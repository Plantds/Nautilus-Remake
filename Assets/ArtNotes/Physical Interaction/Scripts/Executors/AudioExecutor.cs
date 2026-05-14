using UnityEngine;

namespace ArtNotes.PhysicalInteraction
{
    [RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
    public class AudioExecutor : Executor
    {
        [SerializeField] bool 	_playDuringChange,
        						// _editVolumeBySignal,
        						_playWhenSignalPositive,
        						_playWhenSignalZero;
        [SerializeField] float _multiply = 1;

        float _currentSignal, _lastSignal;
        FMODUnity.StudioEventEmitter _source;

        public bool pauseInsteadOfStop;
        float pauseSoundTimer;
        public float pauseSoundTime = 0.1f;

        void Awake()
        {
            _source = GetComponent<FMODUnity.StudioEventEmitter>();
        }  

        void FixedUpdate()
        {
            if (!_playDuringChange) return;

            bool isPaused;
            _source.EventInstance.getPaused(out isPaused);

            if (pauseInsteadOfStop)
            {
                if (_source.IsPlaying())
                {
                    pauseSoundTimer += Time.deltaTime;
                    if (_lastSignal != _currentSignal)
                    {
                        _source.EventInstance.setPaused(false);
                        pauseSoundTimer = 0;
                    }
                    else if (_lastSignal == _currentSignal && !isPaused && pauseSoundTimer >= pauseSoundTime)
                    {
                        _source.EventInstance.setPaused(true);
                    }
                }
                else
                {
                    if (_lastSignal != _currentSignal) _source.Play();
                }
            }
            else
            {
                if (_lastSignal != _currentSignal && !_source.IsPlaying()) _source.Play();
                else if (_lastSignal == _currentSignal && _source.IsPlaying()) _source.Stop();
            }
            
            
            _lastSignal = _currentSignal;
        }

        public override void Execute(float signal)
        {
            if (!_playDuringChange)
            {
                if (signal > 0 && !_source.IsPlaying()) _source.Play();
                else if (signal <= 0 && _source.IsPlaying()) _source.Stop();
            }
            else
                _currentSignal = signal;

            // if (_editVolumeBySignal && signal >= 0 && _source.IsPlaying()) _source.volume = signal * _multiply;
            
			if (_playWhenSignalPositive && signal >= 1) _source.Play();
            if (_playWhenSignalZero && signal < 1) _source.Play();
        }
    }
}