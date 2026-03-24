using UnityEngine;

namespace SimulationEngine.Settings
{
    [CreateAssetMenu(menuName = "Simulation/Boids Settings")]

    public class BoidsSettings : ScriptableObject
    {
        public float speed = 5;
        public float visualRange = 8;
        public float protectedRange = 1;
        public float alignmentFactor = 1;
        public float cohesionFactor = 1;
        public float separationFactor = 1;
    }
}