using UnityEngine;

namespace SimulationEngine.Settings
{
    [CreateAssetMenu(menuName = "Simulation/Boids Settings")]

    public class BoidsSettings : ScriptableObject
    {
        public float minSpeed = 3;
        public float maxSpeed = 6;
        public float visualRange = 8;
        public float protectedRange = 1;
        public float matchingFactor = 1;
        public float centeringFactor = 1;
        public float avoidFactor = 1;
        public float turnFactor = 0.1f;
        public int boidsEnemies = 10;
    }
}