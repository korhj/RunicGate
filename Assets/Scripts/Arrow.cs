using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rb2D;
    int arrowDamage = 0;

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
