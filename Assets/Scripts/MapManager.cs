using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    public bool IsReady { get; private set; } = false;

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

        runicGates = new List<(GameObject, Vector3Int)>();
    }

    private void Start()
    {
        IsReady = true;
    }

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
        Debug.Log($"PlayerInteract after loop {interactedTileCoordinates}");
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
