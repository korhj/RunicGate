using System.Collections.Generic;
using UnityEngine;

public class TimedButton : MonoBehaviour, IPlayerInteractable
{
    [SerializeField]
    private float activationTime;

    [SerializeField]
    private List<MovingPlatform> movingPlatforms;

    [SerializeField]
    private Sprite buttonSprite;

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
        }
    }

    public GameObject Interact()
    {
        time = activationTime;
        foreach (MovingPlatform platform in movingPlatforms)
            platform.Activate();
        return null;
    }

    public Sprite GetButtonSprite()
    {
        return buttonSprite;
    }
}
