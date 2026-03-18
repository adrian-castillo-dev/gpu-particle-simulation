using UnityEngine;

public class GravitySimulation : ParticleSimulation
{
    private int kernel;

    public float G;
    public float softening;

    public GravitySimulation(ComputeShader shader, ParticleBufferManager buffers) : base(shader, buffers)
    {
        kernel = shader.FindKernel("ComputeGravity");
        
        shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
        shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
        
        shader.SetInt("nParticles", buffers.ParticleCount);
    }

    public override void Step(float dt)
    {
        shader.SetFloat("deltaTime", dt);
        shader.SetFloat("G", G);
        shader.SetFloat("softening", softening);
        
        shader.Dispatch(kernel, buffers.ThreadGroups, 1, 1);

        buffers.Swap();
        
        shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
        shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
    }

}
