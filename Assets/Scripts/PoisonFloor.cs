using System;
using UnityEngine;

public class PoisonFloor : MonoBehaviour
{
    public event EventHandler OnPoisonFloorEnter;

    [SerializeField]
    int damage;

    void Start() { }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnPoisonFloorEnter?.Invoke(this, EventArgs.Empty);
    }
}
