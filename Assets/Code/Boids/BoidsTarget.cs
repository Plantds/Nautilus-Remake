using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Boids
{
    public struct BoidsGPUTarget
    {
        public Vector3 Position;
        public float   Range2;
        public float   Factor;
        public float   SolidRange;

        public const int Size = sizeof( float ) * 6;
    };

    [ ExecuteInEditMode ]
    public class BoidsTarget : BoidsInteractable
    {
        [ Header( "Settings" ) ]
        [ SerializeField ]
        public float radius   = 1.0f;
        [ FormerlySerializedAs( "fraction" ) ]
        public float factor = 0.5f;
        [ SerializeField ]
        private bool solid = false;
        
        private Vector3 prevPosition;
        
        private bool isNegative => radius * factor < 0.0f;

        public override Type BufferElementType => typeof( BoidsGPUTarget );

        private void OnEnable()
        {
            AddOrUpdate();
        }

        public void AddOrUpdate()
        {
            OwnData = GetTarget();
            AddSelfToManager();
        }

        public void Remove()
        {
            RemoveSelfFromManager();
        }

        private void Update()
        {
            if( transform.position != prevPosition )
                AddOrUpdate();
        }

        private void OnDestroy()
        {
            Remove();
        }

        private void OnDisable()
        {
            Remove();
        }

        public BoidsGPUTarget GetTarget()
        {
            return new BoidsGPUTarget()
            {
                Position   = transform.position,
                Range2     = radius * radius * Mathf.Sign( radius ),
                Factor     = factor * Mathf.Sign( radius ),
                SolidRange = solid ? ( isNegative ? Mathf.Abs( radius ) : 0.0f ) : 0.0f,
            };
        }
        
    #if UNITY_EDITOR
        private void OnValidate()
        {
            AddOrUpdate();
        }

        private void DrawGizmos()
        {
            Gizmos.color = isNegative ? Color.red : Color.green;
            Gizmos.DrawWireSphere( transform.position, Mathf.Abs( radius ) );
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }
    #endif
    }
}
