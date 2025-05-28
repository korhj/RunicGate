using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private WalkableGameObjectsSO walkableGameObjects;

    [SerializeField]
    SelectedTile selectedTilePrefab;

    [SerializeField]
    GameObject selectedTileContainer;
    public Tilemap Tilemap => tilemap;

    private List<IObstacle> obstacles;
    private Dictionary<Vector2Int, SelectedTile> map;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("MapManager instance already exists");
        }
        Instance = this;

        if (tilemap == null)
        {
            Debug.LogError("Tilemap not found.");
        }

        obstacles = new List<IObstacle>();
        map = new Dictionary<Vector2Int, SelectedTile>();
        CreateSelectedTiles();
    }

    private void CreateSelectedTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    Vector3Int tilePos = new(x, y, z);
                    Vector2Int tileKey = new(x, y);
                    if (tilemap.HasTile(tilePos) && !map.ContainsKey(tileKey))
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
                        map.Add(tileKey, selectedTile);
                    }
                }
            }
        }
    }

    private bool IsAvailable(Vector3Int tilePos)
    {
        foreach (IObstacle obstacle in obstacles)
        {
            if (obstacle.TilePosition == tilePos)
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

    public Vector3Int? FindAvailableTileAt(Vector3Int tilePos, int maxZDiff, int clearance)
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
            if (obstacle.TilePosition == tilePos)
            {
                return obstacle;
            }
        }
        return null;
    }
}
