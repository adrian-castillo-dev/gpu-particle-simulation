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
            
            shader.SetFloat("boidsRange", settings.range);
            shader.SetFloat("boidsSpeed", settings.speed);
            shader.SetFloat("boidsTime", time);
            shader.SetFloat("boidsWeight", settings.weight);
            shader.SetFloat("averageDirectionWeight", settings.averageDirectionWeight);



        }

        public override void Step(float dt)
        {
            shader.Dispatch(kernel, buffers.ThreadGroups, 1, 1);
            
            shader.SetFloat("deltaTime", dt);
            shader.SetFloat("boidsRange", settings.range);
            shader.SetFloat("boidsSpeed", settings.speed);
            shader.SetFloat("boidsWeight", settings.weight);
            shader.SetFloat("averageDirectionWeight", settings.averageDirectionWeight);

            shader.SetFloat("boidsTime", time += 1 * dt);
            
            shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
            shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
            buffers.Swap();
        }
    }
}