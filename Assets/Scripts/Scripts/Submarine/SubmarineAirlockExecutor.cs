using System;

namespace ArtNotes.PhysicalInteraction
{
    public class SubmarineAirlockExecutor : Executor
    {
        [NonSerialized] private float signal;
        public float Signal { get { return signal; } }

        public override void Execute(float _signal)
        {
            signal = _signal;
        }
    }
}