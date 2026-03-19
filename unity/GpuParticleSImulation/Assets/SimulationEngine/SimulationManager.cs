using System;
using UnityEngine;
using Random = UnityEngine.Random;
using SimulationEngine.Rendering;
using SimulationEngine.Core;
using SimulationEngine.Settings;
using SimulationEngine.Simulations;

namespace SimulationEngine
{
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
      [Header("Gravity settings")]
      [SerializeField] private GravitySettings gravitySettings;

      [Header("Particle Life settings")] 
      [SerializeField] private ParticleLifeSettings particleLifeSettings;
      
      private ParticleRenderer renderer;
      private ParticleBufferManager buffers;

      // Simulations
      private GravitySimulation gravitySimulation;
      private ParticleLifeSimulation particleLifeSimulation;

      private void Start()
      {
         buffers = new ParticleBufferManager(particleCount);
         renderer = new ParticleRenderer(mesh, material, particleCount);
         
         Particle[] particles = new Particle[particleCount];

         for (int i = 0; i < particleCount; i++) {
            particles[i].position = Random.insideUnitSphere * simulationSize;
            particles[i].velocity = Random.insideUnitSphere * initialSpeed;
            particles[i].mass = Random.Range(0.2f, 1f);
            particles[i].type = Random.Range(0, particleLifeSettings.types);
         }
         
         buffers.Read.SetData(particles);
         renderer.SetParticleBuffer(buffers.Read);

         SetMaterialSetupVariables();

         gravitySimulation = new GravitySimulation(computeShader, buffers, gravitySettings);
         particleLifeSimulation = new ParticleLifeSimulation(computeShader, buffers, particleLifeSettings);
         
         particleLifeSimulation.SetUp();

      }

      private void Update() {
         particleLifeSimulation.Step(Time.deltaTime);
         
         renderer.SetParticleBuffer(buffers.Read);
         
         renderer.Render();
         
         SetMaterialRuntimeVariables();
      }
      

      private void SetMaterialRuntimeVariables(){
         material.SetFloat("_Radius", radius);
         material.SetInt("nTypes", particleLifeSettings.types);
      }

      private void SetMaterialSetupVariables(){
         material.SetBuffer("particleBuffer", buffers.Read);
         material.SetFloat("_Radius", radius);
         material.SetInt("nTypes", particleLifeSettings.types);
      }
      
      private void OnDestroy() {
         // Release compute buffers
         if (enabled) {
            renderer.Release();
            buffers.Release();
            particleLifeSimulation.Release();
         }
      }
   }
}

