using System;
using UnityEngine;

public class CursedMimic : MonoBehaviour, IPlayerInteractable, IObstacle
{
    [SerializeField]
    Vector3Int startingTilePos;

    [SerializeField]
    Sprite buttonSprite;
    private Vector3Int tilePos;
    public Vector3Int TilePos => tilePos;

    void Start()
    {
        tilePos = startingTilePos;
        MoveToTile(tilePos);
        MapManager.Instance.AddObstacle(this);
    }

    public void MoveToTile(Vector3Int newTilePos)
    {
        Vector3 worldPos = MapManager.Instance.TileToWorld(newTilePos);
        transform.position = worldPos;
        tilePos = newTilePos;
    }

    public void DropMimic(Vector3Int Pos)
    {
        gameObject.SetActive(true);
        MoveToTile(Pos);
        MapManager.Instance.AddObstacle(this);
    }

    public GameObject Interact()
    {
        gameObject.SetActive(false);
        MapManager.Instance.RemoveObstacle(this);
        return gameObject;
    }

    public void SetParent(Transform parentTransform)
    {
        transform.SetParent(parentTransform);
        if (parentTransform != null)
        {
            return;
        }
        tilePos = MapManager.Instance.WorldToTile(transform.position);
    }

    public Sprite GetButtonSprite()
    {
        return buttonSprite;
    }
}
