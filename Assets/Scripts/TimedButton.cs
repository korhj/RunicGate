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

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private List<Sprite> progressSprites;

    [SerializeField]
    private Sprite onSprite;

    [SerializeField]
    private Sprite offSprite;

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

        if (time / activationTime > 0.75f)
        {
            spriteRenderer.sprite = progressSprites[0];
            return;
        }
        if (time / activationTime > 0.5f)
        {
            spriteRenderer.sprite = progressSprites[1];
            return;
        }
        if (time / activationTime > 0.25f)
        {
            spriteRenderer.sprite = progressSprites[2];
            return;
        }
        if (time / activationTime > 0f)
        {
            spriteRenderer.sprite = progressSprites[3];
            return;
        }

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
        spriteRenderer.sprite = onSprite;
    }

    private void DeactivateTraps()
    {
        foreach (MovingPlatform platform in movingPlatforms)
            platform.Deactivate();
        foreach (ArrowTrap trap in arrowTraps)
            trap.Deactivate();
        spriteRenderer.sprite = offSprite;
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
