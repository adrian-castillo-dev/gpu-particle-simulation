using UnityEngine;

namespace Simulation
{
    public abstract class ParticleSimulation<TSettings> where TSettings : ScriptableObject
    {
        protected TSettings settings;
        protected ComputeShader shader;
        protected ParticleBufferManager buffers;
    
        public ParticleSimulation(ComputeShader shader, ParticleBufferManager buffers, TSettings settings)
        {
            this.shader = shader;
            this.buffers = buffers;
            this.settings = settings;
        }

        public abstract void SetUp();
    
        public abstract void Step(float dt);
    }
}


