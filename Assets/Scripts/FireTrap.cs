using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [SerializeField]
    private float cooldown;

    [SerializeField]
    private float activeDuration;

    [SerializeField]
    private int damage;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite onSprite;

    [SerializeField]
    private Sprite offSprite;

    private float timer;
    private bool isOn;
    private MapManager mapManager;

    void Start()
    {
        isOn = true;
        timer = activeDuration;
        spriteRenderer.sprite = onSprite;
        mapManager = MapManager.Instance;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (isOn)
            {
                DeactivateTrap();
            }
            else
            {
                ActivateTrap();
            }
        }

        if (isOn)
        {
            TryDamagePlayer();
        }
    }

    private void ActivateTrap()
    {
        isOn = true;
        timer = activeDuration;
        spriteRenderer.sprite = onSprite;
    }

    private void DeactivateTrap()
    {
        isOn = false;
        timer = cooldown;
        spriteRenderer.sprite = offSprite;
    }

    private void TryDamagePlayer()
    {
        IObstacle obstacle = mapManager.GetObstacle(mapManager.WorldToTile(transform.position));
        if (obstacle is Player player)
        {
            player.TakeDamage(damage);
        }
    }
}
