using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IObstacle
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private RunicGateManager runicGateManager;

    [SerializeField]
    private CursedMimic cursedMimicReference;

    [SerializeField]
    private ExitDoor exitDoor;

    [SerializeField]
    float playerSpeed = 5f;

    [SerializeField]
    int playerHeight = 2;

    [SerializeField]
    int playerJumpHeight = 1;

    [SerializeField]
    private Sprite upSprite;

    [SerializeField]
    private Sprite downSprite;

    [SerializeField]
    private Sprite leftSprite;

    [SerializeField]
    private Sprite rightSprite;

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

    public Vector3Int TilePosition => currentTilePos;

    private bool isMoving;
    private bool isTeleporting;
    private bool isWaiting;
    private GameObject carriedObject;

    private Vector2Int playerDirection;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        currentTilePos = new(0, 0, 0);
        TargetTilePos = new(0, 0, 0);
        isMoving = false;
        isTeleporting = false;
        isWaiting = false;
        playerDirection = new(1, 0);

        MoveToTile(new Vector3Int(0, 0, 0));

        runicGateManager.OnTeleport += (_, e) =>
            StartCoroutine(TeleportWithCooldown(e.targetTilePos, e.exitGateCollider));

        exitDoor.OnExitDoorEntered += (_, e) =>
        {
            if (carriedObject != null)
            {
                Debug.Log("Victory");
            }
        };

        MapManager.Instance.AddObstacle(this);
    }

    void Update()
    {
        if (isWaiting)
        {
            return;
        }
        if (!isMoving && !isTeleporting)
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            if (moveValue.x != 0)
            {
                playerDirection = new Vector2Int((int)Mathf.Sign(moveValue.x), 0);
                SetTargetTile(currentTilePos);
                UpdateSpriteDirection(playerDirection);
            }
            else if (moveValue.y != 0)
            {
                playerDirection = new Vector2Int(0, (int)Mathf.Sign(moveValue.y));
                SetTargetTile(currentTilePos);
                UpdateSpriteDirection(playerDirection);
            }
        }
        else
        {
            MoveTowardsTargetTile();
        }
    }

    private void UpdateSpriteDirection(Vector2Int direction)
    {
        if (MathF.Abs(direction.x) > 0)
        {
            spriteRenderer.sprite = direction.x > 0 ? rightSprite : leftSprite;
        }
        else
        {
            spriteRenderer.sprite = direction.y > 0 ? upSprite : downSprite;
        }
    }

    private void OnInteract()
    {
        if (isMoving || isWaiting)
        {
            return;
        }

        Vector3Int tileToCheck =
            currentTilePos + new Vector3Int(playerDirection.x, playerDirection.y, 0);
        Vector3Int? result = MapManager.Instance.FindAvailableTileAt(
            tileToCheck,
            maxZDiff: 0,
            clearance: 2
        );
        bool targetTileIsAccessible = result.HasValue;
        if (carriedObject != null && targetTileIsAccessible)
        {
            TryDropCarriedObject(tileToCheck);
            return;
        }

        Vector3 worldToCheck = MapManager.Instance.TileToWorld(tileToCheck);
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldToCheck);
        if (colliders.Length == 0)
        {
            if (targetTileIsAccessible)
            {
                runicGateManager.ActivateRunicGate(tileToCheck);
            }
        }
        foreach (Collider2D collider in colliders)
        {
            InteractWithObject(collider, tileToCheck);
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
        Vector3Int targetPos =
            startingTile + new Vector3Int(playerDirection.x, playerDirection.y, 0);

        Vector3Int? nextTile = MapManager.Instance.FindAvailableTileAt(
            targetPos,
            maxZDiff: playerJumpHeight,
            clearance: playerHeight
        );
        if (nextTile.HasValue)
        {
            TargetTilePos = nextTile.Value;
            isMoving = true;
            return true;
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

    public void MoveToTile(Vector3Int tilePos)
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

        MoveToTile(exitTilePos);
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

    public void SetParent(Transform parentTransform)
    {
        transform.SetParent(parentTransform);
        if (parentTransform != null)
        {
            isWaiting = true;
            return;
        }
        isWaiting = false;
        currentTilePos = MapManager.Instance.WorldToTile(transform.position);
    }
}
