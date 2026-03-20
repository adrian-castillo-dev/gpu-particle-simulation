using UnityEngine;

namespace SimulationEngine.Settings
{
    [CreateAssetMenu(menuName = "Simulation/Boids Settings")]

    public class BoidsSettings : ScriptableObject
    {
        public int speed;
    }
}