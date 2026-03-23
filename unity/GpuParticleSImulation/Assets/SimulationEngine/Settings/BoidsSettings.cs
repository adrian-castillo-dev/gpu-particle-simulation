using UnityEngine;

namespace SimulationEngine.Settings
{
    [CreateAssetMenu(menuName = "Simulation/Boids Settings")]

    public class BoidsSettings : ScriptableObject
    {
        public float speed = 5;
        public float range = 8;
        public float weight = 1;
        public float averageDirectionWeight = 1;
    }
}