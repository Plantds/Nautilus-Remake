using System;
using UnityEngine;

namespace Boids
{
    public abstract class BoidsInteractable : MonoBehaviour
    {
        [ Header( "System", order = 10 ) ]
        [ SerializeField ]
        [ Tooltip( "The layers that has the systems that this target will add itself to." ) ]
        private LayerMask systemLayers = 1;
        
        protected bool IsAdded { get; private set; }
        
        public abstract Type BufferElementType { get; }
        
        public object OwnData { get; protected set; }
        
        public LayerMask SystemLayers
        {
            get => systemLayers;
            set
            {
                if( value == systemLayers )
                    return;
                
                systemLayers = value;
                
                RemoveSelfFromManager();
                AddSelfToManager();
            }
        }

        protected void AddSelfToManager()
        {
            if( !( gameObject.activeInHierarchy && enabled ) )
            {
                RemoveSelfFromManager();
                return;
            }
            
            BoidsManager.AddInteractable( this );
            IsAdded = true;
        }

        protected void RemoveSelfFromManager()
        {
            if( !IsAdded )
                return;
            
            BoidsManager.RemoveInteractable( this );
            IsAdded = false;
        }
    }
}