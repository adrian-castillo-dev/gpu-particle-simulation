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
            
            shader.SetFloat("boidsVisualRange", settings.visualRange);
            shader.SetFloat("boidsProtectedRange", settings.protectedRange);

            shader.SetFloat("boidsSpeed", settings.speed);
            shader.SetFloat("alignmentWeight", settings.alignmentFactor);
            shader.SetFloat("cohesionFactor", settings.cohesionFactor);
            shader.SetFloat("separationFactor", settings.separationFactor);



        }

        public override void Step(float dt)
        {
            shader.Dispatch(kernel, buffers.ThreadGroups, 1, 1);
            
            shader.SetFloat("deltaTime", dt);
            
            shader.SetFloat("boidsVisualRange", settings.visualRange);
            shader.SetFloat("boidsProtectedRange", settings.protectedRange);
            shader.SetFloat("boidsSpeed", settings.speed);
            shader.SetFloat("alignmentWeight", settings.alignmentFactor);
            shader.SetFloat("cohesionFactor", settings.cohesionFactor);
            shader.SetFloat("separationFactor", settings.separationFactor);

            
            shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
            shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
            buffers.Swap();
        }
    }
}