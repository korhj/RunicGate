using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer playerSpriteRenderer;

    [SerializeField]
    float playerSpeed = 5f;

    private PlayerInput playerInput;
    InputAction moveAction;
    private Vector3Int currentTilePos;
    private Vector3Int targetTilePos;
    private Vector3 targetWorldPos;

    private bool isMoving;
    private Vector2Int playerDir;

    IEnumerator Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        isMoving = false;
        playerDir = new(0, 0);

        yield return new WaitUntil(
            () => MapManager.Instance != null && MapManager.Instance.IsReady
        );

        currentTilePos = new(0, 0, 0);
        targetTilePos = new(0, 0, 0);
        targetWorldPos = new(0, 0, 0);

        MovePlayerToTile(new Vector3Int(0, 0, 0));
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            //Debug.Log(moveValue);
            if (moveValue.x != 0)
            {
                //Debug.Log(moveValue);
                playerDir = new Vector2Int((int)Mathf.Sign(moveValue.x), 0);
                SetTargetTile(new Vector2Int((int)Mathf.Sign(moveValue.x), 0));
            }
            else if (moveValue.y != 0)
            {
                playerDir = new Vector2Int(0, (int)Mathf.Sign(moveValue.y));
                SetTargetTile(new Vector2Int(0, (int)Mathf.Sign(moveValue.y)));
            }
        }
        else
        {
            //Debug.Log($"MoveTowards {targetTilePos}");
            MoveTowardsTargetTile();
        }
    }

    private void OnInteract()
    {
        Debug.Log($"Player currentTilePos {currentTilePos}");
        MapManager.Instance.PlayerInteract(
            currentTilePos + new Vector3Int(playerDir.x, playerDir.y, 0)
        );
    }

    private void SetTargetTile(Vector2Int moveDirection)
    {
        if (isMoving)
            return;

        Vector2Int currentPos = new Vector2Int(currentTilePos.x, currentTilePos.y);
        Vector2Int targetPos = currentPos + moveDirection;

        Vector3Int? nextTile = MapManager.Instance.GetTilePosFromTileCoordinates(targetPos);

        if (nextTile.HasValue)
        {
            if (Mathf.Abs(currentTilePos.z - nextTile.Value.z) <= 1)
            {
                targetTilePos = nextTile.Value;
                targetWorldPos = MapManager.Instance.Tilemap.GetCellCenterWorld(targetTilePos);
                isMoving = true;
            }
            else
            {
                //Debug.Log("Path blocked");
            }
        }
    }

    private void MoveTowardsTargetTile()
    {
        Vector3 movementDirection = targetWorldPos - transform.position;
        float movementDistance = playerSpeed * Time.deltaTime;

        if (movementDistance >= movementDirection.magnitude)
        {
            transform.position = targetWorldPos;
            isMoving = false;
            currentTilePos = targetTilePos;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetWorldPos,
                movementDistance
            );
        }
    }

    private void MovePlayerToTile(Vector3Int tilePos)
    {
        Vector3 worldPos = MapManager.Instance.Tilemap.GetCellCenterWorld(tilePos);
        transform.position = worldPos;
        currentTilePos = tilePos;
    }
}
