using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    public event EventHandler OnStageClear;

    [SerializeField]
    private WalkableGameObjectsSO walkableGameObjects;

    [SerializeField]
    private RunicGateManager runicGateManager;

    [SerializeField]
    private int childGameObjectCost;

    [SerializeField]
    private int runicGateCost;

    [SerializeField]
    SelectedTile selectedTilePrefab;

    [SerializeField]
    GameObject selectedTileContainer;

    [SerializeField]
    ExitDoor exitDoor;

    [SerializeField]
    Tilemap tilemap;
    private List<IObstacle> obstacles;
    public Dictionary<Vector2Int, SelectedTile> Map { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("MapManager instance already exists");
        }
        Instance = this;

        obstacles = new List<IObstacle>();
        Map = new Dictionary<Vector2Int, SelectedTile>();
        CreateSelectedTiles();
    }

    private void OnEnable()
    {
        exitDoor.OnExitDoorEntered += HandleExitDoorEntered;
    }

    private void OnDisable()
    {
        exitDoor.OnExitDoorEntered -= HandleExitDoorEntered;
    }

    private void HandleExitDoorEntered(object sender, EventArgs e)
    {
        OnStageClear?.Invoke(this, EventArgs.Empty);
    }

    private void CreateSelectedTiles()
    {
        foreach (Transform child in tilemap.transform)
        {
            Vector3Int childTilePos = tilemap.WorldToCell(child.position);
            Vector2Int tileKey = new(childTilePos.x, childTilePos.y);
            if (!Map.ContainsKey(tileKey))
            {
                SelectedTile selectedTile = Instantiate(selectedTilePrefab);
                selectedTile.transform.SetParent(child);

                //selectedTile.transform.position = child.transform.position;
                Vector3 worldPos = TileToWorld(childTilePos);
                selectedTile.transform.position = new(worldPos.x, worldPos.y, worldPos.z + 1);
                selectedTile.tilePos = childTilePos;
                selectedTile.cost = childGameObjectCost;
                Map.Add(tileKey, selectedTile);
            }
        }
        BoundsInt bounds = tilemap.cellBounds;

        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    Vector3Int tilePos = new(x, y, z);
                    Vector2Int tileKey = new(x, y);
                    if (tilemap.HasTile(tilePos) && !Map.ContainsKey(tileKey))
                    {
                        SelectedTile selectedTile = Instantiate(
                            selectedTilePrefab,
                            selectedTileContainer.transform
                        );
                        Vector3 worldPos = TileToWorld(tilePos);

                        selectedTile.transform.position = new(
                            worldPos.x,
                            worldPos.y,
                            worldPos.z + 1
                        );
                        selectedTile.tilePos = tilePos;
                        Map.Add(tileKey, selectedTile);
                    }
                }
            }
        }
    }

    public bool IsAvailable(Vector3Int tilePos)
    {
        foreach (IObstacle obstacle in obstacles)
        {
            if (obstacle.TilePos == tilePos)
            {
                return false;
            }
        }

        foreach (Transform child in tilemap.transform)
        {
            Vector3Int childTilePos = tilemap.WorldToCell(child.position);
            if (childTilePos != tilePos)
            {
                continue;
            }

            GameObject obj = child.gameObject;
            if (obj.TryGetComponent(out MovingPlatform movingPlatform))
            {
                if (movingPlatform.IsMoving)
                {
                    return false;
                }
                return true;
            }
            foreach (var comp in walkableGameObjects.walkableComponents)
            {
                if (obj.GetComponent(comp.GetType()) != null)
                    return true;
            }
            return false;
        }
        if (tilemap.HasTile(tilePos))
        {
            return true;
        }

        return false;
    }

    public Vector3Int? FindWalkableTileAt(Vector3Int tilePos, int maxZDiff, int clearance)
    {
        int x = tilePos.x;
        int y = tilePos.y;
        for (int z = tilePos.z + maxZDiff; z >= tilePos.z - maxZDiff; z--)
        {
            Vector3Int currentTilePos = new(x, y, z);

            if (IsAvailable(currentTilePos))
            {
                for (int i = 1; i <= clearance; i++)
                {
                    Vector3Int tileToCheck = new(x, y, z + i);
                    if (IsAvailable(tileToCheck))
                    {
                        return null;
                    }
                }
                return currentTilePos;
            }
        }

        return null;
    }

    public Vector3Int? FindAvailableTileAt(Vector3Int tilePos, int maxZDiff, int clearance)
    {
        if (runicGateManager.TileHasGate(tilePos))
        {
            return null;
        }
        return FindWalkableTileAt(tilePos, maxZDiff, clearance);
    }

    public Vector3 TileToWorld(Vector3Int tilePos)
    {
        return tilemap.GetCellCenterWorld(tilePos);
    }

    public Vector3Int WorldToTile(Vector3 worldPos)
    {
        return tilemap.WorldToCell(worldPos);
    }

    public Vector3 GetCellSize()
    {
        return tilemap.cellSize;
    }

    public void AddObstacle(IObstacle obstacle)
    {
        if (!obstacles.Contains(obstacle))
        {
            obstacles.Add(obstacle);
        }
    }

    public void RemoveObstacle(IObstacle obstacle)
    {
        obstacles.Remove(obstacle);
    }

    public IObstacle GetObstacle(Vector3Int tilePos)
    {
        foreach (IObstacle obstacle in obstacles)
        {
            if (obstacle.TilePos == tilePos)
            {
                return obstacle;
            }
        }
        return null;
    }

    public void AddRunicGate(Vector3Int tilePos)
    {
        Vector2Int tileKey = new(tilePos.x, tilePos.y);
        if (Map.ContainsKey(tileKey))
        {
            Map[tileKey].cost = runicGateCost;
        }
    }

    public void RemoveRunicGate(Vector3Int tilePos)
    {
        Vector2Int tileKey = new(tilePos.x, tilePos.y);
        if (Map.ContainsKey(tileKey))
        {
            foreach (Transform child in tilemap.transform)
            {
                Vector3Int childTilePos = tilemap.WorldToCell(child.position);
                if (childTilePos == tilePos)
                {
                    Map[tileKey].cost = childGameObjectCost;
                }
            }
            Map[tileKey].cost = 1;
        }
    }
}
