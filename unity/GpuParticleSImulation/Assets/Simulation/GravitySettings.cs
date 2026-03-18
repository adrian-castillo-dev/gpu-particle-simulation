using UnityEngine;

[CreateAssetMenu(menuName = "Simulation/Gravity Settings")]
public class GravitySettings : ScriptableObject
{
    public float G = 1f;
    public float softening = 0.5f;
}
