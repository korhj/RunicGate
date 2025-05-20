using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private Vector3Int tileOffset;

    [SerializeField]
    private float speed;
    private bool isActive;
    private Vector3 startPos;
    private Vector3 targetPos;

    private void Start()
    {
        isActive = false;
        startPos = transform.position;
        Vector3Int startTilePos = MapManager.Instance.WorldToTile(startPos);
        targetPos = MapManager.Instance.TileToWorld(startTilePos + tileOffset);
    }

    private void Update()
    {
        Vector3 destination = isActive ? targetPos : startPos;
        float movementDistance = speed * Time.deltaTime;
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
