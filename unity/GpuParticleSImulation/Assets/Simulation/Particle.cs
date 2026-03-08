using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Particle {
   public Vector3 position;
   public Vector3 velocity;
   public float mass;
   public int type;

   public const int Size =
    sizeof(float) * 3 +
    sizeof(float) * 3 +
    sizeof(float) +
    sizeof(int);
}
