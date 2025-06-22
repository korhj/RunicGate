using System;
using UnityEngine;

public class HealingFountain : MonoBehaviour, IPlayerInteractable
{
    [SerializeField]
    Player player;

    [SerializeField]
    private int healingAmount;

    public GameObject Interact()
    {
        player.TakeDamage(-healingAmount);
        return null;
    }
}
