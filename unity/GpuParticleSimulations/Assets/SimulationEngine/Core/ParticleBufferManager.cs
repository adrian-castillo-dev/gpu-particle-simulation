using UnityEngine;

namespace SimulationEngine.Core
{
    public class ParticleBufferManager
    {
        public ComputeBuffer Read { get; private set; }
        public ComputeBuffer Write { get; private set; }

        public int ParticleCount { get; private set; }
        public int ThreadGroups { get; private set; }

        public ParticleBufferManager(int particleCount)
        {
            ParticleCount = particleCount;
            ThreadGroups = Mathf.CeilToInt(particleCount / 128f);

            Read = new ComputeBuffer(particleCount, Particle.Size);
            Write = new ComputeBuffer(particleCount, Particle.Size);
        }

        public void Swap()
        {
            (Read, Write) = (Write, Read);
        }

        public void Release()
        {
            Read.Release();
            Write.Release();
        }
    }
}
