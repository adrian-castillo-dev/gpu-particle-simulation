using SimulationEngine.Core;
using SimulationEngine.Settings;
using UnityEngine;

namespace SimulationEngine.Simulations
{
    public class BoidsSimulation : ParticleSimulation<BoidsSettings>
    {
        public BoidsSimulation(ComputeShader shader, ParticleBufferManager buffers, BoidsSettings settings) : base(shader, buffers, settings){}

        public override void SetUp()
        {
            
        }

        public override void Step(float dt)
        {
            
        }
    }
}