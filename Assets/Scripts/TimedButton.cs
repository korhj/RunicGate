using System.Collections.Generic;
using UnityEngine;

public class TimedButton : MonoBehaviour, IPlayerInteractable
{
    [SerializeField]
    private float activationTime;

    [SerializeField]
    private List<MovingPlatform> movingPlatforms;

    [SerializeField]
    private List<ArrowTrap> arrowTraps;

    private float time;

    void Start()
    {
        time = 0;
    }

    void Update()
    {
        if (time <= 0)
        {
            return;
        }

        time -= Time.deltaTime;

        if (time <= 0)
        {
            foreach (MovingPlatform platform in movingPlatforms)
                platform.Deactivate();
            foreach (ArrowTrap trap in arrowTraps)
                trap.Deactivate();
        }
    }

    public GameObject Interact()
    {
        time = activationTime;
        foreach (MovingPlatform platform in movingPlatforms)
            platform.Activate();
        foreach (ArrowTrap trap in arrowTraps)
            trap.Activate();
        return null;
    }
}
