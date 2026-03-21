using SimulationEngine.Core;
using SimulationEngine.Settings;
using UnityEngine;

namespace SimulationEngine.Simulations
{
    public class BoidsSimulation : ParticleSimulation<BoidsSettings>
    {
        private int kernel;
        
        public BoidsSimulation(ComputeShader shader, ParticleBufferManager buffers, BoidsSettings settings) : base(shader, buffers, settings){}
        
        public override void SetUp()
        {
            kernel = shader.FindKernel("ComputeBoids");
            
            shader.SetFloat("boidsRange", settings.range);
            shader.SetFloat("boidsSpeed", settings.speed);
            
        }

        public override void Step(float dt)
        {
            shader.Dispatch(kernel, buffers.ThreadGroups, 1, 1);
            
            buffers.Swap();
        }
    }
}