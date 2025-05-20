using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    private List<MovingPlatform> movingPlatforms;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (movingPlatforms == null)
        {
            return;
        }

        foreach (MovingPlatform platform in movingPlatforms)
            platform.Activate();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (movingPlatforms == null)
        {
            return;
        }

        foreach (MovingPlatform platform in movingPlatforms)
        {
            platform.Deactivate();
        }
    }
}
