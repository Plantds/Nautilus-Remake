using UnityEngine;

namespace ArtNotes.PhysicalInteraction
{
    public class ToggleColliderExecutor : Executor
    {
        [SerializeField] Collider _collider;

        [SerializeField] float triggerWhenAbove;

        [SerializeField] bool turnOff = true;

        bool triggerOnce = true;

        void Awake()
        {
        }

        public override void Execute(float signal)
        {
            if (triggerOnce && signal >= triggerWhenAbove)
            {
                if (turnOff)
                {
                    _collider.enabled = false;
                }
                else
                {
                    _collider.enabled = true;
                }
            }
        }
    }
}
