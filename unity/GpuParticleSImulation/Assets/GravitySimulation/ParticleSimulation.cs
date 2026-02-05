 using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

struct Particle {
    public float3 position;
    public float3 velocity;
}

public class ParticleSimulation : MonoBehaviour {
    public Mesh mesh;
    public Material material;
    public ComputeShader computeShader;
    
    [SerializeField] private int particleCount = 500;
    [SerializeField] private float particleRadius = 0.2f;
    [SerializeField] private float simulationSize = 30f;
    [SerializeField] private float G = 6.674f;
    [SerializeField] private float timeScale = 0.003f;  
    [SerializeField] private float softening = 0.5f;

    private ComputeBuffer particleBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] {0, 0, 0, 0, 0};
    private Bounds bounds;

    private int kernelIndex;

    private Particle[] particles;
    
    private void Start() {
        // Spawn particles at random positions
        particles = new Particle[particleCount];
        for (int i = 0; i < particleCount; i++) {
            particles[i].position = new Vector3(
                UnityEngine.Random.Range(-simulationSize, simulationSize),
                UnityEngine.Random.Range(-simulationSize, simulationSize),
                UnityEngine.Random.Range(-simulationSize, simulationSize)
                );
        }
        particleBuffer = new ComputeBuffer(particleCount, Marshal.SizeOf<Particle>());
        particleBuffer.SetData(particles);
        // args buffer
        args[0] = mesh.GetIndexCount(0);
        args[1] = (uint)particleCount;
        args[2] = 0;
        args[3] = 0;
        args[4] = 0;

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
        
        // Setup rendering
        material.SetBuffer("particleBuffer", particleBuffer);
        material.SetFloat("_Radius", particleRadius);
        
        // Setup compute shader
        kernelIndex = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernelIndex, "particles" , particleBuffer);
        // Set Ints
        computeShader.SetInt("_ParticleCount", particleCount);
        // Set Floats
        computeShader.SetFloat("dt", Time.deltaTime);
        computeShader.SetFloat("_G", G);
        computeShader.SetFloat("_Softening", softening);
        
        bounds = new Bounds(Vector3.zero, Vector3.one * simulationSize * 20);
    }

    private void Update() {
        computeShader.SetFloat("dt", Time.deltaTime * timeScale);
        // Run compute shader
        int threadGroups = Mathf.CeilToInt((float)particleCount / 512);
        computeShader.Dispatch(kernelIndex, threadGroups, 1, 1);
        
        // Render particles
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer,
            castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
    }

    private void OnDestroy() {
        particleBuffer.Release();
        argsBuffer.Release();
    }
}
