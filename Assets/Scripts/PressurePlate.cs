using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    private List<MovingPlatform> movingPlatforms;

    [SerializeField]
    private List<ArrowTrap> arrowTraps;
    private bool isTriggered;

    private void Update()
    {
        if (isTriggered)
        {
            ActivateTraps();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isTriggered = true;
        if (movingPlatforms == null)
        {
            return;
        }

        ActivateTraps();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isTriggered = false;
        if (movingPlatforms == null)
        {
            return;
        }
        DeactivateTraps();
    }

    private void ActivateTraps()
    {
        foreach (MovingPlatform platform in movingPlatforms)
            platform.Activate();
        foreach (ArrowTrap trap in arrowTraps)
            trap.Activate();
    }

    private void DeactivateTraps()
    {
        foreach (MovingPlatform platform in movingPlatforms)
            platform.Deactivate();
        foreach (ArrowTrap trap in arrowTraps)
            trap.Deactivate();
    }
}
