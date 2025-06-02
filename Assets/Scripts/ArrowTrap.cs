using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ArrowTrap : MonoBehaviour, IActivatableTrap
{
    [SerializeField]
    private float activationTime = 0.5f;

    [SerializeField]
    private Arrow arrowPrefab;

    [SerializeField]
    private float arrowThrust;

    [SerializeField]
    private int arrowDamage;

    [SerializeField]
    private float arrowlifetime;

    public enum Direction
    {
        Left,
        Right
    }

    [SerializeField]
    private Direction direction;

    [SerializeField]
    private float a;

    [SerializeField]
    private float b;
    private float timer;
    private bool isActive;
    private Vector3 arrowSpawnPoint;
    private Vector2 dir;

    private void Start()
    {
        timer = 0;
        isActive = false;
        Vector3Int tilePos = MapManager.Instance.WorldToTile(transform.position);
        arrowSpawnPoint = MapManager.Instance.TileToWorld(tilePos - new Vector3Int(0, 0, 2));
        dir = direction == Direction.Right ? new Vector2(1, -0.5f) : new Vector2(-1, -0.5f);
    }

    private void Update()
    {
        if (timer <= 0f && isActive)
        {
            timer = activationTime;
            ShootArrow();
        }
        timer -= Time.deltaTime;
    }

    private void ShootArrow()
    {
        Arrow arrow = Instantiate(arrowPrefab, arrowSpawnPoint, Quaternion.identity);
        arrow.Shoot(arrowThrust, dir, arrowDamage, arrowlifetime);
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}
