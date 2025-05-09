using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer playerSpriteRenderer;

    [SerializeField]
    private RunicGateManager runicGateManager;

    [SerializeField]
    float playerSpeed = 5f;

    InputAction moveAction;
    private Vector3Int currentTileCoordinates;

    private Vector3Int _targetTileCoordinates;
    private Vector3 _targetTileWorldCoordinates;
    private Vector3Int TargetTileCoordinates
    {
        get => _targetTileCoordinates;
        set
        {
            _targetTileCoordinates = value;
            _targetTileWorldCoordinates = MapManager.Instance.TileToWorld(_targetTileCoordinates);
        }
    }
    private Vector3 TargetTileWorldCoordinates => _targetTileWorldCoordinates;
    private bool isMoving;
    private bool isTeleporting;

    private Vector2Int playerDirection;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        currentTileCoordinates = new(0, 0, 0);
        TargetTileCoordinates = new(0, 0, 0);
        isMoving = false;
        isTeleporting = false;
        playerDirection = new(1, 0);

        MovePlayerToTile(new Vector3Int(0, 0, 0));

        runicGateManager.OnTeleport += (_, e) =>
            StartCoroutine(TeleportWithCooldown(e.targetTileCoordinates, e.exitGateCollider));
    }

    void Update()
    {
        if (!isMoving && !isTeleporting)
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            if (moveValue.x != 0)
            {
                playerDirection = new Vector2Int((int)Mathf.Sign(moveValue.x), 0);
                SetTargetTile();
            }
            else if (moveValue.y != 0)
            {
                playerDirection = new Vector2Int(0, (int)Mathf.Sign(moveValue.y));
                SetTargetTile();
            }
        }
        else
        {
            MoveTowardsTargetTile();
        }
    }

    private void OnInteract()
    {
        bool targetTileIsAccessible = MapManager.Instance.IsTileAccessible(
            currentTileCoordinates + new Vector3Int(playerDirection.x, playerDirection.y, 0)
        );
        if (targetTileIsAccessible)
        {
            runicGateManager.ToggleRunicGate(
                currentTileCoordinates + new Vector3Int(playerDirection.x, playerDirection.y, 0)
            );
        }
    }

    private void SetTargetTile()
    {
        if (isMoving)
            return;

        Vector2Int targetCoordinates =
            new Vector2Int(currentTileCoordinates.x, currentTileCoordinates.y) + playerDirection;

        Vector3Int? nextTile = MapManager.Instance.GetTopTileAt(targetCoordinates);

        if (nextTile.HasValue)
        {
            if (Mathf.Abs(currentTileCoordinates.z - nextTile.Value.z) <= 1)
            {
                TargetTileCoordinates = nextTile.Value;
                isMoving = true;
            }
        }
    }

    private void MoveTowardsTargetTile()
    {
        Vector3 movementDirection = TargetTileWorldCoordinates - transform.position;
        float movementDistance = playerSpeed * Time.deltaTime;

        if (movementDistance >= movementDirection.magnitude)
        {
            transform.position = TargetTileWorldCoordinates;
            isMoving = false;
            currentTileCoordinates = TargetTileCoordinates;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                TargetTileWorldCoordinates,
                movementDistance
            );
        }
    }

    private void MovePlayerToTile(Vector3Int tileCoordinates)
    {
        Vector3 worldPos = MapManager.Instance.TileToWorld(tileCoordinates);
        transform.position = worldPos;
        currentTileCoordinates = tileCoordinates;
        TargetTileCoordinates = tileCoordinates;
        isMoving = false;
    }

    private bool SetTargetTileForTeleport(Vector3Int exitTileCoordinates)
    {
        Vector2Int targetCoordinates =
            new Vector2Int(exitTileCoordinates.x, exitTileCoordinates.y) + playerDirection;

        Vector3Int? nextTile = MapManager.Instance.GetTopTileAt(targetCoordinates);

        if (nextTile.HasValue && Mathf.Abs(exitTileCoordinates.z - nextTile.Value.z) <= 1)
        {
            TargetTileCoordinates = nextTile.Value;
            isMoving = true;
            Debug.Log("SetTargetTileForTeleport: true");
            return true;
        }
        Debug.Log("SetTargetTileForTeleport: Can't move forward after teleport");
        return false;
    }

    IEnumerator TeleportWithCooldown(Vector3Int exitTileCoordinates, Collider2D exitGateCollider2D)
    {
        isTeleporting = true;
        exitGateCollider2D.enabled = false;

        yield return new WaitForSeconds(0.1f);

        MovePlayerToTile(exitTileCoordinates);
        bool movedAfterTeleport = SetTargetTileForTeleport(exitTileCoordinates);
        if (!movedAfterTeleport)
        {
            playerDirection = -playerDirection;
        }
        isMoving = true;

        yield return new WaitForSeconds(1f / playerSpeed);

        exitGateCollider2D.enabled = true;

        if (!movedAfterTeleport)
        {
            yield return new WaitForSeconds(0.2f);
        }
        isTeleporting = false;
    }
}
