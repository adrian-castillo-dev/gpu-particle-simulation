using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
   private ParticleBufferManager buffers;
   private ComputeBuffer matrixReadBuffer;

   private void Start()
   {
      buffers = new ParticleBufferManager(particleCount);
      InitializeComputeBuffers();
      
      gravityKernelHandle = computeShader.FindKernel("ComputeGravity");
      particleLifeKernelHandle = computeShader.FindKernel("ComputeParticleLifeForces");

      // Rendering args
      args[0] = mesh.GetIndexCount(0);
      args[1] = (uint)particleCount;
      args[2] = mesh.GetIndexStart(0);
      args[3] = mesh.GetBaseVertex(0);
      args[4] = 0;

      attractionMatrix = makeRandomMatrix(); // particle life's attraction matrix
      frictionFactor = Mathf.Pow(0.5f, dt / frictionHalfLife);
      
      Particle[] particles = new Particle[particleCount];

      for (int i = 0; i < particleCount; i++) {
         particles[i].position = Random.insideUnitSphere * simulationSize;
         particles[i].velocity = Random.insideUnitSphere * initialSpeed;
         particles[i].mass = Random.Range(0.2f, 1f);
         particles[i].type = Random.Range(0, types);
      }
      
      argsBuffer.SetData(args);
      buffers.Read.SetData(particles);
      matrixReadBuffer.SetData(attractionMatrix);

      SetComputeSetupVariables();
      SetMaterialSetupVariables();
   
   }

   private void Update() {
      DispatchComputeShaders();
      
      SetMaterialRuntimeVariables();
      SetComputeRuntimeVariables();
      
      Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one * 10000f), argsBuffer);

      SwapParticleBuffers();
   }

   private void InitializeComputeBuffers()
   {
      argsBuffer = new ComputeBuffer( 1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments );
      matrixReadBuffer = new ComputeBuffer( types * types, sizeof(float) );
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

   private void SetMaterialRuntimeVariables(){
      material.SetFloat("_Radius", radius);
      material.SetInt("nTypes", types);
   }

   private void SetComputeRuntimeVariables(){
      computeShader.SetFloat("deltaTime", Time.deltaTime * TimeScale);
      computeShader.SetFloat("softening", softening);
      computeShader.SetFloat("G", G);
      computeShader.SetFloat("rMax", rMax);
      computeShader.SetFloat("frictionFactor", frictionFactor);
      computeShader.SetFloat("forceFactor", forceFactor);
      computeShader.SetFloat("nTypes", types);
   }

   private void SetComputeSetupVariables(){
      computeShader.SetBuffer(gravityKernelHandle, "particleReadBuffer", buffers.Read);
      computeShader.SetBuffer(gravityKernelHandle, "particleWriteBuffer", buffers.Write);
      
      computeShader.SetBuffer(particleLifeKernelHandle, "particleReadBuffer", buffers.Read);
      computeShader.SetBuffer(particleLifeKernelHandle, "particleWriteBuffer", buffers.Write);
      computeShader.SetBuffer(particleLifeKernelHandle, "attractionMatrix", matrixReadBuffer);
      
      computeShader.SetInt("nParticles", particleCount);
      computeShader.SetFloat("softening", softening);
      computeShader.SetFloat("G", G);
      computeShader.SetFloat("rMax", rMax);
      computeShader.SetFloat("frictionFactor", frictionFactor);
      computeShader.SetFloat("forceFactor", forceFactor);
      computeShader.SetFloat("nTypes", types);
   }

   private void SetMaterialSetupVariables(){
      material.SetBuffer("particleBuffer", buffers.Read);
      material.SetFloat("_Radius", radius);
      material.SetInt("nTypes", types);
   }
   

   private void SwapParticleBuffers() {
      buffers.Swap();
      
      material.SetBuffer("particleBuffer", buffers.Read);
      switch (particleBehavior) {
         case ParticleBehavior.Gravity:
            computeShader.SetBuffer(gravityKernelHandle, "particleReadBuffer", buffers.Read);
            computeShader.SetBuffer(gravityKernelHandle, "particleWriteBuffer", buffers.Write);
            break;
         case ParticleBehavior.ParticleLife:
            computeShader.SetBuffer(particleLifeKernelHandle, "particleReadBuffer", buffers.Read);
            computeShader.SetBuffer(particleLifeKernelHandle, "particleWriteBuffer", buffers.Write);
            break;
      }
      
   }

   // Returns a random matrix based on the number of types for the particle life simulation
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
         buffers.Read.Release();
         buffers.Write.Release();
         matrixReadBuffer.Release();
      }
   }
}
