using UnityEngine;
using SimulationEngine.Core;

namespace SimulationEngine.Simulations
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
        
        public abstract void Step(float dt);
        
        public abstract void SetUp();
    }
}


