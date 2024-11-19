using UnityEngine;

// WORLD EVENT
[CreateAssetMenu(fileName = "New WorldEvent", menuName = "ADDON/World Event/WorldEvent Configuration", order = 999)]
public partial class WorldEventConfiguration : ScriptableObject
{
    [Header("[-=-[ World Event List ]-=-]")]
    public WorldEventTemplate[] worldEvents;
}
