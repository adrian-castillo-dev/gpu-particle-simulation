using System;
using UnityEngine;
using Random = UnityEngine.Random;
using SimulationEngine.Rendering;
using SimulationEngine.Core;
using SimulationEngine.Settings;
using SimulationEngine.Simulations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace SimulationEngine
{
   public class SimulationManger : MonoBehaviour {

      private enum ParticleBehavior
      {
         Gravity,
         ParticleLife,
         Boids
      }
     
      [Header("Simulation")]
      [SerializeField] private Mesh mesh;
      [SerializeField] private Material material;
      [SerializeField] private ComputeShader computeShader;
      [SerializeField] private int particleCount = 100;
      [SerializeField] private float radius = 1.0f;
      [SerializeField] private float simulationSize = 10;
      [SerializeField] private float initialSpeed;
      [SerializeField] private ParticleBehavior particleBehavior;
      
      [Header("Gravity settings")]
      [SerializeField] private GravitySettings gravitySettings;

      [Header("Particle Life settings")] 
      [SerializeField] private ParticleLifeSettings particleLifeSettings;
      
      [Header("Boids Settings")]
      [SerializeField] private BoidsSettings boidsSettings;
      
      private ParticleRenderer renderer;
      private ParticleBufferManager buffers;

      // Simulations
      private GravitySimulation gravitySimulation;
      private ParticleLifeSimulation particleLifeSimulation;
      private BoidsSimulation boidsSimulation;
      
      Particle[] readParticles;

      private void Start()
      {
         buffers = new ParticleBufferManager(particleCount);
         renderer = new ParticleRenderer(mesh, material, particleCount);
         SetMaterialSetupVariables();
         InitializeParticles();
         
         renderer.SetParticleBuffer(buffers.Read);
         // Create Simulations
         gravitySimulation = new GravitySimulation(computeShader, buffers, gravitySettings);
         particleLifeSimulation = new ParticleLifeSimulation(computeShader, buffers, particleLifeSettings);
         boidsSimulation = new BoidsSimulation(computeShader, buffers, boidsSettings);
         // Start Simulations
         gravitySimulation.SetUp();
         particleLifeSimulation.SetUp();
         boidsSimulation.SetUp();
         
         readParticles = new Particle[particleCount];
      }

      private void Update() {
         // Changes the particle behavior at runtime
         switch (particleBehavior) {
            case ParticleBehavior.Gravity:
               gravitySimulation.Step(Time.deltaTime);
               break;
            case ParticleBehavior.ParticleLife:
               particleLifeSimulation.Step(Time.deltaTime);
               break;
            case ParticleBehavior.Boids:
               boidsSimulation.Step(Time.deltaTime);
               buffers.Read.GetData(readParticles);
               break;
         }
         
         // Render Particles
         renderer.SetParticleBuffer(buffers.Read);
         renderer.Render();
         
         SetMaterialRuntimeVariables();
      }

      // Material variables to update at runtime
      private void SetMaterialRuntimeVariables(){
         material.SetFloat("_Radius", radius);
         material.SetInt("nTypes", particleLifeSettings.types);
      }
      
      // Material variables to set once
      private void SetMaterialSetupVariables(){
         material.SetBuffer("particleBuffer", buffers.Read);
         material.SetFloat("_Radius", radius);
         material.SetInt("nTypes", particleLifeSettings.types);
      }

      // Generates an array of particles with randomized positions
      private void InitializeParticles()
      {
         Particle[] particles = new Particle[particleCount];

         for (int i = 0; i < particleCount; i++) {
            particles[i].position = Random.insideUnitSphere * simulationSize;
            particles[i].velocity = Random.insideUnitSphere * initialSpeed;
            //particles[i].direction = Vector3.Normalize(Random.insideUnitSphere * 100);
            particles[i].direction = new Vector3(0, 0, 0);
            particles[i].mass = Random.Range(0.2f, 1f);
            particles[i].type = Random.Range(0, particleLifeSettings.types);
         }
         
         buffers.Read.SetData(particles);
      }
      
      private void OnDestroy() {
         // Release compute buffers
         if (enabled) {
            renderer.Release();
            buffers.Release();
            particleLifeSimulation.Release();
         }
      }

      void OnDrawGizmos()
      {
         if (readParticles != null)
         {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(readParticles[5].position, (readParticles[5].direction * 5) + readParticles[5].position);
            for (int i = 0; i < buffers.ParticleCount; i++)
            {
               if (i == 5) continue;
               if (math.length(readParticles[i].position - readParticles[5].position) <= boidsSettings.range / 2)
               {
                  Gizmos.color = Color.red;
                  Gizmos.DrawLine(readParticles[5].position, readParticles[i].position);
               }
               else if (math.length(readParticles[i].position - readParticles[5].position) <= boidsSettings.range)
               {
                  Gizmos.color = Color.green;
                  Gizmos.DrawLine(readParticles[5].position, readParticles[i].position);
               }
            }
         }
      }
   }
}

