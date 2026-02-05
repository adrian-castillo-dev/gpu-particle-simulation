using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

struct newParticle {
    public GameObject gameobject;
    public int type;
    public Vector3 position;
    public Vector3 velocity;
}

public struct matrixRow {
    public float[] columns;
}

public class ParticleLifeCPU : MonoBehaviour {
    public GameObject particlePrefab;
    public int n = 100;
    public float dt = 0.02f;
    public float frictionHalfLife = 0.040f;
    public float distanceMax = 3f;
    public int types = 6;
    public float simSize;
    public float particleRadius;
    public matrixRow[] matrix; 
    public float forceFactor = 1f;
    private float frictionFactor;

    private newParticle[] particles;

    private void Start() {
        matrix = makeRandomMatrix();
        frictionFactor = Mathf.Pow(0.5f, dt / frictionHalfLife);

        particles = InitializeParticles();
    }

    private void Update() {
        ComputeForces(particles);
        UpdateParticleVisuals(particles);
    }

    private matrixRow[] makeRandomMatrix() {
        matrixRow[] rows = new matrixRow[types];
        
        for (int row = 0; row < types; row++) {
            rows[row] = new matrixRow { columns =  new float[types]};
            
            for (int element = 0; element < types; element++) {
                rows[row].columns[element] = Random.Range(-1.0f, 1.0f);
            }
        }

        return rows;
    }

    private newParticle[] InitializeParticles() {
        newParticle[] particles = new newParticle[n];
        for (int i = 0; i < n; i++) {
            particles[i].position = Random.insideUnitSphere * simSize;
            particles[i].velocity = Vector3.zero;
            particles[i].type = Random.Range(0, types);
            particles[i].gameobject = Instantiate(particlePrefab, particles[i].position, Quaternion.identity);
            particles[i].gameobject.transform.localScale = Vector3.one * particleRadius;
            
            float hue = (float)particles[i].type / (float)types;
            particles[i].gameobject.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1, 1);
        }

        return particles;
    }

    private void ComputeForces(newParticle[] particles) {
        for (int i = 0; i < n; i++) {
            newParticle particle = particles[i];
            Vector3 totalForce = Vector3.zero;
            for (int j = 0; j < n; j++) {
                if (j == i) continue;
                newParticle otherParticle = particles[j];
                Vector3 direction = particles[j].position - particles[i].position;
                float distanceSquared = math.dot(direction, direction);
                float distance = math.sqrt(distanceSquared);
                if (distance > 0 && distance < distanceMax) {
                    float force = CalculateForce(distance / distanceMax,
                        matrix[particle.type].columns[otherParticle.type]);
                    totalForce += (direction / (distance + 0.001f) * force) * forceFactor;
                }

            }

            totalForce *= distanceMax;
            particle.velocity *= frictionFactor;
            particle.velocity += totalForce * dt;

            particles[i] = particle;
        }
    }

    private void UpdateParticleVisuals(newParticle[] particles) {
        for (int i = 0; i < n; i++) {
            particles[i].position += particles[i].velocity;
            particles[i].gameobject.transform.position = particles[i].position;
   
        }
        
    }

    private float CalculateForce(float distance, float attraction) {
        float beta = 0.3f;
        if (distance < beta) {
            return distance / beta - 1;
        } else if (distance > beta && distance < 1) {
            return attraction * (1 - Math.Abs(2 * distance - 1 - beta) / (1 - beta));
        } else {
            return 0;
        }
    }
}


