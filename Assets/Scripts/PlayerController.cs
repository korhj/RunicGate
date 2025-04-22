using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer playerSpriteRenderer;

    [SerializeField]
    float playerSpeed = 5f;

    InputAction moveAction;

    private Vector3Int currentTilePos;
    private Vector3Int targetTilePos;

    private bool isMoving;

    IEnumerator Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        isMoving = false;

        yield return new WaitUntil(
            () => MapManager.Instance != null && MapManager.Instance.IsReady
        );
        currentTilePos = new(0, 0, 0);
        targetTilePos = new(0, 0, 0);

        MovePlayerToTile(new Vector3Int(0, 0, 0));
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            if (moveValue.x != 0)
            {
                SetTargetTile(new Vector2Int((int)Mathf.Sign(moveValue.x), 0));
            }
            else if (moveValue.y != 0)
            {
                SetTargetTile(new Vector2Int(0, (int)Mathf.Sign(moveValue.y)));
            }
        }
        else
        {
            Debug.Log($"MoveTowards {targetTilePos}");
            MoveTowardsTargetTile();
        }
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
                isMoving = true;
            }
            else
            {
                Debug.Log("Path blocked");
            }
        }
    }

    private void MoveTowardsTargetTile()
    {
        float step = playerSpeed * Time.deltaTime;
        Vector3 currentPos = transform.position;

        Vector3 worldTargetPos = MapManager.Instance.Tilemap.GetCellCenterWorld(targetTilePos);
        transform.position = Vector3.MoveTowards(currentPos, worldTargetPos, step);

        if (Vector3.Distance(currentPos, worldTargetPos) < 0.001f)
        {
            isMoving = false;
            transform.position = worldTargetPos;
            currentTilePos = targetTilePos;
        }
    }

    private void MovePlayerToTile(Vector3Int tilePos)
    {
        Vector3 worldPos = MapManager.Instance.Tilemap.GetCellCenterWorld(tilePos);
        transform.position = worldPos;
        currentTilePos = tilePos;
    }
}
