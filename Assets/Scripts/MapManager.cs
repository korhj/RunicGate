using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    public bool IsReady { get; private set; } = false;

    [SerializeField]
    private OverlayTile overlayTilePrefab;

    [SerializeField]
    private GameObject overlayContainer;

    private Dictionary<Vector2Int, OverlayTile> map;

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

        map = new Dictionary<Vector2Int, OverlayTile>();
    }

    private void Start()
    {
        GenerateOverlayTiles();
        IsReady = true;
    }

    private void GenerateOverlayTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.max.y; y >= bounds.min.y; y--)
            {
                for (int x = bounds.max.x; x >= bounds.min.x; x--)
                {
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);
                    if (tilemap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(
                            overlayTilePrefab,
                            overlayContainer.transform
                        );
                        var cellWorldPosition = tilemap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(
                            cellWorldPosition.x,
                            cellWorldPosition.y,
                            cellWorldPosition.z + 1
                        );

                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tilemap
                            .GetComponent<TilemapRenderer>()
                            .sortingOrder;

                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }
    }

    void Update() { }

    public OverlayTile GetOverlayTileFromTileLocation(Vector2Int tileLocation)
    {
        if (map.ContainsKey(tileLocation))
        {
            return map[tileLocation];
        }
        else
        {
            Debug.LogWarning($"Tile at {tileLocation} not found.");
            return null;
        }
    }

    public Vector3Int? GetTilePosFromTileCoordinates(Vector2Int tileCoordinates)
    {
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap not found.");
            return null;
        }

        BoundsInt bounds = tilemap.cellBounds;
        int x = tileCoordinates.x;
        int y = tileCoordinates.y;

        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            var tilePos = new Vector3Int(x, y, z);
            if (tilemap.HasTile(tilePos))
            {
                return tilePos;
            }
        }

        return null;
    }
}
