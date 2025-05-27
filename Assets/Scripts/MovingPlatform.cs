using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private Vector3Int tileOffset;

    [SerializeField]
    private float speed;
    private bool isActive;

    private bool isMoving;
    private Vector3 startWorldPos;
    private Vector3 targetWorldPos;

    private IObstacle objectOnPlatform;
    private MapManager mapManager;

    private void Start()
    {
        isActive = false;
        isMoving = false;
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
            if (isMoving && objectOnPlatform != null)
            {
                objectOnPlatform.MoveToTile(mapManager.WorldToTile(destination));
                objectOnPlatform.SetParent(null);
                objectOnPlatform = null;
            }
            transform.position = destination;
            isMoving = false;
            return;
        }

        if (!isMoving)
        {
            IObstacle obstacle = mapManager.GetObstacle(mapManager.WorldToTile(transform.position));
            if (obstacle != null)
            {
                objectOnPlatform = obstacle;
                objectOnPlatform.SetParent(transform);
            }
        }

        isMoving = true;
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
