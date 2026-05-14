using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace ArtNotes.PhysicalInteraction
{
    public class TelColliderExecutor : Executor
    {
        [SerializeField] Collider _collider;

        [SerializeField] float triggerWhenAbove;

        bool triggerOnce = true;

        void Awake()
        {
        }

        public override void Execute(float signal)
        {
            if (triggerOnce && signal >= triggerWhenAbove)
            {
                StartCoroutine(despawnCollider());
            }
        }

        IEnumerator despawnCollider()
        {
            _collider.gameObject.transform.position = new Vector3(10000, 10000, 10000);

            yield return new WaitForSeconds(2);

            yield return null;
        }
    }
}
