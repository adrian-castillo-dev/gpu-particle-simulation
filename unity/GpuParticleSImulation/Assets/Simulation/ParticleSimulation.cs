using UnityEngine;

public abstract class ParticleSimulation
{
    protected ComputeShader shader;
    protected ParticleBufferManager buffers;

    public ParticleSimulation(ComputeShader shader, ParticleBufferManager buffers)
    {
        this.shader = shader;
        this.buffers = buffers;
    }

    public abstract void Step(float dt);
}
