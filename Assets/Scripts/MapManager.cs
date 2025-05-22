using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private WalkableGameObjectsSO walkableGameObjects;
    public Tilemap Tilemap => tilemap;

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
    }

    private bool HasTileOrGameObject(Vector3Int tilePos)
    {
        if (tilemap.HasTile(tilePos))
        {
            return true;
        }
        foreach (Transform child in tilemap.transform)
        {
            Vector3Int childTilePos = tilemap.WorldToCell(child.position);
            if (childTilePos == tilePos)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsWalkable(Vector3Int tilePos)
    {
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

    public Vector3Int? GetTopTileAt(Vector2Int tilePos)
    {
        BoundsInt bounds = tilemap.cellBounds;
        int x = tilePos.x;
        int y = tilePos.y;

        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            Vector3Int currentTilePos = new(x, y, z);
            if (HasTileOrGameObject(currentTilePos))
            {
                return currentTilePos;
            }
        }

        return null;
    }

    public Vector3Int? FindWalkableTileAt(Vector3Int tilePos, int height, int clearance)
    {
        int x = tilePos.x;
        int y = tilePos.y;
        for (int z = tilePos.z + height; z >= tilePos.z - height; z--)
        {
            Vector3Int currentTilePos = new(x, y, z);

            if (IsWalkable(currentTilePos))
            {
                for (int i = 1; i <= clearance; i++)
                {
                    Vector3Int tileToCheck = new(x, y, z + i);
                    if (IsWalkable(tileToCheck))
                    {
                        return null;
                    }
                }
                return currentTilePos;
            }
        }

        return null;
    }

    public bool IsTileAccessible(Vector3Int tilePos)
    {
        BoundsInt bounds = tilemap.cellBounds;
        int x = tilePos.x;
        int y = tilePos.y;

        for (int z = bounds.max.z; z > tilePos.z; z--)
        {
            Vector3Int currentTilePos = new(x, y, z);
            if (HasTileOrGameObject(currentTilePos))
            {
                return false;
            }
        }
        return HasTileOrGameObject(tilePos);
    }
}
