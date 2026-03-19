using UnityEngine;

namespace SimulationEngine.Rendering
{
    public class ParticleRenderer
    {
        private Mesh mesh;
        private Material material;
        private ComputeBuffer argsBuffer;
    
        public ParticleRenderer(Mesh mesh, Material material, int particleCount)
        {
            this.mesh = mesh;
            this.material = material;

            uint[] args = new uint[5];

            args[0] = mesh.GetIndexCount(0);
            args[1] = (uint)particleCount;
            args[2] = mesh.GetIndexStart(0);
            args[3] = mesh.GetBaseVertex(0);
            args[4] = 0;
        
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        
            argsBuffer.SetData(args);
        }

        public void SetParticleBuffer(ComputeBuffer buffer) 
        {
            material.SetBuffer("particleBuffer", buffer);
        }

        public void Render()
        {
            Graphics.DrawMeshInstancedIndirect(
                mesh,
                0,
                material,
                new Bounds(Vector3.zero, Vector3.one * 10000f),
                argsBuffer);
        }

        public void Release()
        {
            argsBuffer.Release();
        }
    }
}

