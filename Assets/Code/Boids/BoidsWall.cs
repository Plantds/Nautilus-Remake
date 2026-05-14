using System;
using Unity.Mathematics;
using UnityEngine;

namespace Boids
{
    [ Serializable ]
    public struct BoidsGPUWall
    {
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 Force;

        public int3 ExitFaces;

        public const int Size = sizeof( float ) * 9 + sizeof( uint ) * 3;
    }

    [ ExecuteInEditMode ]
    [ RequireComponent( typeof( BoxCollider ) ) ]
    public class BoidsWall : BoidsInteractable
    {
        [ Header( "Settings" ) ]
        public Vector3     direction = Vector3.forward;
        public float       factor    = 0.7f;
        public bool3       solidFace = false;

        private BoidsGPUWall wallCache;
        private BoxCollider  boxCollider;
        private Quaternion   rotation = Quaternion.identity;

        public override Type BufferElementType =>  typeof( BoidsGPUWall );

        private void Start()
        {
            rotation = Quaternion.LookRotation( direction, Vector3.up );
            AddSelf();
        }

        private void OnEnable()
        {
            boxCollider = GetComponent< BoxCollider >();
            AddSelf();
        }

        private void OnDisable()
        {
            RemoveSelf();
        }

        private static int GetAxisIndex( float value )
        {
            return value switch
            {
                < 0.0f => -1,
                0.0f   => 0,
                > 0.0f => 1,
                    
                _ => 0
            };
        }
        
        public BoidsGPUWall GetWall()
        {
            var exitFaces = int3.zero;

            if( solidFace.x )
                exitFaces.x = GetAxisIndex( direction.x );
            if( solidFace.y )
                exitFaces.y = GetAxisIndex( direction.y );
            if( solidFace.z )
                exitFaces.z = GetAxisIndex( direction.z );
            
            // TODO: Actually use the cache
            wallCache = new BoidsGPUWall()
            {
                Min       = boxCollider.bounds.min,
                Max       = boxCollider.bounds.max,
                Force     = direction * factor,
                ExitFaces = exitFaces,
            };
            
            return wallCache;
        }

        public void AddSelf()
        {
            OwnData = GetWall();
            AddSelfToManager();
        }

        public void RemoveSelf()
        {
            RemoveSelfFromManager();
        }
        
    #if UNITY_EDITOR
        private void Update()
        {
            // Force this system to update.
            if( gameObject.activeInHierarchy )
                AddSelf();
            
            if( transform.rotation == Quaternion.identity )
                return;
            
            rotation = transform.rotation * rotation;
            direction = rotation * Vector3.forward;
            
            transform.rotation = Quaternion.identity;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine( transform.position, transform.position + direction * factor );
        }

        private void OnValidate()
        {
            direction.Normalize();
            rotation = Quaternion.LookRotation( direction, Vector3.up );
        }
    #endif

    }
}