using UnityEngine;

namespace ArtNotes.PhysicalInteraction
{
    public class SubmarineStearingExecutor : Executor
    {
        public enum SubInput
        {
            forward,
            turn,
            up,

        }
        public SubInput subInput;
        [SerializeField] SC_SubmarineMovement submarine;
        [SerializeField] float maxSpeed;
        public override void Execute(float signal)
        {
            if (subInput == SubInput.forward) submarine.forwardInput = signal * maxSpeed;
            if (subInput == SubInput.turn) submarine.turnInput = signal * maxSpeed;
            if (subInput == SubInput.up) submarine.upInput = signal * maxSpeed;
        }
    }
}
