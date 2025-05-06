using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    public bool IsReady { get; private set; } = false;

    /*
    [SerializeField]
    private OverlayTile overlayTilePrefab;

    [SerializeField]
    private GameObject overlayContainer;

    private Dictionary<Vector2Int, OverlayTile> map;
    */

    [SerializeField]
    private Tilemap tilemap;
    public Tilemap Tilemap => tilemap;

    [SerializeField]
    private GameObject runicGate;
    private List<(GameObject, Vector3Int)> runicGates;

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

        //map = new Dictionary<Vector2Int, OverlayTile>();
        runicGates = new List<(GameObject, Vector3Int)>();
    }

    private void Start()
    {
        //GenerateOverlayTiles();
        IsReady = true;
    }

    /*
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
    */
    public Vector3Int? GetTilePosFromTileCoordinates(Vector2Int tileCoordinates)
    {
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

    public void PlayerInteract(Vector3Int interactedTileCoordinates)
    {
        BoundsInt bounds = tilemap.cellBounds;
        int x = interactedTileCoordinates.x;
        int y = interactedTileCoordinates.y;

        for (int z = bounds.max.z; z > interactedTileCoordinates.z; z--)
        {
            var tilePos = new Vector3Int(x, y, z);
            if (tilemap.HasTile(tilePos))
            {
                Debug.Log("Tile Occupied");
                return;
            }
        }

        if (tilemap.HasTile(interactedTileCoordinates))
        {
            AddRunicGate(interactedTileCoordinates);
        }
    }

    public void AddRunicGate(Vector3Int tileCoordinates)
    {
        if (runicGates.Count >= 2)
        {
            Debug.Log("Too many runicGates");
            return;
        }

        if (!runicGates.Any(i => i.Item2 == tileCoordinates))
        {
            GameObject gate = Instantiate(
                runicGate,
                tilemap.GetCellCenterWorld(tileCoordinates),
                Quaternion.identity
            );
            runicGates.Add((gate, tileCoordinates));
            Debug.Log(gate.transform.position);
        }
    }
}
