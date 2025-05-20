using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mappings/PressurePlate → MovingPlatforms")]
public class PlatePlatformMap : ScriptableObject
{
    [System.Serializable]
    public class PlatePlatforms
    {
        [Tooltip("PressurePlate GameObject that will trigger these MovingPlatforms")]
        public PressurePlate plate;

        [Tooltip("All MovingPlatforms that this PressurePlate should activate/deactivate")]
        public List<MovingPlatform> platforms = new();
    }

    [Tooltip("List of (PressurePlate → MovingPlatforms) mappings")]
    public List<PlatePlatforms> mappings = new();

    public List<MovingPlatform> GetPlatformsFor(PressurePlate plate)
    {
        foreach (var pair in mappings)
        {
            if (pair.plate == plate)
            {
                return pair.platforms;
            }
        }
        ;
        return null;
    }
}
