using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/WalkableGameObjects")]
public class WalkableGameObjectsSO : ScriptableObject
{
    [Tooltip("Components that, if present, make the GameObject walkable")]
    public List<Component> walkableComponents;
}
