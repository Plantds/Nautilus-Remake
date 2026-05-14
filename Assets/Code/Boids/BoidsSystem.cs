using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace Boids
{
    [ ExecuteInEditMode ]
    [ RequireComponent( typeof( VisualEffect ) ) ]
    public class BoidsSystem : MonoBehaviour, IDisposable
    {
        // Particle Buffer
        private static readonly int PrevParticleBuffer = Shader.PropertyToID( "prevParticleBuffer" );
        private static readonly int ParticleBuffer     = Shader.PropertyToID( "particleBuffer" );
        private static readonly int ParticleCount      = Shader.PropertyToID( "particleCount" );
        private static readonly int ParticleOffset     = Shader.PropertyToID( "particleOffset" );
    
        // Target Buffer
        private static readonly int TargetBuffer = Shader.PropertyToID( "targetBuffer" );
        private static readonly int TargetCount  = Shader.PropertyToID( "targetCount" );
    
        // Wall Buffer
        private static readonly int WallBuffer = Shader.PropertyToID( "wallBuffer" );
        private static readonly int WallCount  = Shader.PropertyToID( "wallCount" );
    
        // Times
        private static readonly int TimeHash       = Shader.PropertyToID( "time" );
        private static readonly int DeltaTime      = Shader.PropertyToID( "deltaTime" );
    
        // Speeds
        private static readonly int MinSpeed = Shader.PropertyToID( "minSpeed" );
        private static readonly int MaxSpeed = Shader.PropertyToID( "maxSpeed" );
        private static readonly int Drag     = Shader.PropertyToID( "drag" );
    
        // Ranges
        private static readonly int VisualRange       = Shader.PropertyToID( "visualRange" );
        private static readonly int ProtectedRange    = Shader.PropertyToID( "protectedRangeMin" );
        private static readonly int ProtectedRangeMax = Shader.PropertyToID( "protectedRangeMax" );
        private static readonly int VisualRange2      = Shader.PropertyToID( "visualRange2" );
    
        // Factors
        private static readonly int CenterFactor   = Shader.PropertyToID( "centerFactor" );
        private static readonly int MatchingFactor = Shader.PropertyToID( "matchingFactor" );
        private static readonly int AvoidFactor    = Shader.PropertyToID( "avoidFactor" );
        private static readonly int TargetFactor   = Shader.PropertyToID( "targetFactor" );
    
        // Target
        private static readonly int TargetPosition     = Shader.PropertyToID( "targetPosition" );
        private static readonly int PlayerTargetFactor = Shader.PropertyToID( "playerTargetFactor" );
    
        // Octree
        private static readonly int TreeTexture = Shader.PropertyToID( "treeTexture" );
    
        // VFX
        private static readonly int VFXBoidsPrevBuffer = Shader.PropertyToID( "Boids Prev Buffer" );
        private static readonly int VFXBoidsCurrBuffer = Shader.PropertyToID( "Boids Buffer" );
        private static readonly int VFXBoidsAmount     = Shader.PropertyToID( "Boids Amount" );
        private static readonly int VFXBoidsProgress   = Shader.PropertyToID( "Boids Progress" );
    
        public bool isSelected { get; private set; }
    
        [ VFXType( VFXTypeAttribute.Usage.GraphicsBuffer ) ]
        private struct BoidsParticle
        {
            public Vector3 Position;
            public Vector3 Velocity;
        
            public const int Size = sizeof( float ) * 6;
        }
    
        [ Header( "System Settings" ) ]
        [ SerializeField ]
        private uint particleCount = 1000;
        private uint prevParticleCount;
        [ SerializeField ]
        private ComputeShader shader;
        [ SerializeField ]
        private string        kernelName = "CSMain";
        [ SerializeField ]
        private bool          customInitialization = false;
    
        [ Header( "Targets" ) ]
        public Transform globalTarget;
        public bool      useTargets = true;
    
        [ Header( "Speed" ) ]
        [ SerializeField ]
        private float   minSpeed = 0.2f;
        [ SerializeField ]
        private float   maxSpeed = 1.0f;
        [ SerializeField ]
        private float   drag     = 0.1f;
    
        [ Header( "Range" ) ]
        [ SerializeField ]
        private float   visualRange = 1.0f;
        [ FormerlySerializedAs( "protectedRange" ) ]
        public  float   protectedRangeMin = 0.5f;
        [ SerializeField ]
        public  float   protectedRangeMax = 0.5f;
    
        [ Header( "Factors" ) ]
        [ SerializeField ]
        private float   avoidFactor = 0.05f;
        [ SerializeField ]
        private float   centerFactor = 0.0005f;
        [ SerializeField ]
        private float   matchingFactor = 0.05f;
        [ SerializeField ]
        [ FormerlySerializedAs( "playerTargetFactor" ) ]
        public  float   targetFactor = 0.5f;
    
        [ Header( "Updates" ) ]
        [ Tooltip( "Weather or not the boids will be moving." ) ]
        public  bool   running     = true;
        [ SerializeField ]
        [ Tooltip( "How many seconds the system will have between each update" ) ]
        private float   updateDelay = 1;
        [ SerializeField ]
        private bool   onlyUseDelayInEditor = true;
        [ SerializeField ]
        private float  maxDeltaValue = 0.0f;
        [ SerializeField ]
        [ Tooltip( "How many additional smaller jobs the system should do between updates." ) ]
        private uint   jobsBetweenUpdates = 0;
    
        private bool   isDelayed => ( !onlyUseDelayInEditor || !Application.isPlaying ) && updateDelay > 0.0f;
    
        private VisualEffect  visualEffect;
        private int           addedInLayer;
    
        private int            kernelID = 0;
        private GraphicsBuffer nextParticleBuffer;
        private GraphicsBuffer particleBuffer;
        private GraphicsBuffer prevParticleBuffer;

        private uint         threadX;
        private RenderParams rp;
    
        private Texture3D treeLookupTexture;

        private CommandBuffer commandBuffer;

        private void CreateTreeTexture()
        {
            treeLookupTexture = new Texture3D( 8, 8, 8, GraphicsFormat.R8G8B8A8_UNorm,
                TextureCreationFlags.DontInitializePixels | TextureCreationFlags.DontUploadUponCreate );
        }
    
        private void GenerateParticles( bool setData )
        {
            if( particleCount != prevParticleCount || particleBuffer == null || prevParticleBuffer == null )
            {
                prevParticleBuffer?.Dispose();
                prevParticleBuffer = new GraphicsBuffer( GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopyDestination,
                    ( int )particleCount, BoidsParticle.Size );

                particleBuffer?.Dispose();
                particleBuffer = new GraphicsBuffer( GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopyDestination,
                    ( int )particleCount, BoidsParticle.Size );

                nextParticleBuffer?.Dispose();
                nextParticleBuffer = new GraphicsBuffer( GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopySource,
                    ( int )particleCount, BoidsParticle.Size );
            }
        
            if( !setData )
                return;
        
            var particles = new BoidsParticle[ particleCount ];
            for( var i =  0; i < particleCount; i++ )
            {
                var x = Random.value * 2.0f - 1.0f;
                var y = Random.value * 2.0f - 1.0f;
                var z = Random.value * 2.0f - 1.0f;
            
                ref var particle = ref particles[ i ];
            
                particle.Position = transform.position;
                particle.Position = transform.TransformPoint( x, y, z );
                particle.Velocity = Random.onUnitSphere;
            }
        
            commandBuffer.SetBufferData( nextParticleBuffer, particles );
            commandBuffer.CopyBuffer( nextParticleBuffer, particleBuffer );
            commandBuffer.CopyBuffer( nextParticleBuffer, prevParticleBuffer );
        }

        [ ContextMenu( "Regenerate System" ) ]
        public void RegenerateSystem()
        {
            Init( true );
        }

        [ContextMenu( "Tmp" )]
        public void Tmp()
        {
            var targets = FindObjectsByType< BoidsInteractable >( FindObjectsInactive.Include, FindObjectsSortMode.None );

            foreach( var boidsInteractable in targets )
            {
                boidsInteractable.SystemLayers = 1;
            }
        }

        private void UpdateJobInfo()
        {
            if( jobsBetweenUpdates == 0 )
                return;
        
            jobTimer    = -1.0f;
            currentJob  = 0;
            particlesPerJob = ( int )( particleCount / ( jobsBetweenUpdates ) );
        }
    
        public void Init( bool executeCommands = false )
        {
            if( particleCount <= 0 )
            {
                visualEffect.enabled = false;
                return;
            }
        
            commandBuffer?.Release();
            commandBuffer = new CommandBuffer();
            commandBuffer.name = gameObject.name + ": Boids System";
        
            GenerateParticles( !customInitialization );
        
            CreateTreeTexture();

            RefreshKernel();
        
            // Update VFX
            if( visualEffect == null )
                visualEffect = GetComponent< VisualEffect >();
        
            if( !visualEffect.enabled )
                visualEffect.enabled = true;

            UpdateVFXBuffers();
            if( hasJobs )
                visualEffect.SetGraphicsBuffer( VFXBoidsCurrBuffer, nextParticleBuffer );
            visualEffect.SetFloat( VFXBoidsAmount, particleCount );
            visualEffect.SetFloat( VFXBoidsProgress, 1.0f );
        
            prevParticleCount = particleCount;
        
            updateTimer = -1.0f;
            UpdateJobInfo();
        
            visualEffect.Reinit();
            

            if( customInitialization )
            {
                waitForFrames = 2;
                return;
            }
        
            if( !executeCommands )
                return;

            Graphics.ExecuteCommandBuffer( commandBuffer );
        }
    
        private int GetGroupSize( int nrOfParticles )
        {
            return Mathf.CeilToInt( ( float )nrOfParticles / threadX );
        }
    
        private void RefreshKernel()
        {
            var tmpKernelId = shader.FindKernel( kernelName );

            try
            {
                shader.GetKernelThreadGroupSizes( tmpKernelId, out threadX, out _, out _ );
                kernelID = tmpKernelId;
            }
            catch
            {
                if( kernelID != -1 )
                {
                    kernelID = -1;
                    throw new Exception( "The kernel is invalid. Either you put in the wrong name or the compute shader failed to compile." );
                }
            }
        }

        private void OnEnable()
        {
            visualEffect = GetComponent< VisualEffect >();
        
            Init( true );
        }

        private void OnDisable()
        {
            Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        // THIS seems to prevent the memory leaks.
        ~BoidsSystem()
        {
            Dispose();
        }
    
        public void Dispose()
        {
            // Attempt nr 42069 at stopping the 3 memory leaks, but it doesn't work.
            // If you have any ideas Kjell pls do share them.
            // Note: The memory leaks don't increase per frame and will only happen once per runtime.
            
            BoidsManager.RemoveSystem( this );
            
            particleBuffer?.Dispose();
            particleBuffer = null;
            prevParticleBuffer?.Dispose();
            prevParticleBuffer = null;
            nextParticleBuffer?.Dispose();
            nextParticleBuffer = null;
        }
    
        private float updateTimer;

        private void UpdateVFXProgress()
        {
            if( !visualEffect.HasFloat( VFXBoidsProgress ) )
                return;
        
            var progress = hasJobs ? updateTimer / updateDelay : 1.0f - updateTimer / updateDelay;
            visualEffect.SetFloat( VFXBoidsProgress, progress );
        }

        private void UpdateVFXBuffers()
        {
            if( visualEffect.HasFloat( VFXBoidsProgress ) )
                visualEffect.SetFloat( VFXBoidsProgress, hasJobs ? 1.0f : 0.0f );
        
            if( visualEffect.HasGraphicsBuffer( VFXBoidsPrevBuffer ) )
                visualEffect.SetGraphicsBuffer( VFXBoidsPrevBuffer, prevParticleBuffer );
            visualEffect.SetGraphicsBuffer( VFXBoidsCurrBuffer,     particleBuffer );
        }

        private int   waitForFrames;
        private float jobTimer;
    
        private bool  hasJobs => jobsBetweenUpdates > 0 && updateDelay > 0.0f;
        private int   currentJob;
        private int   particlesPerJob;

#if UNITY_EDITOR
        private void Update()
        {
            if( !Application.isPlaying )
                FixedUpdate();
        }
#endif
    
        private void FixedUpdate()
        {
            if( kernelID == -1 || particleBuffer == null )
            {
                Init();
                return;
            }
        
            if( waitForFrames-- > 0 )
                return;
        
            if( !running )
                return;

            if( hasJobs && ( jobTimer -= Time.deltaTime ) <= 0.0f )
            {
                var delayPerJob = updateDelay / jobsBetweenUpdates;
                var jobs = Mathf.FloorToInt( -jobTimer / delayPerJob );
                for( var i = jobs; i >= 0 && currentJob < jobsBetweenUpdates; i-- )
                {
                    var particleStart = particlesPerJob * currentJob;
                    var particleEnd   = Mathf.Min( ( int )particleCount, particlesPerJob * ( currentJob + 1 ) );
            
                    DispatchGroup( Mathf.Min( updateDelay, maxDeltaValue ), nextParticleBuffer, particleStart, particleEnd );
                    currentJob++;
                }
            
                jobTimer = updateDelay / jobsBetweenUpdates;
            }

            // Allows the system to update every x frames.
            if( isDelayed && ( updateTimer -= Time.deltaTime ) > 0.0f )
            {
                UpdateVFXProgress();
                return;
            }

            currentJob = 0;

            updateTimer = updateDelay;
        
            var delta = isDelayed ? updateDelay : Time.deltaTime;
            if( isDelayed && maxDeltaValue != 0.0f && delta > maxDeltaValue )
                delta = maxDeltaValue;
        
            if( hasJobs )
                commandBuffer.CopyBuffer( nextParticleBuffer, particleBuffer );
        
            ( particleBuffer, prevParticleBuffer ) = ( prevParticleBuffer, particleBuffer );
        
            if( !hasJobs )
            {
                DispatchSystem( delta );
            }

            UpdateVFXBuffers();
        
            Graphics.ExecuteCommandBuffer( commandBuffer );
            commandBuffer.Clear();
        }

        private void SetIntParam( int nameID, int value )
        {
            commandBuffer.SetComputeIntParam( shader, nameID, value );
        }

        private void SetFloatParam( int nameID, float value )
        {
            commandBuffer.SetComputeFloatParam( shader, nameID, value );
        }

        private void SetVectorParam( int nameID, Vector4 value )
        {
            commandBuffer.SetComputeVectorParam( shader, nameID, value );
        }

        private void SetBufferParam( int kernelIndex, int nameID, GraphicsBuffer value )
        {
            commandBuffer.SetComputeBufferParam( shader, kernelIndex, nameID, value );
        }

        private void DispatchSystem( float delta )
        {
            DispatchGroup( delta, particleBuffer, 0, ( int )particleCount );
        }
    
        private void DispatchGroup( float delta, GraphicsBuffer outputBuffer, int particleStart, int particleEnd )
        {
            using( new ProfilingScope( commandBuffer, new ProfilingSampler( "Boids System" ) ) )
            {
#if UNITY_EDITOR
                // Make sure the kernel is valid in case the file is updated.
                RefreshKernel();
                if( kernelID < 0 )
                    return;
#else
            if( kernelID < 0 )
                throw new Exception( "Provided compute shader is invalid." );
#endif
            
                var nrOfParticles = particleEnd - particleStart;
                
                var buffers = BoidsManager.GetInteractionBuffersInLayer( gameObject.layer, commandBuffer );

                int targetCount;
                buffers.TryGetValue( typeof( BoidsGPUTarget ), out var targetBuffer );
                if( targetBuffer == null )
                {
                    targetCount  = 0;
                    targetBuffer = new GraphicsBuffer( GraphicsBuffer.Target.Structured, 1, BoidsGPUTarget.Size );
                }
                else
                    targetCount = targetBuffer.count;
                
                int wallCount;
                buffers.TryGetValue( typeof( BoidsGPUWall ), out var wallBuffer );
                if( wallBuffer == null )
                {
                    wallCount  = 0;
                    wallBuffer = new GraphicsBuffer( GraphicsBuffer.Target.Structured, 1, BoidsGPUWall.Size );
                }
                else
                    wallCount = wallBuffer.count;
            
                // Set Counts
                SetIntParam( ParticleCount,  nrOfParticles );
                SetIntParam( ParticleOffset, particleStart );
                SetIntParam( TargetCount,    targetCount );
                SetIntParam( WallCount,      wallCount );

                // Set Time
                SetFloatParam( TimeHash,  Time.time );
                SetFloatParam( DeltaTime, delta );
            
                // Set Player Target
                if( globalTarget != null )
                {
                    SetVectorParam( TargetPosition, globalTarget.position );
                    SetFloatParam( PlayerTargetFactor, targetFactor );
                }
                else
                    SetFloatParam( PlayerTargetFactor, 0.0f );
            
                // Set Speed
                SetFloatParam( MinSpeed, minSpeed );
                SetFloatParam( MaxSpeed, maxSpeed );
                SetFloatParam( Drag,     drag );
            
                // Set Ranges
                SetFloatParam( VisualRange,       visualRange );
                SetFloatParam( ProtectedRange,    protectedRangeMin );
                SetFloatParam( ProtectedRangeMax, protectedRangeMax );
                SetFloatParam( VisualRange2,      Mathf.Pow( visualRange, 2 ) );
            
                // Set Factors
                SetFloatParam( AvoidFactor,    avoidFactor );
                SetFloatParam( CenterFactor,   centerFactor );
                SetFloatParam( MatchingFactor, matchingFactor );
                SetFloatParam( TargetFactor,   useTargets ? 1.0f : 0.0f );
            
                // Set Buffers
                SetBufferParam( kernelID, PrevParticleBuffer, prevParticleBuffer );
                SetBufferParam( kernelID, ParticleBuffer,     outputBuffer );
                SetBufferParam( kernelID, TargetBuffer, targetBuffer );
                SetBufferParam( kernelID, WallBuffer,   wallBuffer );
            
                // Set Octree (WIP)
                shader.SetTexture( kernelID, TreeTexture, treeLookupTexture );
            
                // This isn't the most effective usage of the gpu as we're discarding a bunch of threads each time.
                // Could be spaced out better in the future.
                // Dispatch
                commandBuffer.DispatchCompute( shader, kernelID, GetGroupSize( nrOfParticles ), 1, 1 );
                
                if( targetCount == 0 )
                    targetBuffer.Dispose();
                if( wallCount == 0 )
                    wallBuffer.Dispose();
            }
        }

        private void AddSelf()
        {
            if( addedInLayer == -1 )
                return;
            
            BoidsManager.AddSystem( this );
            addedInLayer = gameObject.layer;
        }

        private void RemoveSelf()
        {
            BoidsManager.RemoveSystem( this );
            addedInLayer = -1;
        }

        private void RefreshSelf()
        {
            RemoveSelf();
            AddSelf();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            kernelID = 0;
            if( particleCount != prevParticleCount )
                Init( true );
        
            if( particlesPerJob != ( int )( particleCount / ( jobsBetweenUpdates + 1 ) ) )
                UpdateJobInfo();

            updateDelay = Mathf.Clamp( updateDelay, 0.0f, 3600.0f );

            if( addedInLayer != gameObject.layer )
                RefreshSelf();
        }

        private void OnDrawGizmos()
        {
            isSelected = false;
        }

        private void OnDrawGizmosSelected()
        {
            isSelected = true;
        }
#endif
    }
}
