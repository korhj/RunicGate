using System.Collections.Generic;
using UnityEngine;

public class RunicGate : MonoBehaviour
{
    [SerializeField]
    GameObject runicGateVisual;

    private List<GameObject> runicGates;

    void Start()
    {
        runicGates = new List<GameObject>();
        Instantiate<GameObject>(runicGateVisual);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision");
    }
}
