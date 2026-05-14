using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Boids
{
    public class BoidsManager : ScriptableObject
    {
        private static BoidsManager _instance;

        private static BoidsManager Instance
        {
            get
            {
                if( _instance == null )
                    _instance = CreateInstance< BoidsManager >();

                return _instance;
            }
            set => _instance = value;
        }
        
        private static void DestroyInstance()
        {
#if UNITY_EDITOR
            DestroyImmediate( Instance );
#else
            Destroy( Instance );
#endif
            Instance = null;
        }
        
        private static int[] GetLayersFrom( int layers )
        {
            List< int > tmpLayers = new();
            for( var i = 0; i < sizeof( int ); i++ )
            {
                if( ( layers & ( 1 >> i ) ) != 0 )
                    tmpLayers.Add( i );
            }
            
            return tmpLayers.ToArray();
        }
        
        public static void AddSystem( BoidsSystem system )
        {
            if( Instance == null )
                Instance = CreateInstance< BoidsManager >();
            
            Instance.AddSystem( system, system.gameObject.layer );
        }

        public static void RemoveSystem( BoidsSystem system )
        {
            Instance.RemoveSystem( system, system.gameObject.layer );
        }

        public static void AddInteractable( BoidsInteractable interactable )
        {
            Instance.AddInteractable( interactable, GetLayersFrom( interactable.SystemLayers ) );
        }

        public static void RemoveInteractable( BoidsInteractable interactable )
        {
            Instance.RemoveInteractable( interactable, GetLayersFrom( interactable.SystemLayers ) );
        }

        public static Dictionary< Type, GraphicsBuffer > GetInteractionBuffersInLayer( int layer, CommandBuffer commandBuffer )
        {
            return Instance.GetGraphicsBuffersInLayer( layer, commandBuffer );
        }

        public static BoidsSystem[] GetSystemsInLayers( LayerMask layers )
        {
            return Instance == null ? null : Instance.GetSystemsByLayers( GetLayersFrom( layers ) );
        }
        
        private class InteractableCollection
        {
            public class BufferInfo
            {
                public GraphicsBuffer Buffer;
                public Array          Data;
                public Type           ElementType;
                // The capacity is Data.Length
                public int            Size;
                public readonly List< BoidsInteractable > Backup = new();         
                public readonly Dictionary< BoidsInteractable, int > IndexMap = new();
            }
            
            public readonly HashSet< BoidsInteractable >         InteractableSet = new();
            public readonly Dictionary< Type, BufferInfo >       GraphicsBuffers = new();
            
            public bool IsUpdated;

            private static void Resize( ref BufferInfo info, int newSize, bool allowCopy = true )
            {
                // Using the next power of 2 as the capacity will save us plenty of reallocations.
                // We will start with 4 elements as the minimum.
                var newSizeAligned = Mathf.Max( 4, Mathf.NextPowerOfTwo( newSize ) );

                if( info.Data == null || newSize != info.Data.Length )
                {
                    var newArray = Array.CreateInstance( info.ElementType, newSizeAligned );

                    if( allowCopy && info.Data != null )
                        Array.Copy( info.Data, newArray, Mathf.Min( info.Size, newSize ) );
                    
                    info.Data = newArray;
                }
                
                info.Size = newSize;
            }
            
            private static int AddElement( ref BufferInfo info, object element )
            {
                var prevSize = info.Size;
                
                Resize( ref info, info.Size + 1 );
                info.Data.SetValue( element, prevSize );
                
                return prevSize;
            }

            private static void RemoveElement( ref BufferInfo info, int elementIndex )
            {
                if( elementIndex < 0 )
                    return;
                
                var backup = ( Array )info.Data.Clone();
                Resize( ref info, info.Size - 1 );
                
                // If it's the first element we won't need to copy the element beforehand.
                if( elementIndex != 0 )
                    Array.Copy( backup, 0, info.Data, 0, elementIndex );
                
                Array.Copy( backup, elementIndex + 1, info.Data, elementIndex, info.Size - elementIndex );
            }

            /// <summary>
            /// replaces the element at the index with the last element in the array.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="elementIndex"></param>
            private static void RemoveElementFast( ref BufferInfo info, int elementIndex )
            {
                if( elementIndex < 0 )
                    return;

                if( info.Size - 1 < elementIndex )
                {
                    var backup = info.Data.GetValue( info.Size - 1 );
                    Resize( ref info, info.Size - 1 );
                    
                    info.Data.SetValue( backup, elementIndex );
                }
                else
                    Resize( ref info, info.Size - 1 );
            }

            public void Add( BoidsInteractable interactable )
            {
                if( !GraphicsBuffers.ContainsKey( interactable.BufferElementType ) )
                {
                    GraphicsBuffers[ interactable.BufferElementType ] = new BufferInfo()
                    {
                        Buffer = null,
                        Data   = null,
                        ElementType = interactable.BufferElementType,
                        Size = 0
                    };
                }
                
                var bufferInfo = GraphicsBuffers[ interactable.BufferElementType ];
                if( InteractableSet.Add( interactable ) )
                {
                    // Insert the element in the back in case it's added
                    var index = AddElement( ref bufferInfo, interactable.OwnData );
                    bufferInfo.IndexMap[ interactable ] = index;
                    bufferInfo.Backup.Add( interactable );
                }
                else
                {
                    // Otherwise update its values
                    bufferInfo.Data.SetValue( interactable.OwnData, bufferInfo.IndexMap[ interactable ] );
                }
                GraphicsBuffers[ interactable.BufferElementType ] = bufferInfo;

                // As we update the element when it's already added, this will always require an update.
                // The user will have to ensure that things aren't getting updated too often.
                IsUpdated = true;
            }

            public void Remove( BoidsInteractable interactable )
            {
                if( !InteractableSet.Remove( interactable ) )
                    return;
                
                var bufferInfo = GraphicsBuffers[ interactable.BufferElementType ];
                var index = bufferInfo.IndexMap[ interactable ];
                
                RemoveElementFast( ref bufferInfo, index );
                bufferInfo.IndexMap.Remove( interactable );

                if( index != bufferInfo.Size )
                {
                    var newElement = bufferInfo.Backup[ ^1 ];
                    bufferInfo.IndexMap[ newElement ] = index;
                    bufferInfo.Data.SetValue( newElement.OwnData, index );
                    bufferInfo.Backup.RemoveAt( bufferInfo.Backup.Count -1 );
                    bufferInfo.Backup[ index ] = newElement;
                }
                else
                {
                    bufferInfo.Backup.RemoveAt( bufferInfo.Backup.Count -1 );
                }
                
                IsUpdated = true;
            }

            public void UpdateGraphicsBuffers( CommandBuffer commandBuffer )
            {
                if( !IsUpdated )
                    return;
                
                IsUpdated = false;

                foreach( var bufferInfo in GraphicsBuffers.Values )
                {
                    if( bufferInfo.Buffer == null || bufferInfo.Buffer.count != bufferInfo.Size )
                    {
                        bufferInfo.Buffer?.Dispose();
                        bufferInfo.Buffer = new GraphicsBuffer( GraphicsBuffer.Target.Structured,
                            Mathf.Max( 1, bufferInfo.Size ), UnsafeUtility.SizeOf( bufferInfo.ElementType ) );
                    }
                    commandBuffer.SetBufferData( bufferInfo.Buffer, bufferInfo.Data, 0, 0, bufferInfo.Size );
                }
            }

            public void Dispose()
            {
                foreach( var bufferInfo in GraphicsBuffers.Values )
                {
                    bufferInfo.Buffer?.Dispose();
                }
            }
        }
        
        private readonly Dictionary< int, HashSet< BoidsSystem > > _systemMap       = new();
        private readonly Dictionary< int, InteractableCollection > _interactableMap = new();

        private void DeleteIfEmpty()
        {
            if( _systemMap.Count == 0 && _interactableMap.Count == 0 )
                DestroyInstance();
        }
        
        private HashSet< BoidsSystem > GetSystems( int layer )
        {
            if( !_systemMap.ContainsKey( layer ) ) 
                _systemMap[ layer ] = new HashSet< BoidsSystem >();

            return _systemMap[ layer ];
        }

        private InteractableCollection GetInteractableCollection( int layer )
        {
            if( !_interactableMap.ContainsKey( layer ) ) 
                _interactableMap[ layer ] = new InteractableCollection();
            
            return _interactableMap[ layer ];
        }

        private void AddSystem( BoidsSystem system, int layer )
        {
            GetSystems( layer ).Add( system );
        }

        private void RemoveSystem( BoidsSystem system, int layer )
        {
            var systems = GetSystems( layer );
            
            if( !systems.Remove( system ) )
                return;
            
            if( systems.Count == 0 )
                _systemMap.Remove( layer );

            DeleteIfEmpty();
        }

        private void AddInteractable( BoidsInteractable interactable, int[] layers )
        {
            foreach( var layer in layers )
                GetInteractableCollection( layer ).Add( interactable );
        }

        private void RemoveInteractable( BoidsInteractable interactable, int[] layers )
        {
            foreach( var layer in layers )
            {
                var interactableCollection = GetInteractableCollection( layer );

                interactableCollection.Remove( interactable );

                if( interactableCollection.InteractableSet.Count != 0 )
                    continue;
                
                interactableCollection.Dispose();
                _interactableMap.Remove( layer );
            }
            
            DeleteIfEmpty();
        }

        private Dictionary< Type, GraphicsBuffer > GetGraphicsBuffersInLayer( int layer, CommandBuffer commandBuffer )
        {
            var collection = GetInteractableCollection( layer );

            collection.UpdateGraphicsBuffers( commandBuffer );
            
            var dictionary = collection.GraphicsBuffers.ToDictionary(
                bufferInfoPairs => bufferInfoPairs.Key,
                bufferInfoPairs => bufferInfoPairs.Value.Buffer
            );
            
            return dictionary;
        }

        private BoidsSystem[] GetSystemsByLayers( int[] layers )
        {
            List< BoidsSystem > tmpSystems = new();

            foreach( var layer in layers )
                tmpSystems.AddRange( GetSystems( layer ) );
            
            return tmpSystems.ToArray();
        }
    }
}