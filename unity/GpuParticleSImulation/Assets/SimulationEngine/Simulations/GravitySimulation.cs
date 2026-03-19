using UnityEngine;
using SimulationEngine.Core;

namespace SimulationEngine.Simulations
{
    public class GravitySimulation : ParticleSimulation<GravitySettings>
    {
        private int kernel;
        private GravitySettings settings;

        public GravitySimulation(ComputeShader shader, ParticleBufferManager buffers, GravitySettings settings) : base(shader, buffers, settings)
        {
            this.settings = settings;
        
            kernel = shader.FindKernel("ComputeGravity");
        
            shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
            shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
        
            shader.SetInt("nParticles", buffers.ParticleCount);
        }

        public override void Step(float dt)
        {
            shader.SetFloat("deltaTime", dt);
            shader.SetFloat("G", settings.G);
            shader.SetFloat("softening", settings.softening);
        
            shader.Dispatch(kernel, buffers.ThreadGroups, 1, 1);

            buffers.Swap();
        
            shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
            shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
        }

        public override void SetUp()
        {
            
        }

    }
}


