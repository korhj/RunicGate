using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IObstacle
{
    [SerializeField]
    InterfaceDataSO interfaceDataSO;

    [SerializeField]
    private LayerMask clickableTileMask;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private RunicGateManager runicGateManager;

    [SerializeField]
    private ExitDoor exitDoor;

    [SerializeField]
    private MouseController mouseController;

    [SerializeField]
    private InteractButtonUI interactButtonUI;

    [SerializeField]
    private RunicGateButtonUI runicGateButtonUI;

    [SerializeField]
    float playerSpeed = 5f;

    [SerializeField]
    int playerHeight = 2;

    [SerializeField]
    int jumpHeight = 1;

    [SerializeField]
    float maxHealth = 100;

    [SerializeField]
    private Sprite upSprite;

    [SerializeField]
    private Sprite downSprite;

    [SerializeField]
    private Sprite leftSprite;

    [SerializeField]
    private Sprite rightSprite;

    InputAction moveAction;
    private PathFinder pathFinder;
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

    public Vector3Int TilePos => currentTilePos;

    private bool isMoving;
    private bool isTeleporting;
    private bool isWaiting;
    private MapManager mapManager;
    private GameObject carriedObject;
    private Vector2Int playerDirection;
    private Vector2Int directionAfterPath;
    private List<SelectedTile> path;
    private SelectedTile lastTile;
    private float health;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        currentTilePos = new(0, 0, 0);
        TargetTilePos = new(0, 0, 0);
        isMoving = false;
        isTeleporting = false;
        isWaiting = false;
        playerDirection = new(1, 0);
        directionAfterPath = new(0, 0);
        pathFinder = new PathFinder();
        path = new();
        lastTile = null;
        MoveToTile(new Vector3Int(0, 0, 0));
        mapManager = MapManager.Instance;
        health = maxHealth;

        runicGateManager.OnTeleport += (_, e) =>
            StartCoroutine(TeleportWithCooldown(e.targetTilePos, e.exitGateCollider));

        exitDoor.OnExitDoorEntered += (_, e) =>
        {
            if (carriedObject != null)
            {
                Debug.Log("Victory!");
            }
        };

        mouseController.OnTileSelected += (_, e) => SetPath(e.targetSelectedTile);
        interactButtonUI.OnInteractButtonPressed += (_, e) => OnInteract();
        runicGateButtonUI.OnRunicGateButtonPressed += (_, e) => RunicGateInteract();

        mapManager.AddObstacle(this);
    }

    void Update()
    {
        if (isWaiting)
        {
            return;
        }
        if (!isMoving && !isTeleporting)
        {
            if (path.Count > 0)
            {
                MoveAlongPath();
            }
            else
            {
                HandleKeyboardMovement();
            }
        }
        else
        {
            MoveTowardsTargetTile();
        }
    }

    private void HandleKeyboardMovement()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        if (moveValue.x != 0)
        {
            Vector2Int direction = new((int)Mathf.Sign(moveValue.x), 0);
            SetTargetTile(currentTilePos);
            UpdateSpriteDirection(direction);
        }
        else if (moveValue.y != 0)
        {
            Vector2Int direction = new(0, (int)Mathf.Sign(moveValue.y));
            SetTargetTile(currentTilePos);
            UpdateSpriteDirection(direction);
        }
    }

    private void UpdateSpriteDirection(Vector2Int direction)
    {
        playerDirection = direction;
        if (MathF.Abs(direction.x) > 0)
        {
            spriteRenderer.sprite = direction.x > 0 ? rightSprite : leftSprite;
        }
        else
        {
            spriteRenderer.sprite = direction.y > 0 ? upSprite : downSprite;
        }
        SetTargetObject();
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
        if (carriedObject != null && result.HasValue)
        {
            if (carriedObject.TryGetComponent(out CursedMimic cursedMimic))
            {
                cursedMimic.DropMimic(tileToCheck);
                carriedObject = null;
                interfaceDataSO.SetPlayerHasObject(false);
                interfaceDataSO.SetTargetObject(cursedMimic.gameObject);
                return;
            }
        }

        Vector3 worldToCheck = MapManager.Instance.TileToWorld(tileToCheck);
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldToCheck);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out IPlayerInteractable interactable))
            {
                GameObject interactResult = interactable.Interact();
                if (interactResult != null)
                {
                    carriedObject = interactResult;
                    interfaceDataSO.SetPlayerHasObject(true);
                    SetTargetObject();
                }
                return;
            }
        }
    }

    private void RunicGateInteract()
    {
        Vector3Int tileToCheck =
            currentTilePos + new Vector3Int(playerDirection.x, playerDirection.y, 0);
        Vector3 worldToCheck = MapManager.Instance.TileToWorld(tileToCheck);
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldToCheck);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<RunicGate>(out _))
            {
                runicGateManager.DeactivateRunicGate(tileToCheck);
                SetTargetObject();
                return;
            }
        }

        Vector3Int? result = MapManager.Instance.FindWalkableTileAt(
            tileToCheck,
            maxZDiff: 0,
            clearance: 2
        );
        if (result.HasValue)
        {
            runicGateManager.ActivateRunicGate(tileToCheck);
            SetTargetObject();
        }
    }

    private bool SetTargetTile(Vector3Int startingTile)
    {
        Vector3Int targetPos =
            startingTile + new Vector3Int(playerDirection.x, playerDirection.y, 0);

        Vector3Int? nextTile = MapManager.Instance.FindWalkableTileAt(
            targetPos,
            maxZDiff: jumpHeight,
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

    private void SetPath(SelectedTile targetSelectedTile)
    {
        if (!isMoving && !isTeleporting && !isWaiting)
        {
            path = pathFinder.FindPath(
                currentTilePos,
                targetSelectedTile.tilePos,
                jumpHeight: jumpHeight,
                clearance: playerHeight
            );
            if (path == null || path.Count < 1)
            {
                Vector2Int direction =
                    new(
                        Mathf.Clamp(targetSelectedTile.tilePos.x - currentTilePos.x, -1, 1),
                        Mathf.Clamp(targetSelectedTile.tilePos.y - currentTilePos.y, -1, 1)
                    );
                UpdateSpriteDirection(direction);
                return;
            }
            lastTile = path.Last();
            if (lastTile != targetSelectedTile)
            {
                directionAfterPath = new(
                    Mathf.Clamp(targetSelectedTile.tilePos.x - lastTile.tilePos.x, -1, 1),
                    Mathf.Clamp(targetSelectedTile.tilePos.y - lastTile.tilePos.y, -1, 1)
                );
            }
        }
    }

    private void MoveTowardsTargetTile()
    {
        Vector3 movementDirection = TargetTileWorldPos - transform.position;
        float movementDistance = playerSpeed * Time.deltaTime;

        if (movementDistance >= movementDirection.magnitude)
        {
            transform.position = TargetTileWorldPos;
            currentTilePos = TargetTilePos;

            if (path.Count > 0)
            {
                MoveAlongPath();
            }
            else
            {
                isMoving = false;
                SetTargetObject();
                if (lastTile != null)
                {
                    lastTile.Hide();
                }

                if (directionAfterPath.magnitude != 0)
                {
                    UpdateSpriteDirection(directionAfterPath);
                    directionAfterPath = new(0, 0);
                }
            }
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

    private void MoveAlongPath()
    {
        Vector3Int nextTilePos = path[0].tilePos;
        SelectedTile nextTile = path[0];
        path.RemoveAt(0);

        Vector3Int direction = nextTilePos - currentTilePos;
        playerDirection = new Vector2Int(direction.x, direction.y);
        UpdateSpriteDirection(playerDirection);
        SetTargetTile(currentTilePos);
    }

    private void SetTargetObject()
    {
        Vector3Int tileToCheck =
            currentTilePos + new Vector3Int(playerDirection.x, playerDirection.y, 0);
        Vector3 worldToCheck = MapManager.Instance.TileToWorld(tileToCheck);
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldToCheck);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<RunicGate>(out _))
            {
                interfaceDataSO.SetTargetObject(collider.gameObject);
                return;
            }
            if (collider.TryGetComponent<IPlayerInteractable>(out _))
            {
                interfaceDataSO.SetTargetObject(collider.gameObject);
                return;
            }
        }
        interfaceDataSO.SetTargetObject(null);
    }

    [ContextMenu("Take Damage")]
    public void TakeDamage(int damage)
    {
        health -= damage;
        interfaceDataSO.SetPlayerHealthPercent(health / maxHealth);
    }

    public void MoveToTile(Vector3Int tilePos)
    {
        Vector3 worldPos = MapManager.Instance.TileToWorld(tilePos);
        transform.position = worldPos;
        currentTilePos = tilePos;
        TargetTilePos = tilePos;
        isMoving = false;
        SetTargetObject();
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
}
