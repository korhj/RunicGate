using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IActivatableTrap
{
    [SerializeField]
    private Vector3Int tileOffset;

    [SerializeField]
    private float speed;

    private bool isActive;
    public bool IsMoving { get; private set; }
    private Vector3 startWorldPos;
    private Vector3 targetWorldPos;
    private IObstacle objectOnPlatform;
    private MapManager mapManager;

    private void Start()
    {
        isActive = false;
        IsMoving = false;
        startWorldPos = transform.position;
        mapManager = MapManager.Instance;
        Vector3Int startPos = mapManager.WorldToTile(startWorldPos);
        targetWorldPos = mapManager.TileToWorld(startPos + tileOffset);
    }

    private void Update()
    {
        Vector3 destination = isActive ? targetWorldPos : startWorldPos;
        float movementDistance = speed * Time.deltaTime;

        if (movementDistance >= (transform.position - destination).magnitude)
        {
            if (IsMoving && objectOnPlatform != null)
            {
                objectOnPlatform.MoveToTile(mapManager.WorldToTile(destination));
                objectOnPlatform.SetParent(null);
                objectOnPlatform = null;
            }
            if (IsMoving)
            {
                SelectedTile childTile = transform.GetComponentInChildren<SelectedTile>();
                if (childTile != null)
                {
                    childTile.tilePos = mapManager.WorldToTile(destination);
                }
            }
            transform.position = destination;
            IsMoving = false;
            return;
        }

        if (!IsMoving)
        {
            IObstacle obstacle = mapManager.GetObstacle(mapManager.WorldToTile(transform.position));
            if (obstacle != null)
            {
                objectOnPlatform = obstacle;
                objectOnPlatform.SetParent(transform);
            }
        }

        IsMoving = true;
        transform.position = Vector3.MoveTowards(transform.position, destination, movementDistance);
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
