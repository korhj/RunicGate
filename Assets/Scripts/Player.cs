using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer playerSpriteRenderer;

    [SerializeField]
    private RunicGateManager runicGateManager;

    [SerializeField]
    private CursedMimic cursedMimicReference;

    [SerializeField]
    float playerSpeed = 5f;

    InputAction moveAction;
    private Vector3Int currentTilePos;

    private Vector3Int _targetTilePos;
    private Vector3 _targetTileWorldPos;
    private Vector3Int TargetTilePos
    {
        get => _targetTilePos;
        set
        {
            _targetTilePos = value;
            _targetTileWorldPos = MapManager.Instance.TileToWorld(_targetTilePos);
        }
    }
    private Vector3 TargetTileWorldPos => _targetTileWorldPos;
    private bool isMoving;
    private bool isTeleporting;
    private GameObject carriedObject;

    private Vector2Int playerDirection;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        currentTilePos = new(0, 0, 0);
        TargetTilePos = new(0, 0, 0);
        isMoving = false;
        isTeleporting = false;
        playerDirection = new(1, 0);

        MovePlayerToTile(new Vector3Int(0, 0, 0));

        runicGateManager.OnTeleport += (_, e) =>
            StartCoroutine(TeleportWithCooldown(e.targetTilePos, e.exitGateCollider));
        cursedMimicReference.OnTouchedMimic += (_, e) =>
        {
            if (carriedObject == null)
            {
                carriedObject = e.mimicGameObject;
                cursedMimicReference.Interact();
            }
        };
    }

    void Update()
    {
        if (!isMoving && !isTeleporting)
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            if (moveValue.x != 0)
            {
                playerDirection = new Vector2Int((int)Mathf.Sign(moveValue.x), 0);
                SetTargetTile(currentTilePos);
            }
            else if (moveValue.y != 0)
            {
                playerDirection = new Vector2Int(0, (int)Mathf.Sign(moveValue.y));
                SetTargetTile(currentTilePos);
            }
        }
        else
        {
            MoveTowardsTargetTile();
        }
    }

    private void OnInteract()
    {
        if (isMoving)
        {
            return;
        }

        Vector3Int tileToCheck =
            currentTilePos + new Vector3Int(playerDirection.x, playerDirection.y, 0);
        bool targetTileIsAccessible = MapManager.Instance.IsTileAccessible(tileToCheck);

        if (carriedObject != null && targetTileIsAccessible)
        {
            TryDropCarriedObject(tileToCheck);
            return;
        }

        Vector3 worldToCheck = MapManager.Instance.TileToWorld(tileToCheck);
        Collider2D collider = Physics2D.OverlapPoint(worldToCheck);

        if (collider != null)
        {
            InteractWithObject(collider, tileToCheck);
            return;
        }

        if (targetTileIsAccessible)
        {
            runicGateManager.ActivateRunicGate(tileToCheck);
        }
    }

    private void TryDropCarriedObject(Vector3Int tileToCheck)
    {
        if (carriedObject.TryGetComponent<CursedMimic>(out CursedMimic cursedMimic))
        {
            bool tileHasGate = runicGateManager.TileHasGate(tileToCheck);
            if (!tileHasGate)
            {
                Debug.Log("Player.cs DropMimic");
                cursedMimic.DropMimic(tileToCheck);
                carriedObject = null;
            }
        }
    }

    private void InteractWithObject(Collider2D collider, Vector3Int tileToCheck)
    {
        if (collider.TryGetComponent<IPlayerInteractable>(out IPlayerInteractable interactable))
        {
            GameObject interactResult = interactable.Interact();
            if (interactResult != null)
            {
                carriedObject = interactResult;
                Debug.Log($"carriedObject {carriedObject}");
            }
            return;
        }
        if (collider.TryGetComponent<RunicGate>(out _))
        {
            runicGateManager.DeactivateRunicGate(tileToCheck);
        }
    }

    private bool SetTargetTile(Vector3Int startingTile)
    {
        Vector2Int targetPos = new Vector2Int(startingTile.x, startingTile.y) + playerDirection;

        Vector3Int? nextTile = MapManager.Instance.GetTopTileAt(targetPos);
        if (nextTile.HasValue)
        {
            if (Mathf.Abs(startingTile.z - nextTile.Value.z) <= 1)
            {
                TargetTilePos = nextTile.Value;
                isMoving = true;
                return true;
            }
        }
        return false;
    }

    private void MoveTowardsTargetTile()
    {
        Vector3 movementDirection = TargetTileWorldPos - transform.position;
        float movementDistance = playerSpeed * Time.deltaTime;

        if (movementDistance >= movementDirection.magnitude)
        {
            transform.position = TargetTileWorldPos;
            isMoving = false;
            currentTilePos = TargetTilePos;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                TargetTileWorldPos,
                movementDistance
            );
        }
    }

    private void MovePlayerToTile(Vector3Int tilePos)
    {
        Vector3 worldPos = MapManager.Instance.TileToWorld(tilePos);
        transform.position = worldPos;
        currentTilePos = tilePos;
        TargetTilePos = tilePos;
        isMoving = false;
    }

    IEnumerator TeleportWithCooldown(Vector3Int exitTilePos, Collider2D exitGateCollider2D)
    {
        isTeleporting = true;
        exitGateCollider2D.enabled = false;

        yield return new WaitForSeconds(0.1f);

        MovePlayerToTile(exitTilePos);
        bool movedAfterTeleport = SetTargetTile(exitTilePos);
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
