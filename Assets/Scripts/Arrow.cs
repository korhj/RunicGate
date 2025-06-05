using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rb2D;
    int arrowDamage = 0;

    private void Update()
    {
        Vector3Int tilePos = MapManager.Instance.WorldToTile(transform.position);
        Vector3Int? tileAtPos = MapManager.Instance.FindWalkableTileAt(tilePos, 0, 2);
        if (!tileAtPos.HasValue)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IObstacle>(out _))
        {
            if (collision.TryGetComponent(out Player player))
            {
                player.TakeDamage(arrowDamage);
            }
            Destroy(gameObject);
        }
    }

    public void Shoot(float thrust, Vector2 dir, int damage, float lifetime)
    {
        arrowDamage = damage;
        rb2D.AddForce(dir * thrust, ForceMode2D.Impulse);
        Destroy(gameObject, lifetime);
    }
}
