using System.Collections.Generic;
using UnityEngine;

public class RunicGate : MonoBehaviour
{
    private List<GameObject> runicGates;

    void Start()
    {
        runicGates = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision");
    }
}
