using UnityEngine;

namespace SimulationEngine.Settings
{
    [CreateAssetMenu(menuName = "Simulation/Particle Life Settings")]

    public class ParticleLifeSettings : ScriptableObject
    {
        public int types = 9;
        public float frictionHalfLife = 0.008f;
        public float rMax = 25;
        public float forceFactor = 2;
    }
}
