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

    private Player playerOnPlatform;

    private void Start()
    {
        isActive = false;
        isMoving = false;
        startWorldPos = transform.position;
        Vector3Int startPos = MapManager.Instance.WorldToTile(startWorldPos);
        targetWorldPos = MapManager.Instance.TileToWorld(startPos + tileOffset);
    }

    private void Update()
    {
        Vector3 destination = isActive ? targetWorldPos : startWorldPos;
        float movementDistance = speed * Time.deltaTime;

        if (movementDistance >= (transform.position - destination).magnitude)
        {
            if (isMoving && playerOnPlatform != null)
            {
                playerOnPlatform.SetParent(null);
                playerOnPlatform = null;
            }
            transform.position = destination;
            isMoving = false;
            return;
        }

        if (playerOnPlatform != null && playerOnPlatform.transform.position == transform.position)
        {
            playerOnPlatform.SetParent(transform);
        }

        isMoving = true;
        transform.position = Vector3.MoveTowards(transform.position, destination, movementDistance);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            if (!isMoving)
            {
                playerOnPlatform = player;
            }
        }
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
