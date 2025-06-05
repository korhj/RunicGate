using System;
using UnityEngine;

public class PoisonFloor : MonoBehaviour
{
    [SerializeField]
    int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.TakeDamage(damage, true);
        }
    }
}
