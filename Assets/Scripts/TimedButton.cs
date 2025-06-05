using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimedButton : MonoBehaviour, IPlayerInteractable
{
    [SerializeField]
    private float activationTime;

    [SerializeField]
    private List<MovingPlatform> movingPlatforms;

    [SerializeField]
    private List<ArrowTrap> arrowTraps;

    [SerializeField]
    private bool isToggle;
    private bool isActive;
    private float time;

    void Start()
    {
        time = 0;
        isActive = false;
    }

    void Update()
    {
        if (isToggle)
        {
            return;
        }
        if (time <= 0)
        {
            return;
        }

        time -= Time.deltaTime;

        if (time <= 0)
        {
            DeactivateTraps();
        }
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

    public GameObject Interact()
    {
        if (isToggle)
        {
            if (isActive)
            {
                DeactivateTraps();
                isActive = false;
            }
            else
            {
                ActivateTraps();
                isActive = true;
            }
            return null;
        }
        time = activationTime;
        ActivateTraps();

        return null;
    }
}
