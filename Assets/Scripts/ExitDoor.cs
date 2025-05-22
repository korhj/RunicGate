using System;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public event EventHandler<EventArgs> OnExitDoorEntered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out _))
        {
            OnExitDoorEntered?.Invoke(this, EventArgs.Empty);
        }
    }
}
