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

    public Vector3 TileToWorld(Vector3Int tileCooridnates)
    {
        return tilemap.GetCellCenterWorld(tileCooridnates);
    }

    public Vector3Int? GetTopTileAt(Vector2Int tileCoordinates)
    {
        BoundsInt bounds = tilemap.cellBounds;
        int x = tileCoordinates.x;
        int y = tileCoordinates.y;

        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            var currentTileCoordinates = new Vector3Int(x, y, z);
            if (tilemap.HasTile(currentTileCoordinates))
            {
                return currentTileCoordinates;
            }
        }

        return null;
    }

    public bool IsTileAccessible(Vector3Int tileCoordinates)
    {
        BoundsInt bounds = tilemap.cellBounds;
        int x = tileCoordinates.x;
        int y = tileCoordinates.y;

        for (int z = bounds.max.z; z > tileCoordinates.z; z--)
        {
            var currentTileCoordinates = new Vector3Int(x, y, z);
            if (tilemap.HasTile(currentTileCoordinates))
            {
                return false;
            }
        }
        return tilemap.HasTile(tileCoordinates);
    }
}
