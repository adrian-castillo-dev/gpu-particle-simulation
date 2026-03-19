using UnityEngine;

[CreateAssetMenu(menuName = "Simulation/GravitySettings")]
public class GravitySettings : ScriptableObject
{
    public float G = 1f;
    public float softening = 0.5f;
}
