using UnityEngine;
using System.Runtime.InteropServices;


namespace SimulationEngine.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BoidParticle {
        public Vector3 position;
        public Vector3 velocity;

        public const int Size =
            sizeof(float) * 3 +
            sizeof(float) * 3;
    }
}
