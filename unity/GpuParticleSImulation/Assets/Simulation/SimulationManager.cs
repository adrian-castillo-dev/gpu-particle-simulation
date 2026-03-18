using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationManger : MonoBehaviour {
   
   [Header("Simulation")]
   [SerializeField] private Mesh mesh;
   [SerializeField] private Material material;
   [SerializeField] private ComputeShader computeShader;
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

   private ParticleRenderer renderer;
   private ParticleBufferManager buffers;
   private ComputeBuffer matrixReadBuffer;

   private GravitySimulation gravitySimulation;

   private void Start()
   {
      buffers = new ParticleBufferManager(particleCount);
      renderer = new ParticleRenderer(mesh, material, particleCount);
      InitializeComputeBuffers();
      
      attractionMatrix = makeRandomMatrix(); // particle life's attraction matrix
      frictionFactor = Mathf.Pow(0.5f, dt / frictionHalfLife);
      
      Particle[] particles = new Particle[particleCount];

      for (int i = 0; i < particleCount; i++) {
         particles[i].position = Random.insideUnitSphere * simulationSize;
         particles[i].velocity = Random.insideUnitSphere * initialSpeed;
         particles[i].mass = Random.Range(0.2f, 1f);
         particles[i].type = Random.Range(0, types);
      }
      
      buffers.Read.SetData(particles);
      renderer.SetParticleBuffer(buffers.Read);
      matrixReadBuffer.SetData(attractionMatrix);

      SetMaterialSetupVariables();

      gravitySimulation = new GravitySimulation(computeShader, buffers);

   }

   private void Update() {
      gravitySimulation.Step(Time.deltaTime);
      
      renderer.SetParticleBuffer(buffers.Read);
      
      renderer.Render();
      
      SetMaterialRuntimeVariables();
   }

   private void InitializeComputeBuffers()
   {
      matrixReadBuffer = new ComputeBuffer( types * types, sizeof(float) );
   }

   private void SetMaterialRuntimeVariables(){
      material.SetFloat("_Radius", radius);
      material.SetInt("nTypes", types);
   }

   private void SetMaterialSetupVariables(){
      material.SetBuffer("particleBuffer", buffers.Read);
      material.SetFloat("_Radius", radius);
      material.SetInt("nTypes", types);
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
         renderer.Release();
         buffers.Release();
         matrixReadBuffer.Release();
      }
   }
}
