using SimulationEngine.Core;
using SimulationEngine.Settings;
using UnityEngine;

namespace SimulationEngine.Simulations
{
    public class BoidsSimulation : ParticleSimulation<BoidsSettings>
    {
        private int kernel;
        private float time = 0;
        
        public BoidsSimulation(ComputeShader shader, ParticleBufferManager buffers, BoidsSettings settings) : base(shader, buffers, settings){}
        
        public override void SetUp()
        {
            kernel = shader.FindKernel("ComputeBoids");
            // Core buffers
            shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
            shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
            
            // Boids settings
            shader.SetFloat("boidsMinSpeed", settings.minSpeed);
            shader.SetFloat("boidsMaxSpeed", settings.maxSpeed);
            shader.SetFloat("boidsVisualRange", settings.visualRange);
            shader.SetFloat("boidsProtectedRange", settings.protectedRange);
            shader.SetFloat("boidsMatchingFactor", settings.matchingFactor);
            shader.SetFloat("boidsCenteringFactor", settings.centeringFactor);
            shader.SetFloat("boidsAvoidFactor", settings.avoidFactor);
            shader.SetFloat("boidsTurnFactor", settings.turnFactor);

        }

        public override void Step(float dt)
        {
            shader.Dispatch(kernel, buffers.ThreadGroups, 1, 1);
            
            shader.SetFloat("deltaTime", dt);
            
            // Boids settings
            shader.SetFloat("boidsMinSpeed", settings.minSpeed);
            shader.SetFloat("boidsMaxSpeed", settings.maxSpeed);
            shader.SetFloat("boidsVisualRange", settings.visualRange);
            shader.SetFloat("boidsProtectedRange", settings.protectedRange);
            shader.SetFloat("boidsMatchingFactor", settings.matchingFactor);
            shader.SetFloat("boidsCenteringFactor", settings.centeringFactor);
            shader.SetFloat("boidsAvoidFactor", settings.avoidFactor);
            shader.SetFloat("boidsTurnFactor", settings.turnFactor);
            
            shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
            shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
            buffers.Swap();
        }
    }
}