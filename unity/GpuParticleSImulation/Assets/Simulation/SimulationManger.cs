using System;
using UnityEngine;
using Random = UnityEngine.Random;

struct Particle {
   public Vector3 position;
   public Vector3 velocity;
   public float mass;
   public int type;
}

public class SimulationManger : MonoBehaviour {
   private enum ParticleBehavior {
      Gravity,
      ParticleLife
   };
   
   [Header("Simulation")]
   [SerializeField] private Mesh mesh;
   [SerializeField] private Material material;
   [SerializeField] private ComputeShader computeShader;
   [SerializeField] private ParticleBehavior particleBehavior;
   [SerializeField] private int particleCount = 100;
   [SerializeField] private float radius = 1.0f;
   [SerializeField] private float simulationSize = 10;
   [SerializeField] private uint initialSpeed;
   [SerializeField] private float TimeScale = 1; 
   [SerializeField] private int types;
   
   [Header("Gravity settings")]
   [SerializeField] private float softening = 0.5f;
   [SerializeField] private float G;

   [Header("Particle Life settings")] 
   [SerializeField] private float frictionHalfLife = 0.004f;
   [SerializeField] private float rMax;
   [SerializeField] private float forceFactor;

   private float[] attractionMatrix;
   private float frictionFactor;
   private float dt = 0.02f;
   
   private int gravityKernelHandle;
   private int particleLifeKernelHandle;
   private uint[] args = new uint[5];
   private ComputeBuffer argsBuffer;
   private ComputeBuffer particleReadBuffer;
   private ComputeBuffer particleWriteBuffer;
   private ComputeBuffer matrixReadBuffer;

   private void Start() {
      attractionMatrix = makeRandomMatrix();
      frictionFactor = Mathf.Pow(0.5f, dt / frictionHalfLife);
      
      gravityKernelHandle = computeShader.FindKernel("ComputeGravity");
      particleLifeKernelHandle = computeShader.FindKernel("ComputeParticleLifeForces");
      
      args[0] = mesh.GetIndexCount(0);
      args[1] = (uint)particleCount;
      args[2] = mesh.GetIndexStart(0);
      args[3] = mesh.GetBaseVertex(0);
      args[4] = 0;

      InitializeComputeBuffers();
      
      Particle[] particles = new Particle[particleCount];

      for (int i = 0; i < particleCount; i++) {
         particles[i].position = Random.insideUnitSphere * simulationSize;
         particles[i].velocity = Random.insideUnitSphere * initialSpeed;
         particles[i].mass = Random.Range(0.2f, 1f);
         particles[i].type = Random.Range(0, types);
      }

      
      
      argsBuffer.SetData(args);
      particleReadBuffer.SetData(particles);
      matrixReadBuffer.SetData(attractionMatrix);
      
      material.SetBuffer("particleBuffer", particleReadBuffer);
      material.SetFloat("_Radius", radius);
      material.SetInt("nTypes", types);
      
      computeShader.SetBuffer(gravityKernelHandle, "particleReadBuffer", particleReadBuffer);
      computeShader.SetBuffer(gravityKernelHandle, "particleWriteBuffer", particleWriteBuffer);
      
      computeShader.SetBuffer(particleLifeKernelHandle, "particleReadBuffer", particleReadBuffer);
      computeShader.SetBuffer(particleLifeKernelHandle, "particleWriteBuffer", particleWriteBuffer);
      computeShader.SetBuffer(particleLifeKernelHandle, "attractionMatrix", matrixReadBuffer);
      
      computeShader.SetInt("nParticles", particleCount);
      computeShader.SetFloat("softening", softening);
      computeShader.SetFloat("G", G);
      computeShader.SetFloat("rMax", rMax);
      computeShader.SetFloat("frictionFactor", frictionFactor);
      computeShader.SetFloat("forceFactor", forceFactor);
      computeShader.SetFloat("nTypes", types);

   }

   private void Update() {
      DispatchComputeShaders();
      
      material.SetFloat("_Radius", radius);
      material.SetInt("nTypes", types);
      computeShader.SetFloat("deltaTime", Time.deltaTime * TimeScale);
      computeShader.SetFloat("softening", softening);
      computeShader.SetFloat("G", G);
      computeShader.SetFloat("rMax", rMax);
      computeShader.SetFloat("frictionFactor", frictionFactor);
      computeShader.SetFloat("forceFactor", forceFactor);
      computeShader.SetFloat("nTypes", types);


      
      Graphics.DrawMeshInstancedIndirect(
         mesh,
         0,
         material,
         new Bounds(Vector3.zero, Vector3.one * 10000f),
         argsBuffer
      );
      
      SwapParticleBuffers();
   }

   private void DispatchComputeShaders() {
      switch (particleBehavior) {
         case ParticleBehavior.Gravity:
            computeShader.Dispatch(gravityKernelHandle, Mathf.CeilToInt(particleCount / 128f), 1, 1);
            break;
         default:
            computeShader.Dispatch(particleLifeKernelHandle, Mathf.CeilToInt(particleCount / 128f), 1, 1);
            break;
      }
   }

   private void InitializeComputeBuffers() {
      argsBuffer = new ComputeBuffer(
         1,
         args.Length * sizeof(uint),
         ComputeBufferType.IndirectArguments
      );
      particleReadBuffer = new ComputeBuffer(
         particleCount,
         sizeof(float) * 6 + sizeof(int) + sizeof(float)
      );
      particleWriteBuffer = new ComputeBuffer(
         particleCount,
         sizeof(float) * 6 + sizeof(int) + sizeof(float)
      );
      matrixReadBuffer = new ComputeBuffer(
         types * types,
         sizeof(float)
      );   
   }

   private void SwapParticleBuffers() {
      ComputeBuffer temp = particleReadBuffer;
      particleReadBuffer = particleWriteBuffer;
      particleWriteBuffer = temp;
      
      material.SetBuffer("particleBuffer", particleReadBuffer);
      switch (particleBehavior) {
         case ParticleBehavior.Gravity:
            computeShader.SetBuffer(gravityKernelHandle, "particleReadBuffer", particleReadBuffer);
            computeShader.SetBuffer(gravityKernelHandle, "particleWriteBuffer", particleWriteBuffer);
            break;
         case ParticleBehavior.ParticleLife:
            computeShader.SetBuffer(particleLifeKernelHandle, "particleReadBuffer", particleReadBuffer);
            computeShader.SetBuffer(particleLifeKernelHandle, "particleWriteBuffer", particleWriteBuffer);
            break;
      }
      
   }

   // Make the attraction matrix for particle life
   private float[] makeRandomMatrix() {
      float[] matrix = new float[types * types];
      for (int row = 0; row < types; row++) {
         for (int col = 0; col < types; col++) {
            int index = row * types + col;
            matrix[index] = Random.Range(-1.0f, 1.0f);
         }
      }
      return matrix;
   }
   
   private void OnDestroy() {
      // Release compute buffers
      if (enabled) {
         argsBuffer.Release();
         particleReadBuffer.Release();
         particleWriteBuffer.Release();
         matrixReadBuffer.Release();
      }
   }
}
