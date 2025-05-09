using System;
using System.Collections.Generic;
using UnityEngine;

public class RunicGate : MonoBehaviour
{
    public event EventHandler<OnRunicGateEnteredEventArgs> OnRunicGateEntered;

    public class OnRunicGateEnteredEventArgs : EventArgs
    {
        public GameObject runicGateGameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnRunicGateEntered?.Invoke(
            this,
            new OnRunicGateEnteredEventArgs { runicGateGameObject = this.gameObject }
        );
    }
}
