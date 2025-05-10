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
    private GameObject carriedObject;

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
                SetTargetTile(currentTileCoordinates);
            }
            else if (moveValue.y != 0)
            {
                playerDirection = new Vector2Int(0, (int)Mathf.Sign(moveValue.y));
                SetTargetTile(currentTileCoordinates);
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
            currentTileCoordinates + new Vector3Int(playerDirection.x, playerDirection.y, 0);
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
        Vector2Int targetCoordinates =
            new Vector2Int(startingTile.x, startingTile.y) + playerDirection;

        Vector3Int? nextTile = MapManager.Instance.GetTopTileAt(targetCoordinates);

        if (nextTile.HasValue)
        {
            if (Mathf.Abs(startingTile.z - nextTile.Value.z) <= 1)
            {
                TargetTileCoordinates = nextTile.Value;
                isMoving = true;
                return true;
            }
        }
        return false;
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

    IEnumerator TeleportWithCooldown(Vector3Int exitTileCoordinates, Collider2D exitGateCollider2D)
    {
        isTeleporting = true;
        exitGateCollider2D.enabled = false;

        yield return new WaitForSeconds(0.1f);

        MovePlayerToTile(exitTileCoordinates);
        bool movedAfterTeleport = SetTargetTile(exitTileCoordinates);
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
