using SimulationEngine.Core;
using SimulationEngine.Settings;
using UnityEngine;

namespace SimulationEngine.Simulations
{
    public class ParticleLifeSimulation : ParticleSimulation<ParticleLifeSettings>
    {
        private int kernel;
        
        private ComputeBuffer attractionMatrixBuffer;
        private float[] attractionMatrix;

        private float frictionFactor;
        
        public ParticleLifeSimulation(ComputeShader shader, ParticleBufferManager buffers, ParticleLifeSettings settings) : base (shader, buffers, settings) {}

        public override void SetUp()
        {
            kernel = shader.FindKernel("ComputeParticleLifeForces");
            
            // Core Buffers
            shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
            shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
            
            shader.SetInt("nParticles", buffers.ParticleCount);
            
            // Create Attraction Matrix
            int size = settings.types * settings.types;
            
            attractionMatrix = CreateRandomMatrix(settings.types);
            
            attractionMatrixBuffer = new ComputeBuffer(size, sizeof(float));
            attractionMatrixBuffer.SetData(attractionMatrix);
            
            shader.SetBuffer(kernel, "attractionMatrix", attractionMatrixBuffer);
            frictionFactor = Mathf.Pow(0.5f, 0.02f / settings.frictionHalfLife);
        }

        public override void Step(float dt)
        {
            shader.SetFloat("deltaTime", dt);
            shader.SetFloat("rMax", settings.rMax);
            shader.SetFloat("forceFactor", settings.forceFactor);
            shader.SetFloat("frictionFactor", frictionFactor);
            shader.SetInt("nTypes", settings.types);
            
            shader.Dispatch(kernel, buffers.ThreadGroups, 1, 1);
            
            buffers.Swap();
            
            shader.SetBuffer(kernel, "particleReadBuffer", buffers.Read);
            shader.SetBuffer(kernel, "particleWriteBuffer", buffers.Write);
        }
        
        // Returns a random matrix based on the number of types for the particle life simulation
        private float[] CreateRandomMatrix(int types)
        {
            float[] matrix = new float[types * types];
            for (int row = 0; row < types; row++) {
                for (int col = 0; col < types; col++) {
                    int index = row * types + col;
                    matrix[index] = Random.Range(-1.0f, 1.0f);
                }
            }
            return matrix;
        }

        public void Release()
        {
            attractionMatrixBuffer?.Release();
        }
    }
}